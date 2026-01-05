using Application;
using FastFood;
using Infraestructure;
using Infraestructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    var connectionString = context.Configuration.GetConnectionString("DBConnectionStringLogs");

    config.MinimumLevel.Information();

    config.Enrich.FromLogContext();
    config.Enrich.WithProperty("Application", "SIGMulher");
    config.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

    config.WriteTo
        .PostgreSQL(connectionString, "Logs", needAutoCreateTable: true);

    if (!context.HostingEnvironment.IsProduction())
    {
        config.WriteTo.Console();
    }

    config.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
});


ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

IServiceCollection services = builder.Services;

services.AddInfraestructure(configuration);
services.AddFastFood(configuration);
services.AddApplication(configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
    dataContext.Database.Migrate();
}

// Configure the HTTP request pipeline
app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors("AllowCors");

// app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
