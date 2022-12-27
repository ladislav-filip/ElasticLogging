using Microsoft.OpenApi.Models;
using Serilog;

namespace ElasticLoggingWeb.Infrastructure;

public static class WebApplicationExtensions
{
    public static WebApplication UseCustomSwagger(this WebApplication app, IConfiguration configuration)
    {
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
                var hostScheme = request.Headers["X-Forwarded-Scheme"];
                var basePath = configuration.GetValue<string>("BASE_PATH");
                var port =  configuration.GetValue<string>("REWRITE_PORT");
        
                if (!string.IsNullOrEmpty(port))
                {
                    hostPath += $":{port}";
                }
                var serverUrl = $"{hostScheme}://{hostPath}/{basePath}";
                Log.Information(serverUrl);
                swaggerDoc.Servers = new List<OpenApiServer>
                {
                    new() { Url = serverUrl }
                };
            });
        });

        return app;
    }    
}