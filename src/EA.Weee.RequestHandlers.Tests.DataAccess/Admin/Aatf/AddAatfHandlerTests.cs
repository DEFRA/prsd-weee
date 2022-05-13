namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Aatf
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class AddAatfHandlerTests
    {
        private readonly Fixture fixture;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IMap<AatfContactData, AatfContact> contactMapper;

        public AddAatfHandlerTests()
        {
            fixture = new Fixture();
            addressMapper = A.Fake<IMap<AatfAddressData, AatfAddress>>();
            contactMapper = A.Fake<IMap<AatfContactData, AatfContact>>();
        }

        [Fact]
        public async Task HandleAsync_GivenAatfCanBeCopied_OriginalAatfShouldNotBeChanged()
        {
            using (var databaseWrapper = new DatabaseWrapper())
            {
                var organisation = EA.Weee.Domain.Organisation.Organisation.CreatePartnership("trading");

                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(databaseWrapper, organisation);

                var competentAuthority = new Core.Shared.UKCompetentAuthorityData()
                {
                    Id = databaseWrapper.WeeeContext.UKCompetentAuthorities.First().Id,
                    Abbreviation = databaseWrapper.WeeeContext.UKCompetentAuthorities.First().Abbreviation
                };

                var country = await databaseWrapper.WeeeContext.Countries.SingleAsync(c => c.Name == "France");

                var newContact = A.Dummy<AatfContactData>();
                var contact = new AatfContact("FirstName1", "LastName1", "Position1", "Address11", "Address21", "Town1", "County1", "PO12ST341", country, "Telephone1", "Email1");

                var siteAddress = A.Dummy<AatfAddressData>();
                var aatfAddress = new AatfAddress("Site name", "Site address 1", "Site address 2", "Site town", "Site county", "GU22 7UY", country);

                var aatfdata = new AatfData(Guid.NewGuid(), aatf.Name, aatf.ApprovalNumber, 2020,
                    competentAuthority,
                   Core.AatfReturn.AatfStatus.Approved, siteAddress, Core.AatfReturn.AatfSize.Large, DateTime.Parse("01/01/2020"),
                   null, null)
                {
                    FacilityType = Core.AatfReturn.FacilityType.Aatf
                };

                databaseWrapper.WeeeContext.Aatfs.Add(aatf);

                await databaseWrapper.WeeeContext.SaveChangesAsync();

                A.CallTo(() => addressMapper.Map(siteAddress)).Returns(aatfAddress);

                A.CallTo(() => contactMapper.Map(newContact)).Returns(contact);

                var message = new AddAatf()
                {
                    Aatf = aatfdata,
                    OrganisationId = organisation.Id,
                    AatfId = aatf.AatfId,
                    AatfContact = newContact
                };

                await Handler(databaseWrapper).HandleAsync(message);

                databaseWrapper.WeeeContext.Aatfs.Where(a => a.Id == aatf.Id).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.Aatfs.Where(a => a.AatfId == aatf.AatfId).Count().Should().Be(2);
                databaseWrapper.WeeeContext.Aatfs.Where(a => a.Id == aatf.Id && a.ComplianceYear == 2019).Should().NotBeEmpty();
                databaseWrapper.WeeeContext.Aatfs.Where(a => a.AatfId == aatf.AatfId && a.ComplianceYear == 2020).Count().Should().Be(1);
            }
        }

        private AddAatfRequestHandler Handler(DatabaseWrapper databaseWrapper)
        {
            return new AddAatfRequestHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                 new GenericDataAccess(databaseWrapper.WeeeContext),
                addressMapper, contactMapper, new CommonDataAccess(databaseWrapper.WeeeContext));
        }
    }
}
