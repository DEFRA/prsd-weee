namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using Core.AatfReturn;
    using Core.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Services.Caching;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ReturnToObligatedViewModelMapTests
    {
        private readonly ReturnToObligatedViewModelMap mapper;
        private readonly IWeeeCache cache;

        public ReturnToObligatedViewModelMapTests()
        {
            cache = A.Fake<IWeeeCache>();

            mapper = new ReturnToObligatedViewModelMap(cache);
        }

        [Fact]
        public void Map_GivenNullSource_ShouldThrowArgumentNullException()
        {
            Action call = () => mapper.Map(null);

            call.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationId_CachedSchemeNameShouldBeSet()
        {
            const string expected = "scheme";
            var schemeData = A.Fake<SchemePublicInfo>();

            A.CallTo(() => schemeData.Name).Returns(expected);

            var transfer = new ReturnToObligatedViewModelTransfer() { OrganisationId = Guid.NewGuid() };

            A.CallTo(() => cache.FetchSchemePublicInfo(transfer.OrganisationId)).Returns(schemeData);

            var result = mapper.Map(transfer);

            result.SchemeName.Should().Be(expected);
        }

        [Fact]
        public void Map_GivenOrganisationAndAatfId_CachedAatfNameShouldBeSet()
        {
            const string expected = "aatf";
            var aatfData = A.Fake<AatfData>();

            A.CallTo(() => aatfData.Name).Returns(expected);

            var transfer = new ReturnToObligatedViewModelTransfer() { OrganisationId = Guid.NewGuid(), AatfId = Guid.NewGuid() };

            A.CallTo(() => cache.FetchAatfData(transfer.OrganisationId, transfer.AatfId)).Returns(aatfData);

            var result = mapper.Map(transfer);

            result.AatfName.Should().Be(expected);
        }

        [Fact]
        public void Map_GivenEntityIds_IdPropertiesShouldBeSet()
        {
            var transfer = new ReturnToObligatedViewModelTransfer() { OrganisationId = Guid.NewGuid(), AatfId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), SchemeId = Guid.NewGuid() };

            var result = mapper.Map(transfer);

            result.AatfId.Should().Be(transfer.AatfId);
            result.OrganisationId.Should().Be(transfer.OrganisationId);
            result.ReturnId.Should().Be(transfer.ReturnId);
            result.SchemeId.Should().Be(transfer.SchemeId);
        }

        //Need to check only relevant enities are mapped
    }
}
