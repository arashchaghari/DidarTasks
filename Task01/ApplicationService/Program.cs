using ApplicationService;
using ApplicationService.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Subscription;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddCheck<PackagingServiceHealthCheck>("packaging_service");

builder.Services.AddGrpcClient<PackagingGrpcService.PackagingGrpcServiceClient>(o =>
{
    o.Address = new Uri("https://packaging-service-url");
});

builder.Services.AddTransient<PackagingClient>();
builder.Services.AddTransient<PackagingClientWithRollbackService>();

var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");

app.Run();
