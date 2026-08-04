import logging
import signal
import sys

from consumer import start_consumer

# Configure logging to output formatted logs to stdout
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
    handlers=[logging.StreamHandler(sys.stdout)],
)

logger = logging.getLogger("docling-worker")


def handle_shutdown(sig, frame):
    """Gracefully handles termination signals (SIGINT / SIGTERM)."""
    logger.info("Received signal %s. Shutting down worker...", sig)
    sys.exit(0)


def main():
    # Register handlers for Ctrl+C and termination signals
    signal.signal(signal.SIGINT, handle_shutdown)
    signal.signal(signal.SIGTERM, handle_shutdown)

    logger.info("Starting Docling worker...")

    try:
        start_consumer()
    except KeyboardInterrupt:
        logger.info("Worker interrupted.")
    except Exception:
        logger.exception("Worker crashed unexpectedly.")
        sys.exit(1)


if __name__ == "__main__":
    main()