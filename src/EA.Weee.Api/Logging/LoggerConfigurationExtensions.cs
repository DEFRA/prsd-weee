namespace EA.Weee.Api.Logging
{
    using System;
    using Prsd.Core;
    using Serilog;
    using Serilog.Configuration;
    using Serilog.Formatting.Display;

    internal static class LoggerConfigurationExtensions
    {
        private const string DefaultOutputTemplate = "{Timestamp} [{Level}] {Message}{NewLine}{Exception}";

        public static LoggerConfiguration Debug(this LoggerSinkConfiguration loggerConfiguration,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null)
        {
            Guard.ArgumentNotNull(() => loggerConfiguration, loggerConfiguration);

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return loggerConfiguration.Sink(new DebugLogSink(formatter));
        }

        public static LoggerConfiguration Elmah(this LoggerSinkConfiguration loggerConfiguration)
        {
            Guard.ArgumentNotNull(() => loggerConfiguration, loggerConfiguration);

            return loggerConfiguration.Sink(new ElmahLogSink());
        }
    }
}