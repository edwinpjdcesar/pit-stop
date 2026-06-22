using Api.Middleware;
using Core;
using Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var openApiServerUrl = builder.Configuration["OpenApi:ServerUrl"];

builder.Services.AddOpenApi(options =>
{
    if (!string.IsNullOrEmpty(openApiServerUrl))
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Servers = [new Microsoft.OpenApi.OpenApiServer { Url = openApiServerUrl }];
            return Task.CompletedTask;
        });
    }
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDataServices(connectionString);
builder.Services.AddCoreServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PitStopContext>();
    await db.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "PitStop API";
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
