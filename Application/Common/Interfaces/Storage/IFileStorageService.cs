using Microsoft.AspNetCore.Http;

namespace ProjectPlanner.Application.Common.Interfaces.Storage;

public interface IFileStorageService
{
    Task<FileStorageResult> SaveAsync(
        int issueId,
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storedFileName,
        CancellationToken cancellationToken = default);
}