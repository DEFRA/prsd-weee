namespace EA.Weee.Api.Infrastructure.Infrastructure
{
    using Elmah;
    using Serilog.Core;
    using Serilog.Events;

    public class ElmahLogSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            ErrorSignal.FromCurrentContext().Raise(logEvent.Exception);
        }
    }
}