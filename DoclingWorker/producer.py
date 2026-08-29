import os
import time
import json
import logging
from typing import List, Dict, Any
from kafka import KafkaProducer

logger = logging.getLogger("docling-worker.producer")

BOOTSTRAP_SERVERS = os.getenv("KAFKA_BOOTSTRAP_SERVERS", "kafka:29092")

_producer = None


def get_producer(retries: int = 10, delay: float = 3.0) -> KafkaProducer:
    global _producer
    if _producer is not None:
        return _producer

    logger.info("Initializing Kafka Producer connecting to %s...", BOOTSTRAP_SERVERS)
    for attempt in range(1, retries + 1):
        try:
            _producer = KafkaProducer(
                bootstrap_servers=BOOTSTRAP_SERVERS,
                value_serializer=lambda v: json.dumps(v).encode("utf-8")
            )
            logger.info("Kafka Producer initialized successfully.")
            return _producer
        except Exception as e:
            logger.warning(
                "Attempt %d/%d: Failed to connect producer to %s (%s). Retrying in %.1fs...",
                attempt, retries, BOOTSTRAP_SERVERS, e, delay
            )
            time.sleep(delay)

    logger.error("Could not connect Kafka Producer after %d attempts.", retries)
    raise RuntimeError(f"Could not connect Kafka Producer to {BOOTSTRAP_SERVERS}")


def send_parsed_event(
    attachment_id: int,
    chunks: List[Dict[str, Any]],
    structure: Dict[str, Any] | None = None,
) -> None:
    producer = get_producer()
    payload = {
        "attachmentId": attachment_id,
        "chunks": chunks
    }
    if structure is not None:
        payload["structure"] = structure
    
    topic = "document.parsed"
    logger.info("Publishing parsed event for attachmentId: %d to topic: %s", attachment_id, topic)
    
    future = producer.send(topic, value=payload)
    future.get(timeout=10)


def send_failed_event(attachment_id: int, reason: str) -> None:
    producer = get_producer()
    payload = {
        "attachmentId": attachment_id,
        "status": "Failed",
        "reason": reason
    }
    
    topic = "attachment.failed"
    logger.warning("Publishing failure event for attachmentId: %d to topic: %s", attachment_id, topic)
    
    future = producer.send(topic, value=payload)
    future.get(timeout=10)
