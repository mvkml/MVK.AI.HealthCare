using Azure.Monitor.OpenTelemetry.Exporter;
using df_chunk_file.Models;
using df_chunk_file.Services;
using df_chunk_file.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.Configure<BulkImportOptions>(builder.Configuration.GetSection("BulkImport"));

builder.Services.AddHttpClient<IApiUploadClient, ApiUploadClient>();
builder.Services.AddScoped<IFileChunkingService, FileChunkingService>();
builder.Services.AddScoped<IImportResultAggregator, ImportResultAggregator>();
builder.Services.AddScoped<ITempFileCleaner, TempFileCleaner>();

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
