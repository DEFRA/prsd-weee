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

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Organisation Migration Runner";

            var p = new Process();
            var keySelection = string.Empty;
            string input;

            Console.WriteLine(" Enter filepath:");
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
                            DatabaseValidation(organisations);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    DrawMenu();
                }

                var key = Console.ReadKey(true);
                while (!char.IsDigit(key.KeyChar))
                {
                    Console.WriteLine(" Input is not valid. Please try again", input);
                }

                keySelection = key.KeyChar.ToString();
            }
        }

        private static void DrawMenu()
        {
            Console.Clear();

            Console.WriteLine(" Database: ");
            Console.WriteLine(" Server: ");
            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" 1. Validate local file");
            Console.WriteLine(" 2. Validate database connection");
            Console.WriteLine(" 3. Run data migration");
        }

        private static void Exit(int exitCode)
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Environment.Exit(exitCode);
        }

        private static string GetVerbForKeySelection(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    return "Update";
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return "Create";
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    return "Rebuild";
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    return "TestData";
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    return "Baseline";
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    return "Exit";
                default:
                    return string.Empty;
            }
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

                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                }

                Console.WriteLine("Connection successful.");
            }
            catch
            {
                Console.WriteLine("Connection failed. Exiting.");
                Exit(-1);
            }
        }

        private static void DatabaseValidation(IList<OrganisationData> shipments)
        {
            Console.WriteLine(" DB is valid");
        }
    }
}
