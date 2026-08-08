using ProjectPlanner.Application.Common.Dtos.AI;

namespace ProjectPlanner.Application.Services;

public interface IRagService
{
    Task<RagQueryResponseDto> QueryAsync(
        RagQueryRequestDto request,
        CancellationToken cancellationToken = default);
}
