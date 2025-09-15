using Microsoft.OpenApi.Models;

namespace TaskListsAPI.Api.Extensions
{
    public static class SwaggerExtensions
    {
        #region Methods

        // Configure Swagger with links to health endpoints
        public static IServiceCollection AddSwaggerWithHealth(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TaskLists API",
                    Version = "v1",
                    Description = @"
### TaskLists API
REST API for managing task collections and tasks.

#### Useful links:
- [Health endpoint](/health)
- [Health UI dashboard](/health-ui)
"
                });
            });

            return services;
        }

        // Enables Swagger middleware with UI
        public static IApplicationBuilder UseSwaggerWithHealth(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskLists API v1");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }

        #endregion
    }
}