namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using FakeItEasy;
    using Web.Areas.Aatf.Helpers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Weee.Tests.Core;
    using Weee.Tests.Core.SpecimenBuilders;
    using Xunit;

    public class AatfEvidenceSelectYourAatfViewModelMapTests : SimpleUnitTestBase
    {
        private readonly AatfEvidenceSelectYourAatfViewModelMap map;
        private readonly IAatfEvidenceHelper aatfEvidenceHelper;

        public AatfEvidenceSelectYourAatfViewModelMapTests()
        {
            aatfEvidenceHelper = A.Fake<IAatfEvidenceHelper>();

            map = new AatfEvidenceSelectYourAatfViewModelMap(aatfEvidenceHelper);

            TestFixture.Customizations.Add(new AatfFacilityTypeGenerator());
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
        public void Map_GivenSource_AatfEvidenceHelperShouldBeCalled()
        {
            //arrange
            var source = TestFixture.Create<AatfEvidenceToSelectYourAatfViewModelMapTransfer>();

            //act
            map.Map(source);

            //assert
            A.CallTo(() => aatfEvidenceHelper.GroupedValidAatfs(source.AatfList)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenSourceAndAatfEvidenceHelperReturnsEmptyList_AatfListShouldBeEmpty()
        {
            //arrange
            var source = TestFixture.Create<AatfEvidenceToSelectYourAatfViewModelMapTransfer>();

            A.CallTo(() => aatfEvidenceHelper.GroupedValidAatfs(A<List<AatfData>>._)).Returns(new List<AatfData>());

            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenSourceAndAatfEvidenceHelperReturnsList_AatfListShouldBeReturned()
        {
            //arrange
            var source = TestFixture.Build<AatfEvidenceToSelectYourAatfViewModelMapTransfer>()
                .With(a => a.AatfList, TestFixture.CreateMany<AatfData>().ToList)
                .Create();

            A.CallTo(() => aatfEvidenceHelper.GroupedValidAatfs(A<List<AatfData>>._)).Returns(source.AatfList);

            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Should().BeSameAs(source.AatfList);
        }

        [Fact]
        public void Map_GivenSourceWithAatfs_ShouldHaveDisplayNameSet()
        {
            //arrange
            var aatfName1 = TestFixture.Create<string>();
            var aatfApprovalNumber1 = TestFixture.Create<string>();
            var aatfName2 = TestFixture.Create<string>();
            var aatfApprovalNumber2 = TestFixture.Create<string>();

            var aatfList = new List<AatfData>()
            {
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.Name, aatfName1)
                    .With(a => a.ApprovalNumber, aatfApprovalNumber1)
                    .Create(),
                TestFixture.Build<AatfData>()
                    .With(a => a.EvidenceSiteDisplay, true)
                    .With(a => a.Name, aatfName2)
                    .With(a => a.ApprovalNumber, aatfApprovalNumber2)
                    .Create()
            };

            var source = TestFixture.Create<AatfEvidenceToSelectYourAatfViewModelMapTransfer>();

            A.CallTo(() => aatfEvidenceHelper.GroupedValidAatfs(A<List<AatfData>>._)).Returns(aatfList);

            //act
            var result = map.Map(source);

            //assert
            result.AatfList.Count.Should().Be(2);
            result.AatfList.Should().Contain(a => a.Name == $"{aatfName1} ({aatfApprovalNumber1})");
            result.AatfList.Should().Contain(a => a.Name == $"{aatfName2} ({aatfApprovalNumber2})");
        }
    }
}
