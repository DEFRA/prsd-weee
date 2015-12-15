namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Security;

    public class IsProducerAssociateWithAnotherSchemeHandler : IRequestHandler<Requests.Admin.IsProducerAssociateWithAnotherScheme, bool>
    {
        private readonly IWeeeAuthorization authorization;

        public IsProducerAssociateWithAnotherSchemeHandler(IWeeeAuthorization authorization)
        {
            this.authorization = authorization;
        }

        public async Task<bool> HandleAsync(Requests.Admin.IsProducerAssociateWithAnotherScheme request)
        {
            authorization.EnsureCanAccessInternalArea();
            // TODO: The actual implementation

            return await Task.Run(() => true);
        }
    }
}
