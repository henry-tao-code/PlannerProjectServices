import json
import logging
import sys
from kafka import KafkaConsumer

# Configure clean logging output
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S"
)
logger = logging.getLogger("test-reader")


def main():
    logger.info("Initializing Kafka Consumer for 'document.parsed' topic...")

    # Using a dedicated group_id so it reads all available messages from the beginning
    consumer = KafkaConsumer(
        "document.parsed",
        bootstrap_servers="localhost:9092",
        group_id="docling-test-reader-group",
        auto_offset_reset="earliest",
        value_deserializer=lambda m: json.loads(m.decode("utf-8"))
    )

    logger.info("Reader active! Listening for parsed document events...")
    logger.info("Press Ctrl + C in this terminal to exit.\n")

    try:
        for message in consumer:
            event = message.value

            # Extract attachment ID and chunks list safely
            attachment_id = event.get("attachmentId") or event.get("AttachmentId")
            chunks = event.get("chunks", [])

            print("\n" + "=" * 80)
            print(f" 📄 PARSED EVENT RECEIVED | Attachment ID: {attachment_id}")
            print(f" 📊 Total Chunks Generated: {len(chunks)}")
            print("=" * 80)

            # Loop through and print every chunk completely
            for chunk in chunks:
                chunk_index = chunk.get("index", 0)
                content = chunk.get("content", "").strip()

                print(f"\n--- [ Chunk {chunk_index + 1} of {len(chunks)} ] ---")
                print(content)

            print("\n" + "=" * 80)
            print(" ⏳ Waiting for next uploaded document event...\n")

    except KeyboardInterrupt:
        logger.info("\nCtrl+C detected. Shutting down test reader...")
    except Exception as e:
        logger.error(f"Unexpected error in reader loop: {e}")
    finally:
        consumer.close()
        logger.info("Kafka connection closed. Exiting.")
        sys.exit(0)


if __name__ == "__main__":
    main()