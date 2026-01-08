using Azure.Core.Serialization;
using CarHistory.Api.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

// Configuration sources
builder.Configuration
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

// JSON serialization
builder.Services.Configure<WorkerOptions>(o =>
{
    o.Serializer = new JsonObjectSerializer(new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs =
        builder.Configuration.GetConnectionString("Default") ??
        builder.Configuration["ConnectionStrings:Default"] ??
        builder.Configuration["ConnectionStrings__Default"] ??
        Environment.GetEnvironmentVariable("ConnectionStrings__Default");

    if (string.IsNullOrWhiteSpace(cs))
        throw new InvalidOperationException(
            "Missing DB connection string. Set ConnectionStrings__Default in Azure Static Web App environment variables.");

    options.UseNpgsql(cs);
});

// Azure Functions web pipeline
builder.ConfigureFunctionsWebApplication();

var app = builder.Build();
app.Run();
