using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProjectPlanner.Application.Common;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence;

public class UnitOfWork(ProjectPlannerDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw pgEx.SqlState switch
            {
                "23505" => new InvalidOperationException("Duplicate record detected.", ex),
                "23503" => new InvalidOperationException("Referenced record does not exist.", ex),
                "23502" => new InvalidOperationException("A required field is missing.", ex),
                _ => new PersistenceException("Database update failed.", ex)
            };
        }
    }
}