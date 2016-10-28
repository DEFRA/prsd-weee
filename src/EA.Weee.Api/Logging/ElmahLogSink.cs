namespace EA.Weee.Api.Logging
{
    using Elmah;
    using Serilog.Core;
    using Serilog.Events;

    internal class ElmahLogSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            ErrorSignal.FromCurrentContext().Raise(logEvent.Exception);
        }
    }
}