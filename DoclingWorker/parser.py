import logging
from pathlib import Path
from typing import List, Dict, Any, Union
from docling.document_converter import DocumentConverter
from docling.chunking import HybridChunker

logger = logging.getLogger("docling-worker.parser")

logger.info("Initializing Docling DocumentConverter and Chunker...")
converter = DocumentConverter()
chunker = HybridChunker()
logger.info("Docling parser ready.")

def parse_and_chunk_document(file_path: str | Path):
    path = Path(file_path)

    if not path.is_file():
        raise FileNotFoundError(path)

    logger.info("Parsing %s", path)

    result = converter.convert(path)

    return [
        {
            "index": i,
            "content": chunker.serialize(chunk)
        }
        for i, chunk in enumerate(chunker.chunk(result.document))
    ]