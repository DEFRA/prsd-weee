namespace EA.Weee.RequestHandlers.Tests.DataAccess.Factories
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Charges;
    using Domain.AatfReturn;
    using FluentAssertions;
    using RequestHandlers.Factories;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Organisation = Domain.Organisation.Organisation;

    public class ReturnFactoryDataAccessTests
    {
        public ReturnFactoryDataAccessTests()
        {
        }

        [Fact]
        public async Task ValidateAatfApprovalDate_MatchingParameters_ResultShouldBeTrue()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");
                
                await CreateAatf(database, organisation, FacilityType.Aatf, 2019, DateTime.Now);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, DateTime.Now.AddDays(1), EA.Weee.Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ValidateAatfApprovalDate_GivenOrganisationDoesNotMatch_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateAatf(database, organisation, FacilityType.Aatf, 2019, DateTime.Now);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.ValidateAatfApprovalDate(Guid.NewGuid(), new DateTime(2019), EA.Weee.Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ValidateAatfApprovalDate_GivenFacilityDoesNotMatch_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateAatf(database, organisation, FacilityType.Aatf, 2019, DateTime.Now);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, new DateTime(2019), EA.Weee.Core.AatfReturn.FacilityType.Ae);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ValidateAatfApprovalDate_GivenApprovalDateIsNull_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateAatf(database, organisation, FacilityType.Aatf, 2019, null);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, new DateTime(2019), EA.Weee.Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ValidateAatfApprovalDate_GivenApprovalDateIsAfterDate_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateAatf(database, organisation, FacilityType.Aatf, 2019, new DateTime(2019).AddDays(1));

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, new DateTime(2019), EA.Weee.Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        private async Task<Aatf> CreateAatf(DatabaseWrapper database, Organisation organisation, FacilityType facilityType, short year, DateTime? approvalDate)
        {
            var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
            var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
            var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

            var aatf = new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                organisation,
                new AatfAddress("name", "one", "two", "bath", "BANES", "BA2 2PL", country),
                AatfSize.Large, 
                DateTime.Now,
                new AatfContact("first", "last", "position", "adress1", "address2", "town", "county", "postcode", country, "telephone", "email"), 
                facilityType,
                year);

            if (!approvalDate.HasValue)
            {
                aatf.UpdateDetails("name", competentAuthority.Id, "12345678", AatfStatus.Approved, organisation, AatfSize.Large, null);
            }

            database.WeeeContext.Aatfs.Add(aatf);

            await database.WeeeContext.SaveChangesAsync();

            return aatf;
        }
    }
}
