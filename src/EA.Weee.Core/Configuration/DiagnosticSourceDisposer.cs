namespace EA.Weee.Core.Configuration
{
    using System;
    using System.Reflection;

    public static class DiagnosticSourceDisposer
    {
        private const string DiagnosticAssemblyName = "System.Diagnostics.DiagnosticSource, Version=8.0.0.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51";
        private const string DiagnosticSourceEventSourceTypeName = "System.Diagnostics.DiagnosticSourceEventSource";
        private const string LogFieldName = "Log";

        public static void DisposeDiagnosticSourceEventSource()
        {
            try
            {
                var diagnosticSourceEventSource = GetDiagnosticSourceEventSource();
                DisposeSafely(diagnosticSourceEventSource);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                Console.WriteLine($"Error disposing DiagnosticSourceEventSource: {ex.Message}");
            }
        }

        private static object GetDiagnosticSourceEventSource()
        {
            var diagnosticsLib = Assembly.Load(DiagnosticAssemblyName);
            var diagnosticSourceEventSourceType = diagnosticsLib.GetType(DiagnosticSourceEventSourceTypeName);
            return diagnosticSourceEventSourceType.GetField(LogFieldName, BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
        }

        private static void DisposeSafely(object obj)
        {
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
