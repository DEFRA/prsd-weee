namespace EA.Weee.RequestHandlers.Admin.GetAatfs
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Security;
    using Prsd.Core.Mapper;
    using System;
    using System.Threading.Tasks;

    public class GetAatfInfoByAatfIdRequestHandler : IRequestHandler<GetAatfById, AatfData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess aatfDataAccess;
        private readonly IMap<Aatf, AatfData> mapper;

        public GetAatfInfoByAatfIdRequestHandler(IWeeeAuthorization authorization, IMap<Aatf, AatfData> mapper, IGetAatfsDataAccess aatfDataAccess)
        {
            this.authorization = authorization;
            this.mapper = mapper;
            this.aatfDataAccess = aatfDataAccess;   
        }

        public async Task<AatfData> HandleAsync(GetAatfById message)
        {
            authorization.EnsureCanAccessInternalArea();

            var aatf = await aatfDataAccess.GetAatfById(message.AatfId);

            if (aatf == null)
            {
                throw new ArgumentException($"Could not find an aatf with Id {message.AatfId}");
            }

            var aatfMapped = mapper.Map(aatf);

            aatfMapped.CanEdit = authorization.CheckUserInRole(Roles.InternalAdmin);
            
            aatfMapped.Contact.CanEditContactDetails = authorization.CheckUserInRole(Roles.InternalAdmin);

            return aatfMapped;
        }
    }
}
