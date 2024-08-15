namespace EA.Weee.Api.Infrastructure.Serilog
{
    using global::Serilog;
    using System.Configuration;

    public static class LoggerConfig
    {
        public static void ConfigureSerilogWithSqlServer(
            string connectionStringName = "Weee.DefaultConnection",
            string schemaName = "Logging",
            string tableName = "Logs",
            bool autoCreateSqlTable = true)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException($"Connection string '{connectionStringName}' not found.");
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        SchemaName = schemaName,
                        TableName = tableName,
                        AutoCreateSqlTable = autoCreateSqlTable
                    })
                .CreateLogger();

            Log.Information("Serilog configuration completed.");
        }
    }
}
