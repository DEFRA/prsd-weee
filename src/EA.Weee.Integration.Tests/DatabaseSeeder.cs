namespace EA.Weee.Integration.Tests
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;

    public class DatabaseSeeder
    {
        public void RebuildDatabase()
        {
            LaunchCommandLineApp("Rebuild");
        }

        public void UpdateDatabase()
        {
            LaunchCommandLineApp("Update");
        }

        private void LaunchCommandLineApp(string command)
        {
            var aliaScriptsDir = ConfigurationManager.AppSettings["aliaSqlScriptsDir"];
            string aliaExe = string.Empty;
            if (string.IsNullOrWhiteSpace(aliaScriptsDir))
            {
                var src = GetParent("src");
                var aliaSql = Directory.GetFiles($@"{src.FullName}", "AliaSQL.exe", SearchOption.AllDirectories);

                if (aliaSql.Length > 0)
                {
                    aliaExe = aliaSql[0];
                }

                aliaScriptsDir = $@"{src.FullName}\EA.Weee.Database\scripts\";
            }
            else
            {
                aliaExe = $@"{aliaScriptsDir}\AliaSQL.exe";
            }

            Console.WriteLine($"Using AliaSQL from {aliaExe}");

            if (!string.IsNullOrWhiteSpace(aliaExe))
            {
                var arguments =
                    $@"Rebuild {ConfigurationManager.AppSettings["aliaSqlConnectionServer"]} {ConfigurationManager.AppSettings["aliaSqlConnectionDatabase"]} {aliaScriptsDir} {ConfigurationManager.AppSettings["aliaSqlConnectionUser"]} {ConfigurationManager.AppSettings["aliaSqlConnectionPassword"]}";

                Console.Write($"AliaSql command arguments {arguments}");

                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = aliaExe,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    Arguments = arguments
                };

                try
                {
                    var cscProcess = Process.Start(startInfo);

                    if (cscProcess == null)
                    {
                        throw new Exception("Could not start AliaSql");
                    }

                    var output = cscProcess.StandardOutput.ReadToEnd();

                    if (!output.Contains("usdDatabaseVersion") || output.Contains("CREATE DATABASE permission denied in database"))
                    {
                        throw new Exception(output);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to rebuild WEEE.Integration database {ex.Message}");
                    throw;
                }
            }
        }

        private DirectoryInfo GetParent(string parentName)
        {
            Console.WriteLine($"looking for parent {parentName}");
            var dir = new DirectoryInfo(GetType().Assembly.Location);

            if (dir?.Parent == null)
            {
                throw new ArgumentNullException(nameof(parentName));
            }

            while (dir.Parent != null && !dir.Parent.Name.Equals(parentName, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"found parent {dir.Parent.Name}");
                dir = dir.Parent;
            }

            if (dir.Parent?.Parent == null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            return dir.Parent;
        }
    }
}
