using Contracts.Events;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using Domain.Enums;
using System.Text.Json;

namespace ProjectPlanner.Application.Services.Implementations;

public class DocumentParsedProcessor(
    IIssueAttachmentRepository attachmentRepository,
    IHybridIndexIngestor hybridIndexIngestor)
    : IDocumentParsedProcessor
{
    public async Task ProcessAsync(
        DocumentParsedEvent parsedEvent,
        CancellationToken cancellationToken)
    {
        var attachment = await attachmentRepository.GetByIdAsync(
            parsedEvent.AttachmentId,
            cancellationToken);

        if (attachment is not null)
        {
            if (parsedEvent.Structure is not null)
            {
                attachment.DocumentStructureJson = JsonSerializer.Serialize(parsedEvent.Structure);
            }

            attachment.Status = AttachmentStatus.Indexed;
            attachment.ProcessedAt = DateTime.UtcNow;
            attachment.ProcessingError = null;
        }

        await hybridIndexIngestor.IngestAsync(
            parsedEvent.AttachmentId,
            parsedEvent.Chunks,
            cancellationToken);
    }
}
