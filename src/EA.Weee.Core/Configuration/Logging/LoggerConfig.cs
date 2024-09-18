namespace EA.Weee.Core.Configuration.Logging
{
    using Serilog;
    using Serilog.Sinks.MSSqlServer;
    using System.Configuration;
    using System.Web;

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
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    schemaName: schemaName,
                    tableName: tableName,
                    autoCreateSqlTable: true)
                .CreateLogger();

            string appDataPath = HttpContext.Current.Server.MapPath("~/App_Data/");
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                System.IO.File.AppendAllText($"{appDataPath}serilog-self-log.txt", msg);  // Log Serilog internal errors
            });
        }
    }
}
