namespace EA.Weee.RequestHandlers.Admin
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class AddAatfRequestHandler : IRequestHandler<AddAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IMap<UKCompetentAuthorityData, UKCompetentAuthority> competentAuthorityMapper;
        private readonly IMap<AatfContactData, AatfContact> contactMapper;

        public AddAatfRequestHandler(
            IWeeeAuthorization authorization,
            WeeeContext context,
            IGenericDataAccess dataAccess,
            IMap<AatfAddressData, AatfAddress> addressMapper, 
            IMap<UKCompetentAuthorityData, UKCompetentAuthority> competentAuthorityMapper,
            IMap<AatfContactData, AatfContact> contactMapper)
        {
            this.authorization = authorization;
            this.context = context;
            this.addressMapper = addressMapper;
            this.competentAuthorityMapper = competentAuthorityMapper;
            this.contactMapper = contactMapper;
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(AddAatf message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            UKCompetentAuthority authority = competentAuthorityMapper.Map(message.Aatf.CompetentAuthority);
            AatfAddress siteAddress = addressMapper.Map(message.Aatf.SiteAddress);

            //Operator organisation = await context.Operators.FirstOrDefaultAsync(p => p.Organisation.Id == message.OrganisationId);

            Operator organisation = await dataAccess.GetById<Operator>(message.OrganisationId);

            AatfContact contact = contactMapper.Map(message.AatfContact);

            Aatf aatf = new Aatf(
                message.Aatf.Name,
                message.Aatf.CompetentAuthority.Id,
                message.Aatf.ApprovalNumber,
                Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(message.Aatf.AatfStatus.Value),
                organisation,
                siteAddress,
                Enumeration.FromValue<Domain.AatfReturn.AatfSize>(message.Aatf.Size.Value),
                message.Aatf.ApprovalDate.GetValueOrDefault(),
                contact);

            await dataAccess.Add<Aatf>(aatf);

            await context.SaveChangesAsync();

            return true;
        }
    }
}
