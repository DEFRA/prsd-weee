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
            var src = GetParent("src");

            if (src != null)
            {
                LaunchCommandLineApp(src);
            }
        }

        private void LaunchCommandLineApp(FileSystemInfo src)
        {
            var aliaSql = Directory.GetFiles($"{src.FullName}\\packages", "AliaSQL.exe", SearchOption.AllDirectories);
            var scriptsDirectory = $"{src.FullName}\\EA.Weee.Database\\scripts\\";

            if (aliaSql.Length > 0)
            {
                Console.WriteLine($"Using AliaSQL from {aliaSql[0]}");

                var arguments =
                    $@"Rebuild .\sqlexpress EA.Weee.Integration {scriptsDirectory} {ConfigurationManager.AppSettings["aliSqlConnectionUser"]} {ConfigurationManager.AppSettings["aliSqlConnectionPassword"]}";

                Console.Write($"AliaSql command arguments {arguments}");

                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = aliaSql[0],
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    Arguments = $@"Rebuild .\sqlexpress EA.Weee.Integration {scriptsDirectory} {ConfigurationManager.AppSettings["aliSqlConnectionUser"]} {ConfigurationManager.AppSettings["aliSqlConnectionPassword"]}"
                };

                try
                {
                    var cscProcess = Process.Start(startInfo);

                    if (cscProcess == null)
                    {
                        throw new Exception("Could not start AliaSql");
                    }

                    var output = cscProcess.StandardOutput.ReadToEnd();

                    if (!output.Contains("usdDatabaseVersion"))
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
