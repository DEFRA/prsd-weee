namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Security;

    public class IsProducerAssociatedWithAnotherSchemeHandler : IRequestHandler<Requests.Admin.IsProducerAssociatedWithAnotherScheme, bool>
    {
        private readonly IWeeeAuthorization authorization;

        public IsProducerAssociatedWithAnotherSchemeHandler(IWeeeAuthorization authorization)
        {
            this.authorization = authorization;
        }

        public async Task<bool> HandleAsync(Requests.Admin.IsProducerAssociatedWithAnotherScheme request)
        {
            authorization.EnsureCanAccessInternalArea();
            // TODO: The actual implementation

            return await Task.Run(() => false);
        }
    }
}
