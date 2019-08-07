namespace EA.Weee.RequestHandlers.Aatf
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Aatf;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Aatf.GetAatfExternal;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Aatf;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetAatfByIdExternalHandler : IRequestHandler<GetAatfByIdExternal, AatfDataExternal>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext context;
        private readonly IGetAatfExternalDataAccess dataAccess;
        private readonly IMap<AatfContact, AatfContactData> mapper;

        public GetAatfByIdExternalHandler(IWeeeAuthorization authorization, IUserContext context, IMap<AatfContact, AatfContactData> mapper, IGetAatfExternalDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.context = context;
            this.mapper = mapper;
            this.dataAccess = dataAccess;
        }

        public async Task<AatfDataExternal> HandleAsync(GetAatfByIdExternal message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatf = await dataAccess.GetAatfById(message.AatfId);

            var aatfContactData = mapper.Map(aatf.Contact);

            var aatfData = new AatfDataExternal(aatf.Id, aatf.Name)
            {
                Contact = aatfContactData
            };

            return aatfData;
        }
    }
}
