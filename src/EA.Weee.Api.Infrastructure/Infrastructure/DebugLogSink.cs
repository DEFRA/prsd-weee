namespace EA.Weee.Api.Infrastructure.Infrastructure
{
    using System.Diagnostics;
    using System.IO;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Formatting.Display;

    public class DebugLogSink : ILogEventSink
    {
        private readonly MessageTemplateTextFormatter formatter;

        public DebugLogSink(MessageTemplateTextFormatter formatter)
        {
            this.formatter = formatter;
        }

        public void Emit(LogEvent logEvent)
        {
            var sr = new StringWriter();
            formatter.Format(logEvent, sr);
            var text = sr.ToString().Trim();

            Debug.WriteLine(text);
        }
    }
}