using ProjectPlanner.Api.Endpoints;
using ProjectPlanner.Application;
using ProjectPlanner.Application.Common.Settings;
using ProjectPlanner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices();

builder.Services.AddProjectPlannerDbContext(builder.Configuration);
builder.Services.AddDataRepositories();
builder.Services.AddMessagingServices(builder.Configuration);
builder.Services.AddSecurityServices(builder.Configuration);

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectPlanner API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapAllApplicationEndpoints();

app.Run();