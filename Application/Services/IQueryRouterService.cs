using Domain.Entities;

namespace ProjectPlanner.Application.Services;

public interface IQueryRouterService
{
    QueryRoute Route(string query);
}