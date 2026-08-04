using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ProjectPlanner.Application.Common.Interfaces.Storage;
using ProjectPlanner.Application.Common.Settings;

namespace ProjectPlanner.Infrastructure.Storage;

public class LocalFileStorageService(
    IWebHostEnvironment environment,
    IOptions<FileStorageOptions> options) : IFileStorageService
{
    private readonly string _uploadPath = Path.Combine(
        environment.WebRootPath ?? environment.ContentRootPath,
        options.Value.BasePath);

    public async Task<FileStorageResult> SaveAsync(
        int issueId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";

        var relativePath = Path.Combine(
            "issues",
            issueId.ToString(),
            storedFileName);

        var fullPath = Path.Combine(_uploadPath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return new FileStorageResult(
            storedFileName,
            relativePath.Replace("\\", "/"));
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var normalizedKey = storageKey.Replace('/', Path.DirectorySeparatorChar)
                                      .Replace('\\', Path.DirectorySeparatorChar);

        var fullPath = Path.Combine(_uploadPath, normalizedKey);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}