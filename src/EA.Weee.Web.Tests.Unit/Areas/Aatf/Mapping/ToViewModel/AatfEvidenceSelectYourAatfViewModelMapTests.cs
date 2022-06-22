namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using FluentAssertions;
    using System;
    using Core.AatfReturn;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Weee.Tests.Core;
    using Xunit;

    public class AatfEvidenceSelectYourAatfViewModelMapTests : SimpleUnitTestBase
    {
        private readonly AatfEvidenceSelectYourAatfViewModelMap map;

        public AatfEvidenceSelectYourAatfViewModelMapTests()
        {
            map = new AatfEvidenceSelectYourAatfViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_SelectYourAatfViewModelShouldBeReturned()
        {
            //act
            var result = map.Map(new AatfEvidenceToSelectYourAatfViewModelMapTransfer());

            //assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenSourceWithOrganisation_OrganisationShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();

            //act
            var result = map.Map(new AatfEvidenceToSelectYourAatfViewModelMapTransfer { OrganisationId = organisationId });

            //assert
            result.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public void Map_GivenSourceWithAes_AatfListShouldBeEmpty()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfList = TestFixture.Build<AatfData>().With(a => a.FacilityType, FacilityType.Ae)
                .CreateMany<AatfData>();

            var source = TestFixture.Build<AatfEvidenceToSelectYourAatfViewModelMapTransfer>().With(s => s.AatfList, aatfList).Create();
            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Should().BeEmpty();
        }
    }
}
