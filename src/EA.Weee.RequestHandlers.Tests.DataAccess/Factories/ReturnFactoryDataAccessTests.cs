namespace EA.Weee.RequestHandlers.Tests.DataAccess.Factories
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Charges;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using FluentAssertions;
    using RequestHandlers.Factories;
    using RequestHandlers.Shared;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;

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

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, DateTime.Now.AddDays(1), 2019, EA.Weee.Core.AatfReturn.FacilityType.Aatf);

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

                var result = await dataAccess.ValidateAatfApprovalDate(Guid.NewGuid(), DateTime.Now.AddDays(1), 2019, EA.Weee.Core.AatfReturn.FacilityType.Aatf);

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

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, DateTime.Now.AddDays(1), 2019, EA.Weee.Core.AatfReturn.FacilityType.Ae);

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

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, DateTime.Now.AddDays(1), 2019, EA.Weee.Core.AatfReturn.FacilityType.Aatf);

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

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, new DateTime(2019), 2019, EA.Weee.Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ValidateAatfApprovalDate_GivenYearDoesNotMatch_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateAatf(database, organisation, FacilityType.Aatf, 2019, new DateTime(2019));

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.ValidateAatfApprovalDate(organisation.Id, DateTime.Now.AddDays(1), 2020, EA.Weee.Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ValidateReturnQuarter_MatchingParameters_ResultShouldBeTrue()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateReturn(database, organisation, 2019, QuarterType.Q1, FacilityType.Aatf);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.HasReturnQuarter(organisation.Id, 2019, QuarterType.Q1, Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ValidateReturnQuarter_GivenOrganisationDoesNotMatch_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateReturn(database, organisation, 2019, QuarterType.Q1, FacilityType.Aatf);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.HasReturnQuarter(Guid.NewGuid(), 2019, QuarterType.Q1, Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ValidateReturnQuarter_GivenQuarterDoesNotMatch_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateReturn(database, organisation, 2019, QuarterType.Q1, FacilityType.Aatf);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.HasReturnQuarter(organisation.Id, 2019, QuarterType.Q2, Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ValidateReturnQuarter_GivenYearDoesNotMatch_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateReturn(database, organisation, 2019, QuarterType.Q1, FacilityType.Aatf);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.HasReturnQuarter(organisation.Id, 2020, QuarterType.Q1, Core.AatfReturn.FacilityType.Aatf);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ValidateReturnQuarter_GivenFacilityTypeDoesNotMatch_ResultShouldBeFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");

                await CreateReturn(database, organisation, 2019, QuarterType.Q1, FacilityType.Aatf);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var result = await dataAccess.HasReturnQuarter(organisation.Id, 2019, QuarterType.Q1, Core.AatfReturn.FacilityType.Ae);

                result.Should().BeFalse();
            }
        }

        private async Task<Return> CreateReturn(DatabaseWrapper database, Organisation organisation, int year, QuarterType quarter, FacilityType facilityType)
        {
            var @return = new Domain.AatfReturn.Return(organisation, new Quarter(year, quarter), database.Model.AspNetUsers.First().Id, facilityType);

            database.WeeeContext.Returns.Add(@return);

            await database.WeeeContext.SaveChangesAsync();

            return @return;
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
                aatf.UpdateDetails("name", competentAuthority, "12345678", AatfStatus.Approved, organisation, AatfSize.Large, null);
            }

            database.WeeeContext.Aatfs.Add(aatf);

            await database.WeeeContext.SaveChangesAsync();

            return aatf;
        }
    }
}
