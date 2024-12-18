namespace EA.Weee.Core.Configuration.Logging
{
    using Serilog;
    using Serilog.Events;
    using System.Configuration;

    public static class LoggerConfig
    {
        public static void ConfigureSerilogWithSqlServer(
            string connectionStringName = "Weee.DefaultConnection",
            string schemaName = "Logging",
            string tableName = "Logs")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException($"Connection string '{connectionStringName}' not found.");
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("IdentityServer3", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire", LogEventLevel.Error)
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    schemaName: schemaName,
                    tableName: tableName,
                    autoCreateSqlTable: true)
                .CreateLogger();
        }
    }
}
