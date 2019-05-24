namespace EA.Weee.Migration.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Dapper;
    using Model;
    using NLog;
    using Validation;

    internal class Program
    {
        private static readonly string insertOrganisationSql =
            @"INSERT INTO Organisation.Organisation (Id, Name, OrganisationType, OrganisationStatus, TradingName, CompanyRegistrationNumber, BusinessAddressId)
                                                                 VALUES (NEWID(), @Name, @OrganisationType, @OrganisationStatus, @TradingName, @CompanyRegistrationNumber, @BusinessAddressId)";

        private static readonly string insertAddressSql = @"DECLARE @InsertedRows AS TABLE (Id uniqueidentifier);
                                                            INSERT INTO Organisation.Address (Id, Address1, Address2, TownOrCity, CountyOrRegion, Postcode, CountryId, Telephone, Email) OUTPUT Inserted.Id INTO @InsertedRows
                                                            VALUES (NEWID(), @Address1, @Address2, @TownOrCity, @CountyOrRegion, @Postcode, @CountryId, @Telephone, @Email);
                                                            SELECT Id from @InsertedRows";

        private static readonly string queryCountrySql = @"SELECT * FROM Lookup.Country WHERE Name = @CountryName;";

        private static readonly string checkExistingOrganisationSql =
            @"SELECT * FROM Organisation.Organisation WHERE Name = @Name;";

        private static readonly string DatabaseName = GetDatabaseName();
        private static readonly string DbServer = GetDatabaseServer();

        private static void Main(string[] args)
        {
            var logger = SetupLogger();

            Console.Title = "Organisation Migration Runner";

            var p = new Process();
            var keySelection = string.Empty;
            string fileName;

            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" Enter .xlsx filepath:");
            fileName = Console.ReadLine().Replace("\"", "");

            while (!File.Exists(fileName))
            {
                Console.WriteLine(" Could not find file {0}. Please try again", fileName);
                fileName = Console.ReadLine().Replace("\"", "");
            }

            IList<Organisation> organisations;

            if (!XlsxOrganisationDataReader.TryGetOrganisationData(fileName, out organisations))
            {
                Console.WriteLine(" Errors reading spreadsheet.  Exiting.");
                Exit(-1);
            }

            while (string.Compare(keySelection, "Exit", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                if (!string.IsNullOrEmpty(keySelection))
                {
                    switch (keySelection)
                    {
                        case ("1"):
                            LocalValidation(organisations);
                            break;
                        case ("2"):
                            CheckDatabaseConnection();
                            LocalValidation(organisations);
                            break;
                        case ("3"):
                            CheckDatabaseConnection();
                            LocalValidation(organisations);
                            RunDatabaseMigration(organisations);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    DrawMenu(organisations, fileName);
                }

                var key = Console.ReadKey(true);
                while (!char.IsDigit(key.KeyChar))
                {
                    Console.WriteLine(" Input is not valid. Please try again", fileName);
                }

                keySelection = key.KeyChar.ToString();
            }
        }

        private static Logger SetupLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            return logger;
        }

        private static void DrawMenu(IList<Organisation> organisations, string fileName)
        {
            Console.Clear();
            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" Database: {0}", DatabaseName);
            Console.WriteLine(" Server: {0}", DbServer);
            Console.WriteLine(" File: {0}", fileName);
            Console.WriteLine(" Found {0} organisations", organisations.Count);
            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" 1. Validate datafile");
            Console.WriteLine(" 2. Check database connection");
            Console.WriteLine(" 3. Run data migration");
        }

        private static void Exit(int exitCode)
        {
            Console.WriteLine(" Press any key to continue...");
            Console.ReadKey();
            Environment.Exit(exitCode);
        }

        private static void LocalValidation(IList<Organisation> organisations)
        {
            Console.WriteLine(Environment.NewLine);
            if (OrganisationDataListValidator.HasErrors(organisations))
            {
                Console.WriteLine(" Found errors in organisation data.  Exiting");
                Exit(-1);
            }

            Console.WriteLine(" No validation errors found");
        }

        private static void CheckDatabaseConnection()
        {
            Console.WriteLine(Environment.NewLine);
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"];

                if (connectionString == null)
                {
                    Console.WriteLine(" Connection string Weee.DefaultConnection not found. Exiting.");
                    Exit(-1);
                }

                var builder = new SqlConnectionStringBuilder(connectionString.ConnectionString);

                Console.WriteLine(" Trying connection to database {0} on server {1}.", builder.InitialCatalog,
                    builder.DataSource);

                using (var sqlConnection = new SqlConnection(builder.ConnectionString))
                {
                    sqlConnection.Open();
                    sqlConnection.Close();
                }

                Console.WriteLine(" Connection successful.");
            }
            catch
            {
                Console.WriteLine(" Connection failed. Exiting.");
                Exit(-1);
            }
        }

        private static void RunDatabaseMigration(IList<Organisation> organisations)
        {
            Console.WriteLine(Environment.NewLine);
            using (var sqlConnection = DatabaseHelper.CreateConnection())
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction()) {
                    foreach (var organisation in organisations)
                    {
                        var existingOrganisation = sqlConnection
                            .Query<object>(checkExistingOrganisationSql, new { Name = organisation.Name }, transaction: transaction)
                            .FirstOrDefault();

                        if (existingOrganisation != null)
                        {
                            Console.WriteLine(" Organisation {0} already exists", organisation.Name);
                            continue;
                        }

                        var country = sqlConnection.Query<Country>(queryCountrySql,
                            new { CountryName = organisation.AddressData.CountryName }, transaction: transaction).FirstOrDefault();

                        if (country == null)
                        {
                            Console.WriteLine(
                                " Could not find a match for database entry for country {0} for organisation {1}. Exiting.",
                                organisation.AddressData.CountryName, organisation.Name);
                            Exit(-1);
                        }

                        var addressId = sqlConnection.Query<Guid>(insertAddressSql,
                            new
                            {
                                Address1 = organisation.AddressData.Address1,
                                Address2 = organisation.AddressData.Address2,
                                TownOrCity = organisation.AddressData.TownOrCity,
                                CountyOrRegion = organisation.AddressData.CountyOrRegion,
                                Postcode = organisation.AddressData.Postcode,
                                CountryId = country.Id,
                                Telephone = organisation.AddressData.Telephone,
                                Email = organisation.AddressData.Email
                            }, transaction: transaction).FirstOrDefault();

                        if (addressId == null)
                        {
                            Console.WriteLine(" Could not save address for '{0}'. Exiting.", organisation.Name);
                            Exit(-1);
                        }

                        sqlConnection.Execute(insertOrganisationSql,
                            new
                            {
                                Name = organisation.Name,
                                OrganisationType = organisation.OrganisationType.Value,
                                OrganisationStatus = 1,
                                TradingName = organisation.TradingName,
                                CompanyRegistrationNumber = organisation.RegistrationNumber,
                                BusinessAddressId = addressId,
                            }, transaction: transaction);
                        Console.WriteLine(" Successfully saved data for {0}", organisation.Name);
                    }

                    transaction.Commit();
                }

                sqlConnection.Close();
            }

            Console.WriteLine(" Successfully imported data");
            Exit(0);
        }

        private static string GetDatabaseName()
        {
            var databasename = ConfigurationManager.AppSettings["DatabaseName"];

            if (string.IsNullOrEmpty(databasename))
            {
                var projectname = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

                databasename = projectname.Replace("Database.", String.Empty).Replace(".Database", String.Empty)
                    .Replace("Database", String.Empty);
            }

            return databasename;
        }

        private static string GetDatabaseServer()
        {
            var servername = ConfigurationManager.AppSettings["DatabaseServer"];

            if (string.IsNullOrEmpty(servername))
            {
                servername = ".\\sqlexpress";
            }

            return servername;
        }
    }
}