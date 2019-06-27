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
    using RequestHandlers.Factories;
    using RequestHandlers.Shared;
    using Weee.Tests.Core;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;

    public class FetchAatfDataAccessTests
    {
        private readonly IQuarterWindowFactory quarterWindowFactory;

        public FetchAatfDataAccessTests()
        {
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
        }

        [Fact]
        public async Task FetchAatfByReturnQuarterWindow_GivenMatchingParameters_ReturnedListContainsAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);
                var @return = new Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf);

                A.CallTo(() => quarterWindowFactory.GetQuarterWindow(@return.Quarter)).Returns(new QuarterWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1), QuarterType.Q1));

                await genericDataAccess.AddMany<Aatf>(new List<Aatf>() { aatf });

                var aatfList = await dataAccess.FetchAatfByReturnQuarterWindow(@return);

                aatfList.Should().Contain(aatf);
            }
        }

        [Fact]
        public async Task FetchAatfByReturnQuarterWindow_GivenNotMatchingApprovalDate_ReturnedListShouldNotContainAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now.AddDays(1), 2019);
                var @return = new Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf);

                A.CallTo(() => quarterWindowFactory.GetQuarterWindow(@return.Quarter)).Returns(new QuarterWindow(DateTime.Now, DateTime.Now, QuarterType.Q1));

                await genericDataAccess.AddMany<Aatf>(new List<Aatf>() { aatf });

                var aatfList = await dataAccess.FetchAatfByReturnQuarterWindow(@return);

                aatfList.Should().NotContain(aatf);
                aatfList.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task FetchAatfByReturnQuarterWindow_GivenNonMatchingComplianceYear_ReturnedListShouldNotContainAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2020);
                var @return = new Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf);

                A.CallTo(() => quarterWindowFactory.GetQuarterWindow(@return.Quarter)).Returns(new QuarterWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1), QuarterType.Q1));

                await genericDataAccess.AddMany<Aatf>(new List<Aatf>() { aatf });

                var aatfList = await dataAccess.FetchAatfByReturnQuarterWindow(@return);

                aatfList.Should().NotContain(aatf);
                aatfList.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task FetchAatfByReturnId_GivenReturnId_ReturnedListShouldContainAatfs()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                var @return = new EA.Weee.Domain.AatfReturn.Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1),
                    database.WeeeContext.Users.First().Id, FacilityType.Aatf);

                await genericDataAccess.Add(new ReturnAatf(aatf, @return));

                var aatf2 = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                await genericDataAccess.Add(new ReturnAatf(aatf2, @return));

                var aatfList = await dataAccess.FetchAatfByReturnId(@return.Id);

                aatfList.Count.Should().Be(2);
                aatfList.Should().Contain(aatf);
                aatfList.Should().Contain(aatf2);
            }
        }

        [Fact]
        public async Task FetchAatfByReturnId_GivenReturnId_ReturnedListShouldOnlyContainReturnRelatedAatfs()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                var @return = new EA.Weee.Domain.AatfReturn.Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1),
                    database.WeeeContext.Users.First().Id, FacilityType.Aatf);

                await genericDataAccess.Add(new ReturnAatf(aatf, @return));

                var @return2 = new EA.Weee.Domain.AatfReturn.Return(aatf.Organisation, new Quarter(2019, QuarterType.Q1),
                    database.WeeeContext.Users.First().Id, FacilityType.Aatf);

                var aatf2 = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                await genericDataAccess.Add(new ReturnAatf(aatf2, @return2));

                var aatfList = await dataAccess.FetchAatfByReturnId(@return.Id);

                aatfList.Count.Should().Be(1);
                aatfList.Should().Contain(aatf);
                aatfList.Should().NotContain(aatf2);
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
