using Contracts.Dtos;

namespace ProjectPlanner.Application.Common.Interfaces.AI;

public interface ITEIEmbeddingService
{
    Task<EmbeddingResponseDto> GetEmbeddingsAsync(
        EmbeddingRequestDto request,
        CancellationToken cancellationToken = default);
}