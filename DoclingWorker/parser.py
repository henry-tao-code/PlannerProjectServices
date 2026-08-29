import logging
import os
import re
from pathlib import Path
from typing import List, Dict, Any, Union
from docling.document_converter import DocumentConverter
from docling.chunking import HybridChunker

logger = logging.getLogger("docling-worker.parser")

# Keep the end of the preceding chunk with the next one so a retrieval result
# retains context that spans a chunk boundary.
TOKEN_OVERLAP = 50
MAX_CHUNK_TOKENS = int(os.getenv("DOCLING_MAX_CHUNK_TOKENS", "450"))

logger.info("Initializing Docling DocumentConverter and Chunker...")
converter = DocumentConverter()
chunker = HybridChunker(max_tokens=MAX_CHUNK_TOKENS)
logger.info("Docling parser ready.")


def _trailing_token_text(text: str, token_count: int = TOKEN_OVERLAP) -> str:
    """Return the final ``token_count`` tokenizer tokens as source text.

    Docling's tokenizer is also used for its chunk-size limits, so using its
    offsets keeps the overlap aligned with the embedding model.  The regex
    fallback keeps parsing available for tokenizer implementations that do not
    expose offsets.
    """
    if not text or token_count <= 0:
        return ""

    try:
        tokenizer = chunker.tokenizer.get_tokenizer()
        offsets = tokenizer(
            text,
            add_special_tokens=False,
            return_offsets_mapping=True,
        )["offset_mapping"]
        offsets = [offset for offset in offsets if offset[1] > offset[0]]

        if offsets:
            start = offsets[max(0, len(offsets) - token_count)][0]
            return text[start:].strip()
    except (AttributeError, KeyError, TypeError, ValueError):
        logger.debug("Tokenizer offsets unavailable; using approximate token overlap.")

    tokens = list(re.finditer(r"\S+", text))
    return text[tokens[max(0, len(tokens) - token_count)].start():].strip() if tokens else ""


def _token_count(text: str) -> int:
    """Count tokens with the same tokenizer used to enforce chunk limits."""
    try:
        return chunker.tokenizer.count_tokens(text)
    except (AttributeError, TypeError, ValueError):
        try:
            tokenizer = chunker.tokenizer.get_tokenizer()
            return len(tokenizer.encode(text, add_special_tokens=False))
        except (AttributeError, TypeError, ValueError):
            return len(re.findall(r"\S+", text))


def _chunk_page(chunk) -> int | None:
    """Return the first source page represented by a Docling chunk."""
    for item in getattr(getattr(chunk, "meta", None), "doc_items", []):
        location = _location(item)
        if "page" in location:
            return location["page"]
    return None


def _split_table_text(prefix: str, markdown: str) -> list[str]:
    """Split deterministic table Markdown into bounded, header-aware chunks."""
    lines = [line.rstrip() for line in markdown.splitlines() if line.strip()]
    if not lines:
        return []

    # Repeat the Markdown header in every part so rows remain understandable.
    header = lines[:2] if len(lines) > 1 and set(lines[1]) <= {"|", "-", ":", " "} else []
    body = lines[2:] if header else lines
    parts: list[str] = []
    current = header.copy()

    for line in body:
        candidate = "\n".join([prefix, *current, line])
        if len(current) > len(header) and _token_count(candidate) > MAX_CHUNK_TOKENS:
            parts.append("\n".join([prefix, *current]))
            current = [*header, line]
        else:
            current.append(line)

    if current:
        parts.append("\n".join([prefix, *current]))
    return parts


def _location(item) -> dict[str, Any]:
    """Extract the first page and bounding box associated with a Docling item."""
    provenance = getattr(item, "prov", [])
    if not provenance:
        return {}

    source = provenance[0]
    location = {"page": source.page_no}
    bbox = getattr(source, "bbox", None)
    if bbox is not None:
        location["bbox"] = [bbox.l, bbox.t, bbox.r, bbox.b]
    return location


def extract_document_structure(document) -> dict[str, Any]:
    """Build a deterministic, JSON-serializable representation of a document.

    This deliberately preserves table source data and locations without adding
    an LLM transformation step.  Consumers can use it for display, citations,
    or later table-specific processing.
    """
    tables = []
    for index, table in enumerate(document.tables):
        tables.append(
            {
                "index": index,
                **_location(table),
                "rowCount": table.data.num_rows,
                "columnCount": table.data.num_cols,
                "markdown": table.export_to_markdown(doc=document),
                "html": table.export_to_html(doc=document),
                "data": table.data.model_dump(mode="json"),
            }
        )

    pictures = [
        {"index": index, **_location(picture)}
        for index, picture in enumerate(document.pictures)
    ]

    pages = [
        {
            "page": page_number,
            "width": page.size.width,
            "height": page.size.height,
        }
        for page_number, page in sorted(document.pages.items())
    ]

    return {
        "metadata": {
            "documentName": document.name,
            "pageCount": len(document.pages),
            "textBlockCount": len(document.texts),
            "tableCount": len(tables),
            "pictureCount": len(pictures),
        },
        "pages": pages,
        "tables": tables,
        "pictures": pictures,
    }


def parse_and_extract_document(file_path: str | Path) -> dict[str, Any]:
    path = Path(file_path)

    if not path.is_file():
        raise FileNotFoundError(path)

    logger.info("Parsing %s", path)

    result = converter.convert(path)

    chunks: list[dict[str, Any]] = []
    previous_original_content = ""

    for chunk in chunker.chunk(result.document):
        original_content = chunker.serialize(chunk)
        overlap = _trailing_token_text(previous_original_content)
        content = original_content
        if overlap:
            content = f"{overlap}\n\n{content}"

        chunks.append(
            {
                "index": len(chunks),
                "content": content,
                "tokenCount": _token_count(content),
                "type": "content",
                "page": _chunk_page(chunk),
            }
        )
        previous_original_content = original_content

    # Index exact, deterministic table text separately. This makes a table
    # independently retrievable without relying on an LLM-generated summary.
    for table_index, table in enumerate(result.document.tables):
        location = _location(table)
        page = location.get("page")
        prefix = f"Table {table_index + 1}" + (f" on page {page}" if page is not None else "")
        markdown = table.export_to_markdown(doc=result.document)
        for part in _split_table_text(prefix, markdown):
            chunks.append(
                {
                    "index": len(chunks),
                    "content": part,
                    "tokenCount": _token_count(part),
                    "type": "table",
                    "page": page,
                    "tableIndex": table_index,
                }
            )

    return {
        "chunks": chunks,
        "structure": extract_document_structure(result.document),
    }


def parse_and_chunk_document(file_path: str | Path) -> list[dict[str, Any]]:
    """Backward-compatible chunk-only view of ``parse_and_extract_document``."""
    return parse_and_extract_document(file_path)["chunks"]
