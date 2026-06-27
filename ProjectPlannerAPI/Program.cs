using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ProjectPlanner.Api.Endpoints;
using ProjectPlanner.Application;
using ProjectPlanner.Infrastructure;
using ProjectPlanner.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddApplicationServices();

builder.Services.AddProjectPlannerDbContext(builder.Configuration);
builder.Services.AddDataRepositories();
builder.Services.AddSecurityServices(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ProjectPlannerAPI v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapAllApplicationEndpoints();

app.Run();