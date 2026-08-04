namespace ProjectPlanner.Application.Common.Interfaces.Storage;

public record FileStorageResult(
    string StoredFileName,
    string StorageKey);