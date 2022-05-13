namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.AatfTaskList
{
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Factories;
    using RequestHandlers.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;

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
        public async Task FetchAatfByReturnQuarterWindow_GivenMatchingParameters_AatfsShouldBeOrderedByName()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatfList = await CreateMultipleAatf(database, FacilityType.Aatf, DateTime.Now, 2019);
                var @return = new Return(aatfList[0].Organisation, new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf);

                A.CallTo(() => quarterWindowFactory.GetQuarterWindow(@return.Quarter)).Returns(new QuarterWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1), QuarterType.Q1));

                await genericDataAccess.AddMany<Aatf>(aatfList);

                var returnedAatfList = await dataAccess.FetchAatfByReturnQuarterWindow(@return);

                aatfList.Should().BeInDescendingOrder(m => m.Name);
            }
        }

        [Fact]
        public async Task FetchAatfByReturnId_GivenMatchingParameters_AatfsShouldBeOrderedByName()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);
                var dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);

                var aatfList = await CreateMultipleAatf(database, FacilityType.Aatf, DateTime.Now, 2019);
                var @return = new Return(aatfList[0].Organisation, new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf);

                A.CallTo(() => quarterWindowFactory.GetQuarterWindow(@return.Quarter)).Returns(new QuarterWindow(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1), QuarterType.Q1));

                await genericDataAccess.AddMany<Aatf>(aatfList);

                var returnedAatfList = await dataAccess.FetchAatfByReturnId(@return.Id);

                aatfList.Should().BeInDescendingOrder(m => m.Name);
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
        public async Task FetchById_GivenAatfId_CorrectAatfShouldBeReturned()
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

                var @return2 = new EA.Weee.Domain.AatfReturn.Return(aatf2.Organisation, new Quarter(2019, QuarterType.Q1),
                    database.WeeeContext.Users.First().Id, FacilityType.Aatf);

                await genericDataAccess.Add(new ReturnAatf(aatf2, @return2));

                var returnedAatf = await dataAccess.FetchById(aatf.Id);

                returnedAatf.Should().BeEquivalentTo(aatf);
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

        [Theory]
        [InlineData(Core.AatfReturn.FacilityType.Aatf)]
        [InlineData(Core.AatfReturn.FacilityType.Ae)]
        public async Task FetchAatfByApprovalNumber_GivenApprovalNumber_ReturnedShouldBeAatf(Core.AatfReturn.FacilityType type)
        {
            FacilityType facilityType;

            if (type == Core.AatfReturn.FacilityType.Aatf)
            {
                facilityType = FacilityType.Aatf;
            }
            else
            {
                facilityType = FacilityType.Ae;
            }

            using (var database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);
                FetchAatfDataAccess dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                GenericDataAccess genericDataAccess = new GenericDataAccess(database.WeeeContext);

                string approvalNumber = "test";

                Aatf aatf = await CreateAatf(database, facilityType, DateTime.Now, 2019, approvalNumber);

                await genericDataAccess.Add(aatf);

                Aatf result = await dataAccess.FetchByApprovalNumber(approvalNumber);

                Assert.NotNull(result);
                Assert.Equal(approvalNumber, result.ApprovalNumber);
                Assert.Equal(facilityType, result.FacilityType);
            }
        }

        [Fact]
        public async Task FetchAatfByApprovalNumber_GivenApprovalNumberThatDoesntExist_ReturnedShouldNull()
        {
            using (var database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);
                FetchAatfDataAccess dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                GenericDataAccess genericDataAccess = new GenericDataAccess(database.WeeeContext);

                string approvalNumber = "test";

                Aatf aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                await genericDataAccess.Add(aatf);

                Aatf result = await dataAccess.FetchByApprovalNumber(approvalNumber);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task FetchAatfByApprovalNumber_GivenApprovalNumberThatDoesntExistForCY_ReturnedShouldBeNull()
        {
            using (var database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);
                FetchAatfDataAccess dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                GenericDataAccess genericDataAccess = new GenericDataAccess(database.WeeeContext);

                string approvalNumber = "test";

                Aatf aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019);

                await genericDataAccess.Add(aatf);

                Aatf result = await dataAccess.FetchByApprovalNumber(approvalNumber, 2019);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task FetchAatfByApprovalNumber_GivenApprovalNumberThatExistForCY_ReturnedShouldNotBeNull()
        {
            using (var database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);
                FetchAatfDataAccess dataAccess = new FetchAatfDataAccess(database.WeeeContext, quarterWindowFactory);
                GenericDataAccess genericDataAccess = new GenericDataAccess(database.WeeeContext);

                string approvalNumber = "test";

                Aatf aatf = await CreateAatf(database, FacilityType.Aatf, DateTime.Now, 2019, approvalNumber);

                await genericDataAccess.Add(aatf);

                Aatf result = await dataAccess.FetchByApprovalNumber(approvalNumber, 2019);

                Assert.NotNull(result);
            }
        }

        private async Task<Aatf> CreateAatf(DatabaseWrapper database, FacilityType facilityType, DateTime date, short year, string approvalNumber = null, string name = null)
        {
            var country = database.WeeeContext.Countries.First();
            var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
            var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
            var organisation = Organisation.CreatePartnership("Dummy");
            var contact = new AatfContact("First Name", "Last Name", "Manager", "1 Address Lane", "Address Ward", "Town", "County", "Postcode", country, "01234 567890", "email@email.com");

            if (approvalNumber == null)
            {
                approvalNumber = "12345678";
            }

            if (name == null)
            {
                name = "name";
            }

            return new Aatf(name,
                competentAuthority,
                approvalNumber,
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

        private async Task<List<Aatf>> CreateMultipleAatf(DatabaseWrapper database, FacilityType facilityType, DateTime date, short year)
        {
            var country = database.WeeeContext.Countries.First();
            var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
            var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
            var organisation = Organisation.CreatePartnership("Dummy");
            var contact = new AatfContact("First Name", "Last Name", "Manager", "1 Address Lane", "Address Ward", "Town", "County", "Postcode", country, "01234 567890", "email@email.com");
            var aatfList = new List<Aatf>();

            aatfList.Add(new Aatf("B",
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
                database.WeeeContext.PanAreas.First()));

            aatfList.Add(new Aatf("A",
                competentAuthority,
                "12345679",
                AatfStatus.Approved,
                organisation,
                AddressHelper.GetAatfAddress(database),
                A.Fake<AatfSize>(),
                date,
                contact,
                facilityType,
                year,
                database.WeeeContext.LocalAreas.First(),
                database.WeeeContext.PanAreas.First()));

            return aatfList;
        }
    }
}
