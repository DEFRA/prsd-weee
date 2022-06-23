namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Internal
{
    using AutoFixture;
    using Core.AatfReturn;
    using DataAccess;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Factories;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using RequestHandlers.Aatf;
    using Weee.Tests.Core;
    using Xunit;
    using FacilityType = Domain.AatfReturn.FacilityType;

    public class AatfDataAccessTests
    {
        private readonly Fixture fixture;
        private readonly WeeeContext context;
        private readonly AatfDataAccess dataAccess;
        private readonly DbContextHelper dbContextHelper;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        public AatfDataAccessTests()
        {
            fixture = new Fixture();
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();

            dataAccess = new AatfDataAccess(context, genericDataAccess, quarterWindowFactory);
        }

        [Fact]
        public async Task GetDetails_GivenAatfId_ContactShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var result = await dataAccess.GetDetails(aatfId);

            result.Should().BeEquivalentTo(aatf);
        }

        [Fact]
        public void UpdateDetails_GivenNewData_SaveChangesAsyncShouldBeCalled()
        {
            var oldDetails = A.Fake<Aatf>();
            var newDetails = fixture.Create<Aatf>();

            dataAccess.UpdateDetails(oldDetails, newDetails);

            A.CallTo(() => oldDetails.UpdateDetails(
                newDetails.Name,
                newDetails.CompetentAuthority,
                newDetails.ApprovalNumber,
                newDetails.AatfStatus,
                newDetails.Organisation,
                newDetails.Size,
                newDetails.ApprovalDate,
                newDetails.LocalArea,
                newDetails.PanArea)).MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public void UpdateAddress_GivenNewData_SaveChangesAsyncShouldBeCalled()
        {
            var oldDetails = A.Fake<AatfAddress>();
            var newDetails = new AatfAddress();
            var country = A.Fake<Country>();

            dataAccess.UpdateAddress(oldDetails, newDetails, country);

            A.CallTo(() => oldDetails.UpdateAddress(
                newDetails.Name,
                newDetails.Address1,
                newDetails.Address2,
                newDetails.TownOrCity,
                newDetails.CountyOrRegion,
                newDetails.Postcode,
                country)).MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task GetContact_GivenAatfId_ContactShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();

            var aatfContact = A.Fake<AatfContact>();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => context.AatfContacts).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<AatfContact>() { aatfContact }));
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Contact).Returns(aatfContact);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var result = await dataAccess.GetContact(aatfId);

            result.Should().BeEquivalentTo(aatfContact);
        }

        [Fact]
        public void UpdateContact_GivenNewAddressData_SaveChangesAsyncShouldBeCalled()
        {
            var oldSite = A.Fake<AatfContact>();
            var newSite = new AatfContactData() { AddressData = new AatfContactAddressData() };
            var country = A.Fake<Country>();

            dataAccess.UpdateContact(oldSite, newSite, country);

            A.CallTo(() => oldSite.UpdateDetails(
                newSite.FirstName,
                newSite.LastName,
                newSite.Position,
                newSite.AddressData.Address1,
                newSite.AddressData.Address2,
                newSite.AddressData.TownOrCity,
                newSite.AddressData.CountyOrRegion,
                newSite.AddressData.Postcode,
                country,
                newSite.Telephone,
                newSite.Email)).MustHaveHappened(1, Times.Exactly)
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, false, true)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        public async void HasAatfData_GivenId_ReturnsBasedOnData(bool hasReceived, bool hasReused, bool hasSentOn, bool expectedResult)
        {
            var weeeReceived = new List<WeeeReceived>();
            var weeeReused = new List<WeeeReused>();
            var weeeSentOn = new List<WeeeSentOn>();

            var aatfId = Guid.NewGuid();

            if (hasReceived)
            {
                var weee = A.Dummy<WeeeReceived>();
                A.CallTo(() => weee.AatfId).Returns(aatfId);
                weeeReceived.Add(weee);
            }

            if (hasReused)
            {
                var weee = A.Dummy<WeeeReused>();
                A.CallTo(() => weee.AatfId).Returns(aatfId);
                weeeReused.Add(weee);
            }

            if (hasSentOn)
            {
                var weee = A.Dummy<WeeeSentOn>();
                A.CallTo(() => weee.AatfId).Returns(aatfId);
                weeeSentOn.Add(weee);
            }

            A.CallTo(() => context.WeeeReceived).Returns(dbContextHelper.GetAsyncEnabledDbSet(weeeReceived));
            A.CallTo(() => context.WeeeReused).Returns(dbContextHelper.GetAsyncEnabledDbSet(weeeReused));
            A.CallTo(() => context.WeeeSentOn).Returns(dbContextHelper.GetAsyncEnabledDbSet(weeeSentOn));
            A.CallTo(() => context.ReturnAatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<ReturnAatf>()));
            A.CallTo(() => context.Returns).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Return>()));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>()));

            var result = await dataAccess.HasAatfData(aatfId);

            expectedResult.Should().Be(result);
        }

        [Fact]
        public async void HasAatfData_GiveAatfHasNilReturn_TrueShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var @return = A.Fake<Return>();
            A.CallTo(() => @return.Id).Returns(returnId);
            A.CallTo(() => @return.NilReturn).Returns(true);

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);

            var returnAatf = new ReturnAatf(aatf, @return);

            A.CallTo(() => context.WeeeReceived).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReceived>()));
            A.CallTo(() => context.WeeeReused).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReused>()));
            A.CallTo(() => context.WeeeSentOn).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>()));
            A.CallTo(() => context.ReturnAatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<ReturnAatf>() { returnAatf }));
            A.CallTo(() => context.Returns).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Return>() { @return }));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var result = await dataAccess.HasAatfData(aatfId);

            result.Should().BeTrue();
        }

        [Fact]
        public async void HasAatfData_GiveAatfDoesNotHaveNilReturnAndNoOtherWeeeData_FalseShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var @return = A.Fake<Return>();
            A.CallTo(() => @return.Id).Returns(returnId);
            A.CallTo(() => @return.NilReturn).Returns(false);

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);

            var returnAatf = new ReturnAatf(aatf, @return);

            A.CallTo(() => context.WeeeReceived).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReceived>()));
            A.CallTo(() => context.WeeeReused).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReused>()));
            A.CallTo(() => context.WeeeSentOn).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>()));
            A.CallTo(() => context.ReturnAatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<ReturnAatf>() { returnAatf }));
            A.CallTo(() => context.Returns).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Return>() { @return }));
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var result = await dataAccess.HasAatfData(aatfId);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void HasAatfOrganisationOtherAeOrAatf_HasAatf_ShouldBeExpectedResult(bool hasOtherAatfs)
        {
            const short complianceYear = 2019;
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => aatf.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);

            var aatfs = new List<Aatf> { aatf };

            if (hasOtherAatfs)
            {
                var otherAatf = A.Fake<Aatf>();
                A.CallTo(() => otherAatf.Organisation).Returns(organisation);
                A.CallTo(() => otherAatf.ComplianceYear).Returns(complianceYear);
                A.CallTo(() => otherAatf.FacilityType).Returns(FacilityType.Aatf);

                aatfs.Add(otherAatf);
            }

            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));

            var result = await dataAccess.HasAatfOrganisationOtherAeOrAatf(aatf);

            hasOtherAatfs.Should().Be(result);
        }

        [Fact]
        public async void HasAatfOrganisationOtherAeOrAatf_GivenNoMatchingComplianceYear_ShouldBeFalse()
        {
            const short complianceYear = 2019;
            const short nonMatchComplianceYear = 2020;

            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => aatf.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);

            var otherAatf = A.Fake<Aatf>();
            A.CallTo(() => otherAatf.Organisation).Returns(organisation);
            A.CallTo(() => otherAatf.ComplianceYear).Returns(nonMatchComplianceYear);
            A.CallTo(() => otherAatf.FacilityType).Returns(FacilityType.Aatf);

            var aatfs = new List<Aatf> { aatf, otherAatf };

            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));

            var result = await dataAccess.HasAatfOrganisationOtherAeOrAatf(aatf);

            result.Should().BeFalse();
        }

        [Fact]
        public async void HasAatfOrganisationOtherAeOrAatf_GivenNoMatchingFacilityType_ShouldBeFalse()
        {
            const short complianceYear = 2019;
            var nonMatchingFacilityType = FacilityType.Ae;

            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => aatf.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);

            var otherAatf = A.Fake<Aatf>();
            A.CallTo(() => otherAatf.Organisation).Returns(organisation);
            A.CallTo(() => otherAatf.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => otherAatf.FacilityType).Returns(nonMatchingFacilityType);

            var aatfs = new List<Aatf> { aatf, otherAatf };

            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));

            var result = await dataAccess.HasAatfOrganisationOtherAeOrAatf(aatf);

            result.Should().BeFalse();
        }

        [Fact]
        public async void DeleteAatf_DeletesAatf()
        {
            var aatfId = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();
            var returnAatf = A.Fake<ReturnAatf>();

            A.CallTo(() => returnAatf.Aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));
            A.CallTo(() => context.ReturnAatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<ReturnAatf>() { returnAatf }));

            await dataAccess.RemoveAatf(aatfId);

            A.CallTo(() => genericDataAccess.Remove(aatf)).MustHaveHappened(1, Times.Exactly)
                .Then(A.CallTo(() => genericDataAccess.Remove(returnAatf)).MustHaveHappened(1, Times.Exactly))
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }

        [Fact]
        public async Task GetDetails_GivenAatfId_ComplianceYearsShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();            
            var aatf = A.Fake<Aatf>();
            var aatfIdForAatf1 = Guid.NewGuid();
            var aatf1 = A.Fake<Aatf>();

            A.CallTo(() => aatf.AatfId).Returns(aatfId);
            A.CallTo(() => aatf.ComplianceYear).Returns((short)2019);
            A.CallTo(() => aatf1.AatfId).Returns(aatfIdForAatf1);
            A.CallTo(() => aatf1.ComplianceYear).Returns((short)2020);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf, aatf1 }));

            var result = await dataAccess.GetComplianceYearsForAatfByAatfId(aatfId);

            result.Should().Contain(2019);
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetDetails_GivenCYAatfId_AatfShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();
            var aatf = A.Fake<Aatf>();
            var aatfIdForAatf1 = Guid.NewGuid();
            var aatf1 = A.Fake<Aatf>();
            var id = Guid.NewGuid();
            var idForAatf1 = Guid.NewGuid();

            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.AatfId).Returns(aatfId);
            A.CallTo(() => aatf.ComplianceYear).Returns((short)2019);

            A.CallTo(() => aatf1.Id).Returns(idForAatf1);
            A.CallTo(() => aatf1.AatfId).Returns(aatfIdForAatf1);
            A.CallTo(() => aatf1.ComplianceYear).Returns((short)2020);

            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf, aatf1 }));

            var result = await dataAccess.GetAatfByAatfIdAndComplianceYear(aatfId, 2019);

            result.Should().Be(aatf.Id);
            result.Should().NotBe(aatf1.Id);
        }
    }
}
