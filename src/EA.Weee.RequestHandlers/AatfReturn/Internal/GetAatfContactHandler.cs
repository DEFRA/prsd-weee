namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using Prsd.Core.Mediator;
    using Security;

    public class GetAatfContactHandler : IRequestHandler<GetAatfContact, AatfContactData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfContactDataAccess dataAccess;
        private readonly IMap<AatfContact, AatfContactData> mapper;

        public GetAatfContactHandler(IWeeeAuthorization authorization, IAatfContactDataAccess dataAccess, IMap<AatfContact, AatfContactData> mapper)
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

            return result;
        }
    }
}
