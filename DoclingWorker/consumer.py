import os
import time
import json
import logging
from kafka import KafkaConsumer

from storage import resolve_storage_path
from parser import parse_and_chunk_document
from producer import send_parsed_event, send_failed_event

logger = logging.getLogger("docling-worker.consumer")

BOOTSTRAP_SERVERS = os.getenv("KAFKA_BOOTSTRAP_SERVERS", "kafka:29092")

def create_consumer(retries: int = 10, delay: float = 3.0) -> KafkaConsumer:
    """
    Creates and returns a KafkaConsumer instance with startup retry logic.
    """
    logger.info("Connecting Kafka Consumer to %s...", BOOTSTRAP_SERVERS)
    for attempt in range(1, retries + 1):
        try:
            consumer = KafkaConsumer(
                "attachment.uploaded",
                bootstrap_servers=BOOTSTRAP_SERVERS,
                group_id="docling-worker-group",
                auto_offset_reset="latest",
                value_deserializer=lambda m: json.loads(m.decode("utf-8"))
            )
            logger.info("Kafka Consumer connected successfully.")
            return consumer
        except Exception as e:
            logger.warning(
                "Attempt %d/%d: Failed to connect consumer to %s (%s). Retrying in %.1fs...",
                attempt, retries, BOOTSTRAP_SERVERS, e, delay
            )
            time.sleep(delay)

    raise RuntimeError(f"Could not connect Kafka Consumer to {BOOTSTRAP_SERVERS}")


def start_consumer():
    consumer = create_consumer()

    logger.info("Docling worker started, waiting for messages...")

    for message in consumer:
        event = message.value
        
        # Extract fields (handling camelCase or PascalCase)
        attachment_id = event.get("attachmentId") or event.get("AttachmentId")
        storage_key = event.get("storageKey") or event.get("StorageKey")

        if not storage_key or attachment_id is None:
            logger.warning("Skipping invalid event: %s", event)
            continue

        try:
            # 1. Resolve physical path
            file_path = resolve_storage_path(storage_key)

            # 2. Convert and chunk document via Docling
            chunks = parse_and_chunk_document(file_path)

            # 3. PUBLISH TO KAFKA ('document.parsed' topic)
            send_parsed_event(attachment_id=attachment_id, chunks=chunks)
            logger.info("Successfully published %d chunks for attachmentId: %d", len(chunks), attachment_id)

        except Exception as e:
            logger.error("Failed processing attachment %d: %s", attachment_id, e)
            
            # 4. PUBLISH FAILURE TO KAFKA ('attachment.failed' topic)
            send_failed_event(attachment_id=attachment_id, reason=str(e))