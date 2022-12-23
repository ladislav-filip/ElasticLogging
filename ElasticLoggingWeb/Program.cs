using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Information("Start application v4...");

builder.Host
    .UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder
    .Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, request) =>
    {
        Log.Debug("Headers -------------->>>>>>>>");
        foreach (var (key, value) in request.Headers)
        {
            Log.Debug("    {Key}: {Value}", key, value);
        }
        
        if (!request.Headers.ContainsKey("X-Forwarded-Host"))
        {
            return;
        }

        var hostPath = request.Headers["X-Forwarded-Host"];
        var basePath = builder.Configuration.GetValue<string>("BASE_PATH");
        var port =  builder.Configuration.GetValue<string>("REWRITE_PORT");
        if (!string.IsNullOrEmpty(port))
        {
            hostPath += $":{port}";
        }
        var serverUrl = $"{request.Scheme}://{hostPath}/{basePath}";
        Log.Information(serverUrl);
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new() { Url = serverUrl }
        };
    });
});
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();