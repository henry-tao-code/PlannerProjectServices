import os
import logging

logger = logging.getLogger("docling-worker.storage")

# Default root inside the container
STORAGE_BASE_DIR = os.getenv("STORAGE_BASE_DIR", "/app/wwwroot/uploads")

def resolve_storage_path(storage_key: str) -> str:
    clean_key = storage_key.replace("\\", "/").lstrip("/")
    
    prefixes_to_remove = [
        "ProjectPlannerAPI/wwwroot/uploads/",
        "PlannerProjectServices/API/wwwroot/uploads/",
        "app/wwwroot/uploads/"
    ]
    for prefix in prefixes_to_remove:
        if clean_key.startswith(prefix):
            clean_key = clean_key[len(prefix):]

    # Combine with container base directory
    full_path = os.path.normpath(os.path.join(STORAGE_BASE_DIR, clean_key))

    if not os.path.exists(full_path):
        logger.error("File does not exist at resolved path: %s", full_path)
        raise FileNotFoundError(f"File not found: {full_path}")

    return full_path