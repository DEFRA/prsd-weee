namespace EA.Weee.Api.Services
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.ExceptionHandling;

    internal class DebugExceptionLogger : IExceptionLogger
    {
        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            Debug.WriteLine(context.Exception.ToString());
            return Task.FromResult(0);
        }
    }
}