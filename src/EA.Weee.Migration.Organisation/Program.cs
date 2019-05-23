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
    using System.Text;
    using System.Threading.Tasks;
    using Dapper;

    internal class Program
    {
        private static readonly string insertOrganisationSql = @"INSERT INTO Organisation.Organisation (Name, OrganisationType, OrganisationStatus, TradingName, CompanyRegistrationNumber, BusinessAddressId) VALUES (@Name, @OrganisationType, @OrganisationStatus, @TradingName, @CompanyRegistrationNumber, @BusinessAddressId)";
        private static readonly string insertAddressSql = @"DECLARE @InsertedRow AS TABLE (Id uniqueidentifier);
                                                            INSERT INTO Organisation.Address (Address1, Address2, TownOrCity, CountyOrRegion, Postcode, CountryId, Telephone, Email) OUTPUT Inserted.Id INTO @InsertedRows
                                                            VALUES (Address1, @Address2, @TownOrCity, @CountyOrRegion, @Postcode, @CountryId, @Telephone, @Email);
                                                            SELECT Id from @InsertedRows";
        private static readonly string queryCountrySql = @"SELECT Id FROM Lookup.Country WHERE Name = @CountryName";

        private static readonly string DatabaseName = GetDatabaseName();
        private static readonly string DbServer = GetDatabaseServer();

        private static void Main(string[] args)
        {
            Console.Title = "Organisation Migration Runner";

            var p = new Process();
            var keySelection = string.Empty;
            string input;

            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" Enter .xlsx filepath:");
            input = Console.ReadLine().Replace("\"", "");

            while (!File.Exists(input))
            {
                Console.WriteLine(" Could not find file {0}. Please try again", input);
                input = Console.ReadLine().Replace("\"", "");
            }

            IList<OrganisationData> organisations;

            if (!XlsxOrganisationDataReader.TryGetOrganisationData(input, out organisations))
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
                    DrawMenu(organisations);
                }

                var key = Console.ReadKey(true);
                while (!char.IsDigit(key.KeyChar))
                {
                    Console.WriteLine(" Input is not valid. Please try again", input);
                }

                keySelection = key.KeyChar.ToString();
            }
        }

        private static void DrawMenu(IList<OrganisationData> organisations)
        {
            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" Database: " + DatabaseName);
            Console.WriteLine(" Server: " + DbServer);
            Console.WriteLine(" Found {0} organisations", organisations.Count);
            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" 1. Validate data");
            Console.WriteLine(" 2. Check database connection");
            Console.WriteLine(" 3. Run data migration");
        }

        private static void Exit(int exitCode)
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Environment.Exit(exitCode);
        }

        private static void LocalValidation(IList<OrganisationData> organisations)
        {
            Console.WriteLine(" File is valid");
        }

        private static void CheckDatabaseConnection()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"];

                if (connectionString == null)
                {
                    Console.WriteLine("Connection string Weee.DefaultConnection not found. Exiting.");
                    Exit(-1);
                }

                var builder = new SqlConnectionStringBuilder(connectionString.ConnectionString);

                Console.WriteLine("Trying connection to database {0} on server {1}.", builder.InitialCatalog, builder.DataSource);

                using (var sqlConnection = new SqlConnection(builder.ConnectionString))
                {
                    sqlConnection.Open();
                    sqlConnection.Close();
                }

                Console.WriteLine("Connection successful.");
            }
            catch
            {
                Console.WriteLine("Connection failed. Exiting.");
                Exit(-1);
            }
        }

        private static void RunDatabaseMigration(IList<OrganisationData> organisations)
        {
            var builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"].ConnectionString);

            using (var sqlConnection = new SqlConnection(builder.ConnectionString))
            {
                sqlConnection.Open();
                foreach (var organisation in organisations)
                {
                    organisation.AddressData.CountryId = sqlConnection.Query<Guid>(queryCountrySql,
                        new { CountryName = organisation.AddressData.CountryName }).First();
                    var addressId = sqlConnection.Execute(insertAddressSql,
                        new
                        {
                            Address1 = organisation.AddressData.Address1,
                            Address2 = organisation.AddressData.Address2,
                            TownOrCity = organisation.AddressData.TownOrCity,
                            CountyOrRegion = organisation.AddressData.CountyOrRegion,
                            Postcode = organisation.AddressData.Postcode,
                            CountryId = organisation.AddressData.CountryId,
                            Telephone = organisation.AddressData.Telephone,
                            Email = organisation.AddressData.Email
                        });

                    sqlConnection.Execute(insertOrganisationSql,
                        new
                        {
                            Name = organisation.Name,
                            OrganisationType = organisation.OrganisationType.Value,
                            OrganisationStatus = 1,
                            TradingName = organisation.TradingName,
                            CompanyRegistrationNumbe = organisation.RegistrationNumber,
                            BusinessAddressId = addressId,
                        });
                }
                sqlConnection.Close();
            }

            Console.WriteLine("Connection successful.");
        }

        private static string GetDatabaseName()
        {
            var databasename = ConfigurationManager.AppSettings["DatabaseName"];

            if (string.IsNullOrEmpty(databasename))
            {
                var projectname = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

                databasename = projectname.Replace("Database.", String.Empty).Replace(".Database", String.Empty).Replace("Database", String.Empty);
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