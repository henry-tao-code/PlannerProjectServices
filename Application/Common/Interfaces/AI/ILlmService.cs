namespace ProjectPlanner.Application.Common.Interfaces.AI;

public interface ILlmService
{
    Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}