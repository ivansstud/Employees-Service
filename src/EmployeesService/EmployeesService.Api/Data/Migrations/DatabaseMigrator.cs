using DbUp;
using DbUp.Engine;

namespace EmployeesService.Api.Data.Migrations;

public class DatabaseMigrator
{
	public static void UpgradeIfRequired(string connectionString)
	{
		EnsureDatabase.For.PostgresqlDatabase(connectionString);

		UpgradeEngine upgrader = DeployChanges.To.PostgresqlDatabase(connectionString)
			.WithScriptsEmbeddedInAssembly(typeof(DatabaseMigrator).Assembly)
			.LogToConsole()
			.Build();

		if (upgrader.IsUpgradeRequired())
		{
			upgrader.PerformUpgrade();
		}
	}
}
