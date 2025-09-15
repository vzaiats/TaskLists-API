using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using TaskListsAPI.Api.Extensions;
using TaskListsAPI.Api.Middleware;
using TaskListsAPI.Application.Interfaces;
using TaskListsAPI.Application.Services;
using TaskListsAPI.Infrastructure.Database;
using TaskListsAPI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add controllers
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Add headers "api-supported-versions"
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV"; // v1, v1.1
        options.SubstituteApiVersionInUrl = true;
    });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // XML file from Application project
    var appXmlPath = Path.Combine(AppContext.BaseDirectory, "TaskListsAPI.Application.xml");
    if (File.Exists(appXmlPath))
        c.IncludeXmlComments(appXmlPath);

    // Add examples
    c.ExampleFilters();
});

// Add Swagger examples
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
// Add Swagger API with health check
builder.Services.AddAppHealthChecks(builder.Configuration);
builder.Services.AddSwaggerWithHealth();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add repositories
builder.Services.AddScoped<ITaskCollectionRepository, TaskCollectionRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();

// Add services
builder.Services.AddScoped<ITaskCollectionService, TaskCollectionService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Apply migrations
    await db.Database.MigrateAsync();
    // Seed data
    await SeedData.ApplyAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // Swagger UI configuration
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskListsAPI v1");
        c.RoutePrefix = "swagger"; // Swagger available at /swagger/index.html
    });

    app.UseSwaggerWithHealth();
}

app.UseHttpsRedirection();
app.UseAuthorization(); // Use authorization
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapAppHealthChecks();

app.Run();