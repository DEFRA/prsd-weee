namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Security;

    public class GetAatfContactHandler : IRequestHandler<GetAatfContact, AatfContactData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess dataAccess;
        private readonly IMap<AatfContact, AatfContactData> mapper;

        public GetAatfContactHandler(IWeeeAuthorization authorization, IAatfDataAccess dataAccess, IMap<AatfContact, AatfContactData> mapper)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.mapper = mapper;
        }

        public async Task<AatfContactData> HandleAsync(GetAatfContact message)
        {
            authorization.EnsureCanAccessInternalArea();

            var contact = await dataAccess.GetContact(message.AatfId);

            var result = mapper.Map(contact);

            result.CanEditContactDetails = authorization.CheckUserInRole(Roles.InternalAdmin);

            return result;
        }
    }
}
