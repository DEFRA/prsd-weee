namespace EA.Weee.Sroc.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using Autofac;
    using global::Autofac;
    using Serilog;

    internal class Program
    {
        private static IContainer Container { get; set; }
       
        private const string RollbackCommand = "Rollback";
        private const string RunCommand = "Run";
        private const string ExitCommand = "Exit";

        private static void Main(string[] args)
        {
            Bootstrap();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs\\charge-migrator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                //args = new string[] { "Run" };

                if (args.Length != 1)
                {
                    Exit(0);
                }
                
                var command = args[0];

                CheckDatabaseConnection();

                var producerChargesUpdater = Container.Resolve<IUpdateProducerCharges>();
                switch (command)
                {
                    case RollbackCommand:
                        Log.Information("Begin rollback charges");
                        producerChargesUpdater.RollbackCharges();
                        Log.Information("End rollback charges");
                        break;
                        
                    case RunCommand:
                        Log.Information("Begin update charges");
                        producerChargesUpdater.UpdateCharges();
                        Log.Information("End rollback charges");
                        break;

                    case ExitCommand:
                        Exit(0);
                        break;

                    default:
                        Exit(0);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            Exit(0);
        }

        private static string GetVerbForKeySelection(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    return RollbackCommand;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return RunCommand;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    return ExitCommand;
                default:
                    return string.Empty;
            }
        }

        private static void CheckDatabaseConnection()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Weee.DefaultConnection"];

                if (connectionString == null)
                {
                    throw new ApplicationException("Connection string Weee.DefaultConnection not found. Exiting.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Exit(-1);
            }
        }

        private static void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        private static void Bootstrap()
        {
            var builder = new ContainerBuilder();
            // Register individual components            
            builder.RegisterModule(new MigrationRegistrationModule());

            Container = builder.Build();
        }
    }
}