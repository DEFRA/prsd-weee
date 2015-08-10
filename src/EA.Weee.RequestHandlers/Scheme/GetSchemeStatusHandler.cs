namespace EA.Weee.RequestHandlers.Scheme
{
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    public class GetSchemeStatusHandler : IRequestHandler<GetSchemeStatus, SchemeStatus>
    {
        private readonly WeeeContext context;

        public GetSchemeStatusHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<SchemeStatus> HandleAsync(GetSchemeStatus message)
        {
            // TODO: Implement
            return await Task.Run(() => SchemeStatus.Pending);
        }
    }
}
