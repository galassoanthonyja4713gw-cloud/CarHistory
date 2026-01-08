using Azure.Core.Serialization;
using CarHistory.Api.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

builder.Services.Configure<WorkerOptions>(o =>
{
    o.Serializer = new JsonObjectSerializer(new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    });
});

// Try to resolve the connection string from all common shapes.
// IMPORTANT: Do NOT throw here; let the host start so we can inspect runtime config in Azure.
string? cs =
    builder.Configuration.GetConnectionString("Default") ??
    builder.Configuration["ConnectionStrings:Default"] ??
    builder.Configuration["ConnectionStrings__Default"] ??
    Environment.GetEnvironmentVariable("ConnectionStrings__Default") ??
    Environment.GetEnvironmentVariable("ConnectionStrings_Default");

// Only register EF if we actually have a connection string.
if (!string.IsNullOrWhiteSpace(cs))
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(cs));
}

builder.ConfigureFunctionsWebApplication();

var app = builder.Build();
app.Run();
