namespace EA.Weee.Core.Configuration.Logging
{
    using Serilog;
    using Serilog.Sinks.MSSqlServer;
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
                .MinimumLevel.Debug()
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        SchemaName = schemaName,
                        TableName = tableName,
                        AutoCreateSqlTable = true
                    })
                .CreateLogger();
        }
    }
}
