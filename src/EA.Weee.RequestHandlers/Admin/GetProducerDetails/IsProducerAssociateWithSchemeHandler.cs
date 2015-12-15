namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Security;

    public class IsProducerAssociateWithSchemeHandler : IRequestHandler<Requests.Admin.IsProducerAssociateWithScheme, bool>
    {
        private readonly IWeeeAuthorization authorization;

        public IsProducerAssociateWithSchemeHandler(IWeeeAuthorization authorization)
        {
            this.authorization = authorization;
        }

        public async Task<bool> HandleAsync(Requests.Admin.IsProducerAssociateWithScheme request)
        {
            authorization.EnsureCanAccessInternalArea();
            // TODO: The actual implementation

            return await Task.Run(() => true);
        }
    }
}
