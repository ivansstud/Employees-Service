using EmployeesService.Api.Data.Migrations;
using EmployeesService.Api.Data.Repositories;
using EmployeesService.Api.Endpoints;
using EmployeesService.Api.Models.Options;
using EmployeesService.Api.Services;
using EmployeesService.Api.Validators;
using FluentValidation;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new Exception("Строка подключения к базе данных не установлена");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();

builder.AddCors("AllowAll")
	.AddOpenTelemetry("Employees.Api")
	.AddSerilog()
	.AddUnitOfWork(connectionString)
	.AddApplicationServices()
	.AddCaching();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.MapEndpoints();

DatabaseMigrator.UpgradeIfRequired(connectionString);

app.Run();


internal static class Startup
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        EmployeesEndpoints.MapEndpoints(app);
        return app;
    }

    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder, string name)
    {
		builder.Services.AddCors(options =>
		{
			options.AddPolicy(name, policy =>
			{
				policy
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			});
		});
		return builder;
	}

	public static WebApplicationBuilder AddUnitOfWork(this WebApplicationBuilder builder, string connectionString)
	{
		builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));
		builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
		return builder;
	}

	public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IEmployeesService, EmployeesService.Api.Services.EmployeesService>();
		return builder;
	}

	public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder, string appName)
	{
		builder.Services
			.AddOpenTelemetry()
			.ConfigureResource(resource => resource.AddService(appName))
			.WithTracing(tracing =>
			{
				tracing
					.AddHttpClientInstrumentation()
					.AddAspNetCoreInstrumentation();

				tracing
					.AddOtlpExporter()
					.AddNpgsql();
			});
		return builder;
	}

	public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
	{
		builder.Host.UseSerilog((context, logConfig) =>
		{
			logConfig.ReadFrom.Configuration(context.Configuration);
		});
		return builder;
	}

	public static WebApplicationBuilder AddCaching(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<EmployeesCachingOptions>(builder.Configuration.GetSection(EmployeesCachingOptions.Section));

		builder.Services.AddStackExchangeRedisCache(options =>
		{
			options.Configuration = builder.Configuration.GetConnectionString("Redis");
		});

		builder.Services.AddScoped<IRedisCachingService, RedisCachingService>();
		builder.Services.AddScoped<IEmployeesCacheService, EmployeesCacheService>();

		return builder;
	}
}