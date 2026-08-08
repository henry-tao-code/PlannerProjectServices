using Domain.Entities;
using ProjectPlanner.Application.Common.Dtos.AI;

namespace ProjectPlanner.Application.Services;

public interface IPromptBuilderService
{
    string Build(
        string query,
        IReadOnlyList<RagSourceDto> sources,
        QueryRoute route);
}