using Contracts.Constants;
using Contracts.Events;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using ProjectPlanner.Application.Common.Dtos.Issue;
using ProjectPlanner.Application.Common.Interfaces.Messaging;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Application.Common.Interfaces.Storage;

namespace ProjectPlanner.Application.Services.Implementations;

public class IssueAttachmentService(
    IIssueAttachmentRepository attachmentRepository,
    IIssueRepository issueRepository,
    IKafkaProducer kafkaProducer,
    IFileStorageService fileStorageService,
    IUnitOfWork unitOfWork) : IIssueAttachmentService
{
    public async Task<IssueAttachmentDto> UploadAsync(
    int issueId,
    IFormFile file,
    CancellationToken cancellationToken = default)
    {
        var issue = await issueRepository.GetByIdAsync(
            issueId,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Issue {issueId} does not exist.");

        var storageResult =
            await fileStorageService.SaveAsync(
                issueId,
                file,
                cancellationToken);

        var attachment = new IssueAttachment
        {
            IssueId = issueId,
            OriginalFileName = file.FileName,
            StoredFileName = storageResult.StoredFileName,
            StorageKey = storageResult.StorageKey,
            ContentType = file.ContentType,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };

        await attachmentRepository.AddAsync(
            attachment,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        var uploadedEvent =
            new AttachmentUploadedEvent
            (
                attachment.Id,
                attachment.IssueId,
                attachment.StorageKey,
                attachment.OriginalFileName,
                attachment.ContentType,
                attachment.FileSize,
                attachment.UploadedAt
            );


        await kafkaProducer.PublishAsync(
            KafkaTopics.AttachmentUploaded,
            uploadedEvent,
            cancellationToken);

        return new IssueAttachmentDto
        {
            Id = attachment.Id,
            IssueId = attachment.IssueId,
            OriginalFileName = attachment.OriginalFileName,
            StoredFileName = attachment.StoredFileName,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize,
            UploadedAt = attachment.UploadedAt
        };
    }

    public async Task<IEnumerable<IssueAttachment>> GetByIssueIdAsync(
        int issueId,
        CancellationToken cancellationToken = default)
    {
        return await attachmentRepository.GetByIssueIdAsync(
            issueId,
            cancellationToken);
    }

    public async Task<IssueAttachment?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await attachmentRepository.GetByIdAsync(
            id,
            cancellationToken);
    }

    public async Task MarkProcessedAsync(
    DocumentProcessedEvent message,
    CancellationToken ct)
    {
        var attachment =
            await attachmentRepository.GetByIdAsync(
                message.AttachmentId,
                ct);

        if (attachment == null)
            return;


        if (message.Success)
        {
            attachment.Status =
                AttachmentStatus.Indexed;

            attachment.ProcessedAt =
                message.ProcessedAt;
        }
        else
        {
            attachment.Status =
                AttachmentStatus.Failed;

            attachment.ProcessingError =
                message.ErrorMessage;
        }


        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(
    long id,
    CancellationToken cancellationToken = default)
    {
        var attachment = await attachmentRepository.GetByIdAsync(
            id,
            cancellationToken) ?? throw new KeyNotFoundException(
                $"Attachment {id} does not exist.");
        attachmentRepository.Remove(attachment);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}