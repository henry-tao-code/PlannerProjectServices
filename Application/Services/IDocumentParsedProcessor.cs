using Contracts.Events;

namespace ProjectPlanner.Application.Services;

public interface IDocumentParsedProcessor
{
    Task ProcessAsync(
        DocumentParsedEvent parsedEvent,
        CancellationToken cancellationToken);
}