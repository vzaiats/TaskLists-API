using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TaskListsAPI.Api.Extensions
{
    public static class HealthCheckExtensions
    {
        #region Methods

        // Add application health checks and HealthChecks UI dashboard
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            // Add PostgreSQL health check
            services.AddHealthChecks()
                .AddNpgSql(
                    configuration.GetConnectionString("DefaultConnection")!,
                    name: "PostgreSQL",
                    failureStatus: HealthStatus.Unhealthy);

            // Add HealthChecks UI (web dashboard)
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15); // Check every 15 seconds
                opt.MaximumHistoryEntriesPerEndpoint(60); // Keep history entries
                opt.AddHealthCheckEndpoint("API with DB", "/health"); // Monitoring endpoint
            })
            .AddInMemoryStorage(); // Use in-memory storage for UI

            return services;
        }

        // Map health check endpoints (/health and /health-ui)
        public static IEndpointRouteBuilder MapAppHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            // JSON API for health status
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Web dashboard for health monitoring
            endpoints.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui"; // UI available at /health-ui
            });

            return endpoints;
        }

        #endregion
    }
}
