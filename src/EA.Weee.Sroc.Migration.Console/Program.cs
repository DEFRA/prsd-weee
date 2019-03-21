namespace EA.Weee.Sroc.Migration.Console
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Autofac;
    
    public class Program
    {
        private static void Main()
        {
            Console.Title = "2019 charge calculations";

            try
            {
                var keySelection = string.Empty;
                var p = new Process();
                var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
                while (string.Compare(keySelection, "Exit", StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    if (!string.IsNullOrEmpty(keySelection))
                    {
                        System.Console.WriteLine();

                        Console.WriteLine("Processing command");
                        var cmdArguments = string.Format("{0}", keySelection);
                        p.StartInfo.FileName = string.Format(@"{0}\chargesMigrator.exe", currentDirectory);
                        p.StartInfo.Arguments = cmdArguments;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.Start();

                        Console.WriteLine(p.StandardOutput.ReadToEnd());
                        Console.WriteLine("Finishing command");

                        System.Console.WriteLine("Press Any Key to Continue");
                    }
                    else
                    {
                        DrawMenu();
                    }

                    var key = Console.ReadKey(true);
                    keySelection = GetVerbForKeySelection(key);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty));
            }
        }

        private static void DrawMenu()
        {
            Console.Clear();

            Console.WriteLine(" ----------------------------------------------------------------------------");
            Console.WriteLine(" 1. Rollback 2019 calculated charges");
            Console.WriteLine(" 2. Re-calculate 2019 calculated charges");
            Console.WriteLine(" 3. Exit program");
        }

        private static string GetVerbForKeySelection(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    return "Rollback";
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return "Run";
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    return "Exit";
                default:
                    return string.Empty;
            }
        }
    }
}
