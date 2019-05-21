namespace EA.Weee.RequestHandlers.Admin
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using System.Linq;
    using System.Threading.Tasks;

    public class AddAatfRequestHandler : IRequestHandler<AddAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IMap<AatfContactData, AatfContact> contactMapper;

        public AddAatfRequestHandler(
            IWeeeAuthorization authorization,
            IGenericDataAccess dataAccess,
            IMap<AatfAddressData, AatfAddress> addressMapper, 
            IMap<AatfContactData, AatfContact> contactMapper)
        {
            this.authorization = authorization;
            this.addressMapper = addressMapper;
            this.contactMapper = contactMapper;
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(AddAatf message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            AatfAddress siteAddress = addressMapper.Map(message.Aatf.SiteAddress);

            Operator @operator = await dataAccess.GetSingleByExpression<Operator>(new OperatorByOrganisationIdSpecification(message.OrganisationId));

            if (@operator == null)
            {
                Organisation organisation = await dataAccess.GetById<Organisation>(message.OrganisationId);

                @operator = new Operator(organisation);

                await dataAccess.Add<Operator>(@operator);
            }

            AatfContact contact = contactMapper.Map(message.AatfContact);

            Aatf aatf = new Aatf(
                message.Aatf.Name,
                message.Aatf.CompetentAuthority.Id,
                message.Aatf.ApprovalNumber,
                Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(message.Aatf.AatfStatus.Value),
                @operator,
                siteAddress,
                Enumeration.FromValue<Domain.AatfReturn.AatfSize>(message.Aatf.Size.Value),
                message.Aatf.ApprovalDate.GetValueOrDefault(),
                contact);

            await dataAccess.Add<Aatf>(aatf);

            return true;
        }
    }
}
