using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Search;

public class SearchIndexStartupService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Create a scope to resolve scoped repositories during startup
        using var scope = serviceProvider.CreateScope();
        var searchIndexRepo = scope.ServiceProvider.GetRequiredService<ISearchIndexRepository>();

        await searchIndexRepo.EnsureIndexCreatedAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}