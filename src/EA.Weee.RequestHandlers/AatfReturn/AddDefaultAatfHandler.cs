namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Charges;
    using Core.Shared;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Specification;

    public class AddDefaultAatfHandler : IRequestHandler<AddDefaultAatf, bool>
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly WeeeContext context;

        public AddDefaultAatfHandler(IGenericDataAccess genericDataAccess,
            ICommonDataAccess commonDataAccess,
            WeeeContext context)
        {
            this.genericDataAccess = genericDataAccess;
            this.commonDataAccess = commonDataAccess;
            this.context = context;
        }

        public async Task<bool> HandleAsync(AddDefaultAatf message)
        {
            var @operator = await genericDataAccess.GetSingleByExpression<Operator>(new OperatorByOrganisationIdSpecification(message.OrganisationId));

            if (@operator == null)
            {
                var organisation = await genericDataAccess.GetById<Organisation>(message.OrganisationId);

                @operator = new Operator(organisation);
            }

            var competantAuthority = await commonDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

            var country = await context.Countries.SingleAsync(c => c.Name == "UK - England");

            var contact = CreateDefaultContact(country);

            if (!context.Aatfs.Any(a => a.Operator.Organisation.Id == message.OrganisationId))
            {
                context.Aatfs.AddRange(GetAatfs(@operator, competantAuthority, contact, country));
            }

            await context.SaveChangesAsync();

            return true;
        }

        private List<Aatf> GetAatfs(Operator @operator, UKCompetentAuthority competentAuthority, AatfContact contact, Country country)
        {
            var siteAddress = CreateAatfSiteAddress(country);
            var aatfs = new List<Aatf>()
            {
                new Aatf("ABB Ltd Darlaston", competentAuthority, "123456789", AatfStatus.Approved, @operator, siteAddress, AatfSize.Large, DateTime.Now, contact, FacilityType.Aatf),
                new Aatf("ABB Ltd Woking", competentAuthority, "123456789", AatfStatus.Approved, @operator, siteAddress, AatfSize.Large, DateTime.Now, contact, FacilityType.Aatf),
                new Aatf("ABB Ltd Maidenhead", competentAuthority, "123456789", AatfStatus.Approved, @operator, siteAddress, AatfSize.Large, DateTime.Now, contact, FacilityType.Aatf),
                new Aatf("AE Ltd Felixstowe", competentAuthority, "123456789", AatfStatus.Approved, @operator, siteAddress, AatfSize.Large, DateTime.Now, contact, FacilityType.Ae),
            };

            return aatfs;
        }

        private AatfAddress CreateAatfSiteAddress(Country country)
        {
            return new AatfAddress("Name", "Building", "Road", "Bath", "BANES", "BA2 2YU", country);
        }

        private AatfContact CreateDefaultContact(Country country)
        {
            return new AatfContact("First Name", "Last Name", "Manager", "1 Address Lane", "Address Ward", "Town", "County", "Postcode", country, "01234 567890", "email@email.com");
        }
    }
}
