using EmployeesService.Api.Data;
using EmployeesService.Api.Data.Migrations;
using EmployeesService.Api.Endpoints;
using EmployeesService.Api.Services;
using EmployeesService.Api.Validators;
using FluentValidation;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new Exception("Строка подключения к базе данных не установлена");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();

builder.AddCors("AllowAll")
	.AddUnitOfWork(connectionString)
	.AddApplicationServices();

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
}