namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfReturn;
    using DataAccess;
    using Domain;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.Internal;
    using Weee.Tests.Core;
    using Xunit;

    public class AatfDataAccessTests
    {
        private readonly Fixture fixture;
        private readonly WeeeContext context;
        private readonly AatfDataAccess dataAccess;
        private readonly DbContextHelper dbContextHelper;

        public AatfDataAccessTests()
        {
            fixture = new Fixture();
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();

            dataAccess = new AatfDataAccess(context);
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
                newSite.Email)).MustHaveHappened(Repeated.Exactly.Once)
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, false, true)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        public async void DoesAatfHaveData_GivenId_ReturnsBasedOnData(bool hasReceived, bool hasReused, bool hasSentOn, bool expectedResult)
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

            var result = await dataAccess.DoesAatfHaveData(aatfId);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void DoesAatfOrganisationHaveActiveUsers_ReturnsResultBasedOnActiveUsers(bool hasActiveUsers)
        {
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Organisation).Returns(organisation);

            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var organisationUsers = new List<OrganisationUser>();
            if (hasActiveUsers)
            {
                var user = A.Fake<OrganisationUser>();
                A.CallTo(() => user.OrganisationId).Returns(organisationId);
                organisationUsers.Add(user);
            }

            A.CallTo(() => context.OrganisationUsers).Returns(dbContextHelper.GetAsyncEnabledDbSet(organisationUsers));

            var result = await dataAccess.DoesAatfOrganisationHaveActiveUsers(aatfId);

            Assert.Equal(hasActiveUsers, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void DoesAatfOrganisationHaveMoreAatfs_ReturnsResultBasedOnOtherAatfs(bool hasOtherAatfs)
        {
            var aatfId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Organisation).Returns(organisation);

            var aatfs = new List<Aatf>();

            aatfs.Add(aatf);

            if (hasOtherAatfs)
            {
                var otherAatf = A.Fake<Aatf>();
                A.CallTo(() => otherAatf.Organisation).Returns(organisation);

                aatfs.Add(otherAatf);
            }

            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(aatfs));

            var result = await dataAccess.DoesAatfOrganisationHaveMoreAatfs(aatfId);

            Assert.Equal(hasOtherAatfs, result);
        }

        [Fact]
        public async void DeleteAatf_DeletesAatf()
        {
            var aatfId = Guid.NewGuid();

            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Id).Returns(aatfId);

            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            await dataAccess.RemoveAatf(aatfId);

            A.CallTo(() => context.Aatfs.Remove(aatf)).MustHaveHappened(Repeated.Exactly.Once)
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }
    }
}
