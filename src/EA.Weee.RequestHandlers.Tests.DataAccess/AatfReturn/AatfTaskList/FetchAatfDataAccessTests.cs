namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.RequestHandlers.Charges;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Shared;
    using Weee.Tests.Core;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;

    public class FetchAatfDataAccessTests
    {
        [Fact]
        public async Task FetchAatfByOrganisationIdAndQuarter_GivenMatchingParameters_ReturnedListContainsAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);
                
                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);
              
                await genericDataAccess.AddMany<Aatf>(new List<Aatf>() { aatf });

                var aatfList = await dataAccess.FetchAatfByOrganisationIdAndQuarter(aatf.Organisation.Id, 2019, DateTime.Now.AddDays(1));

                aatfList.Should().Contain(aatf);
            }
        }

        [Fact]
        public async Task FetchAatfByOrganisationIdAndQuarter_GivenNotMatchingApprovalDate_ReturnedListShouldNotContainAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);
                
                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now.AddDays(1), 2019);

                await genericDataAccess.AddMany<Aatf>(new List<Aatf>() { aatf });

                var aatfList = await dataAccess.FetchAatfByOrganisationIdAndQuarter(aatf.Organisation.Id, 2019, DateTime.Now);

                aatfList.Should().NotContain(aatf);
                aatfList.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task FetchAatfByOrganisationIdAndQuarter_GivenNonMatchingComplianceYear_ReturnedListShouldNotContainAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                await genericDataAccess.AddMany<Aatf>(new List<Aatf>() { aatf });

                var aatfList = await dataAccess.FetchAatfByOrganisationIdAndQuarter(aatf.Organisation.Id, 2020, DateTime.Now.AddDays(1));

                aatfList.Should().NotContain(aatf);
                aatfList.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task FetchAatfByReturnId_GivenReturnWithSentOnAatf_ReturnedListShouldContainAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                var @return = new EA.Weee.Domain.AatfReturn.Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1),
                    database.WeeeContext.Users.First().Id, FacilityType.Aatf);

                await CreateWeeeSentOn(database, aatf, @return);

                var aatfList = await dataAccess.FetchAatfByReturnId(@return.Id);

                aatfList.Should().Contain(aatf);
            }
        }

        [Fact]
        public async Task FetchAatfByReturnId_GivenReturnWithReusedAatf_ReturnedListShouldContainAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                var @return = new EA.Weee.Domain.AatfReturn.Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1),
                    database.WeeeContext.Users.First().Id, FacilityType.Aatf);

                await CreateWeeeReused(aatf, @return, genericDataAccess);

                var aatfList = await dataAccess.FetchAatfByReturnId(@return.Id);

                aatfList.Should().Contain(aatf);
            }
        }

        [Fact]
        public async Task FetchAatfByReturnId_GivenReturnWithReceivedOnAatf_ReturnedListShouldContainAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                var @return = new EA.Weee.Domain.AatfReturn.Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1),
                    database.WeeeContext.Users.First().Id, FacilityType.Aatf);

                await CreateWeeeReceived(aatf, @return, genericDataAccess);

                var aatfList = await dataAccess.FetchAatfByReturnId(@return.Id);

                aatfList.Should().Contain(aatf);
            }
        }

        [Fact]
        public async Task FetchAatfByReturnId_GivenReturnMultipleReturnTypes_ReturnedListShouldContainDistinctAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                var @return = new EA.Weee.Domain.AatfReturn.Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1),
                    database.WeeeContext.Users.First().Id, FacilityType.Aatf);

                await CreateWeeeReceived(aatf, @return, genericDataAccess);
                await CreateWeeeReceived(aatf, @return, genericDataAccess);

                await CreateWeeeReused(aatf, @return, genericDataAccess);
                await CreateWeeeReused(aatf, @return, genericDataAccess);

                await CreateWeeeSentOn(database, aatf, @return);
                await CreateWeeeSentOn(database, aatf, @return);

                var aatf2 = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                await CreateWeeeSentOn(database, aatf2, @return);

                var aatfList = await dataAccess.FetchAatfByReturnId(@return.Id);

                aatfList.Count.Should().Be(2);
                aatfList.Should().Contain(aatf);
                aatfList.Should().Contain(aatf2);
            }
        }

        private async Task CreateWeeeReceived(Aatf aatf, Return @return, GenericDataAccess genericDataAccess)
        {
            var scheme = new EA.Weee.Domain.Scheme.Scheme(aatf.Organisation);

            var received = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf, @return);

            await genericDataAccess.Add<Domain.AatfReturn.WeeeReceived>(received);
        }

        private async Task CreateWeeeSentOn(DatabaseWrapper database, Aatf aatf, Return @return)
        {
            var genericDataAccess = new GenericDataAccess(database.WeeeContext);

            var sentOn = new EA.Weee.Domain.AatfReturn.WeeeSentOn(AddressHelper.GetAatfAddress(database), AddressHelper.GetAatfAddress(database),
                aatf, @return);

            await genericDataAccess.Add(sentOn);
        }

        private async Task CreateWeeeReused(Aatf aatf, Return @return, GenericDataAccess genericDataAccess)
        {
            var reused = new EA.Weee.Domain.AatfReturn.WeeeReused(aatf, @return);

            await genericDataAccess.Add<Domain.AatfReturn.WeeeReused>(reused);
        }

        private async Task<Aatf> CreateAatf(DatabaseWrapper database, FacilityType facilityType, DateTime date, short year)
        {
            var country = database.WeeeContext.Countries.First();
            var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
            var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
            var organisation = Organisation.CreatePartnership("Dummy");
            var contact = new AatfContact("First Name", "Last Name", "Manager", "1 Address Lane", "Address Ward", "Town", "County", "Postcode", country, "01234 567890", "email@email.com");

            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                organisation,
                AddressHelper.GetAatfAddress(database),
                A.Fake<AatfSize>(),
                date,
                contact,
                facilityType,
                year,
                database.WeeeContext.LocalAreas.First(),
                database.WeeeContext.PanAreas.First());
        }
    }
}
