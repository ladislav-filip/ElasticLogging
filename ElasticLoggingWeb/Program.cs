using ElasticLoggingWeb.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Information("Start application v10...");

builder.Host
    .UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwaggerGen();
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddProcessAllocatedMemoryHealthCheck(maximumMegabytesAllocated: 50, tags: new[] { "memory" }, name: "Process alocated memory")
    .AddElasticsearch(cfg =>
    {
        var url = builder.Configuration["Serilog:WriteTo:0:Args:nodeUris"];
        Log.Information("Elasticsearch URL address is '{Url}'", url);
        cfg.UseServer(url);
    }, name: "Elasticsearch")
    ;

// https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
builder.Services
    .AddHealthChecksUI(cfg =>
    {
        var healthPath = builder.Configuration.GetValue<string>("HEALTH_URL")!;
        cfg.AddHealthCheckEndpoint("main", healthPath);
        cfg.MaximumHistoryEntriesPerEndpoint(50);
    })
    .AddInMemoryStorage();

var app = builder
    .Build();

app.UseCustomSwagger(builder.Configuration);

app.UseSwaggerUI();

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

// app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app
    .UseRouting()
    .UseEndpoints(cfg =>
    {
        cfg.MapControllers();
        cfg.MapHealthChecksUI(setup =>
        {
            setup.AddCustomStylesheet("wwwroot/health.css");
        });
    });

app.Run();