namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Weee.Security;

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
