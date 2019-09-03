namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Xunit;

    public class AssociatedEntitiesViewModelMapTests
    {
        private readonly Fixture fixture;
        private readonly AssociatedEntitiesViewModelMap map;

        public AssociatedEntitiesViewModelMapTests()
        {
            fixture = new Fixture();

            map = new AssociatedEntitiesViewModelMap();
        }

        [Fact]
        public void Map_ThereAreRelatedAatfs_HasAnyRelatedEntitiesIsTrue()
        {
            var aatf = GetAatfDataList(FacilityType.Aatf, fixture.Create<short>(), fixture.Create<DateTime>(), fixture.Create<Guid>(),
                fixture.Create<Guid>());

            var model = map.Map(new AssociatedEntitiesViewModelTransfer() { AssociatedAatfs = new List<AatfDataList>() { aatf }, AatfId = fixture.Create<Guid>() });

            model.HasAnyRelatedEntities.Should().BeTrue();
        }

        [Fact]
        public void Map_ThereAreEmptyRelatedAatfs_HasAnyRelatedEntitiesIsFalse()
        {
            var model = map.Map(new AssociatedEntitiesViewModelTransfer() { AssociatedAatfs = new List<AatfDataList>(), AatfId = fixture.Create<Guid>() });

            model.HasAnyRelatedEntities.Should().BeFalse();
        }

        [Fact]
        public void Map_ThereAreNullRelatedAatfs_HasAnyRelatedEntitiesIsFalse()
        {
            var model = map.Map(new AssociatedEntitiesViewModelTransfer() { AatfId = fixture.Create<Guid>() });

            model.HasAnyRelatedEntities.Should().BeFalse();
        }

        [Fact]
        public void Map_ThereAreRelatedAes_HasAnyRelatedEntitiesIsTrue()
        {
            var ae = GetAatfDataList(FacilityType.Ae, fixture.Create<short>(), fixture.Create<DateTime>(), fixture.Create<Guid>(),
                fixture.Create<Guid>());

            var model = map.Map(new AssociatedEntitiesViewModelTransfer() { AssociatedAatfs = new List<AatfDataList>() { ae }, AatfId = fixture.Create<Guid>() });

            model.HasAnyRelatedEntities.Should().BeTrue();
        }

        [Fact]
        public void Map_ThereAreRelatedSchemes_HasAnyRelatedEntitiesIsTrue()
        {
            var model = map.Map(new AssociatedEntitiesViewModelTransfer() { AssociatedSchemes = new List<SchemeData>() { A.Fake<SchemeData>() }, AatfId = fixture.Create<Guid>() });

            model.HasAnyRelatedEntities.Should().BeTrue();
        }

        [Fact]
        public void Map_ThereAreEmptyRelatedSchemes_HasAnyRelatedEntitiesIsFalse()
        {
            var model = map.Map(new AssociatedEntitiesViewModelTransfer() { AssociatedSchemes = new List<SchemeData>(), AatfId = fixture.Create<Guid>() });

            model.HasAnyRelatedEntities.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenValidSource_WithNoAssociatedSchemes_PropertyShouldBeNull()
        {
            var aatfData = fixture.Create<AatfData>();

            var transfer = new AssociatedEntitiesViewModelTransfer() { AatfId = aatfData.AatfId };

            var result = map.Map(transfer);

            result.AssociatedSchemes.Should().BeNull();
        }

        [Fact]
        public void Map_GivenValidSource_WithAssociatedSchemes_PropertiesShouldBeMapped()
        {
            var aatfData = fixture.Create<AatfData>();
            var associatedSchemes = A.Fake<List<Core.Scheme.SchemeData>>();

            var transfer = new AssociatedEntitiesViewModelTransfer()
            {
                AatfId = aatfData.AatfId,
                AssociatedSchemes = associatedSchemes
            };

            var result = map.Map(transfer);

            result.AssociatedSchemes.Should().BeEquivalentTo(associatedSchemes);
        }

        [Fact]
        public void Map_GivenValidSource_WithAssociatedSchemesAndSchemeIdIsProvided_SchemeWithIdShouldBeFilteredFromSchemeList()
        {
            var schemeId = fixture.Create<Guid>();
            var matchedScheme = fixture.Build<SchemeData>()
                .With(s => s.Id, schemeId).Create();

            var notMatchedScheme = fixture.Build<SchemeData>().Create();

            var associatedSchemes = new List<SchemeData>()
            {
                matchedScheme,
                notMatchedScheme
            };

            var transfer = new AssociatedEntitiesViewModelTransfer()
            {
                AssociatedSchemes = associatedSchemes,
                SchemeId = schemeId
            };

            var result = map.Map(transfer);

            result.AssociatedSchemes.Count.Should().Be(1);
            result.AssociatedSchemes.Should().Contain(notMatchedScheme);
            result.AssociatedSchemes.Should().NotContain(matchedScheme);
        }

        [Fact]
        public void Map_GivenValidSource_WithAatfs_AEListShouldOnlyContainAEsAndAatfListShouldOnlyContainAatfs()
        {
            var aatfData = fixture.Create<AatfData>();
            var associatedAatfs = new List<AatfDataList>
            {
                GetAatfDataList(FacilityType.Aatf, fixture.Create<short>(), fixture.Create<DateTime>(), fixture.Create<Guid>(), fixture.Create<Guid>()),
                GetAatfDataList(FacilityType.Ae, fixture.Create<short>(), fixture.Create<DateTime>(), fixture.Create<Guid>(), fixture.Create<Guid>())
            };

            var transfer = new AssociatedEntitiesViewModelTransfer()
            {
                AatfId = aatfData.AatfId,
                AssociatedAatfs = associatedAatfs
            };

            var result = map.Map(transfer);

            foreach (var ae in result.AssociatedAes)
            {
                ae.FacilityType.Should().Be(FacilityType.Ae);
            }

            foreach (var aatf in result.AssociatedAatfs)
            {
                aatf.FacilityType.Should().Be(FacilityType.Aatf);
            }
        }

        [Fact]
        public void Map_GivenValidSource_WithAatfs_AatfListShouldNotContainAatfFoundInSource()
        {
            var aatfData = fixture.Create<AatfData>();
            var aatfDataList = GetAatfDataList(FacilityType.Aatf, fixture.Create<short>(), fixture.Create<DateTime>(), aatfData.AatfId, aatfData.Id);

            var associatedAatfs = new List<AatfDataList>
            {
                GetAatfDataList(FacilityType.Aatf, fixture.Create<short>(), fixture.Create<DateTime>(), fixture.Create<Guid>(), fixture.Create<Guid>()),
                aatfDataList
            };

            var transfer = new AssociatedEntitiesViewModelTransfer()
            {
                AatfId = aatfData.AatfId,
                AssociatedAatfs = associatedAatfs
            };

            var result = map.Map(transfer);

            foreach (var aatf in result.AssociatedAatfs)
            {
                aatf.Id.Should().NotBe(aatfDataList.Id);
            }
        }

        [Fact]
        public void Map_GivenValidSource_WithAatfs_AeListShouldNotContainAatfFoundInSource()
        {
            var aatfData = fixture.Create<AatfData>();
            var aatfDataList = GetAatfDataList(FacilityType.Ae, fixture.Create<short>(), fixture.Create<DateTime>(), aatfData.AatfId, aatfData.Id);

            var associatedAatfs = new List<AatfDataList>
            {
                GetAatfDataList(FacilityType.Ae, fixture.Create<short>(), fixture.Create<DateTime>(), fixture.Create<Guid>(), fixture.Create<Guid>()),
                aatfDataList
            };

            var transfer = new AssociatedEntitiesViewModelTransfer()
            {
                AatfId = aatfData.AatfId,
                AssociatedAatfs = associatedAatfs
            };

            var result = map.Map(transfer);

            foreach (var ae in result.AssociatedAes)
            {
                ae.Id.Should().NotBe(aatfDataList.Id);
            }
        }

        [Fact]
        public void Map_GivenListOfAatfs_GivenAatfsWithTheSameAatfIdTheLatestApprovalDateShouldBeReturned()
        {
            var aatfData = fixture.Create<AatfData>();
            var aatfId = fixture.Create<Guid>();

            var aatfDataList1 = GetAatfDataList(FacilityType.Aatf, 2019, new DateTime(2018, 10, 1), aatfId, fixture.Create<Guid>());
            var aatfDataList2 = GetAatfDataList(FacilityType.Aatf, 2020, new DateTime(2019, 10, 1), aatfId, fixture.Create<Guid>());

            var associatedAatfs = new List<AatfDataList>()
            {
                aatfDataList1,
                aatfDataList2
            };

            var transfer = new AssociatedEntitiesViewModelTransfer()
            {
                AatfId = aatfData.AatfId,
                AssociatedAatfs = associatedAatfs
            };

            var result = map.Map(transfer);

            result.AssociatedAatfs.Count.Should().Be(1);
            result.AssociatedAatfs.Should().Contain(aatfDataList2);
            result.AssociatedAatfs.Should().NotContain(aatfDataList1);
        }

        [Fact]
        public void Map_GivenListOfAatfsAndNoAatfId_AssociatedAatfsShouldBeEntireList()
        {
            var aatfData = fixture.Create<AatfData>();
            var aatfId = fixture.Create<Guid>();

            var aatfDataList1 = GetAatfDataList(FacilityType.Aatf, fixture.Create<short>(), fixture.Create<DateTime>(), null, fixture.Create<Guid>());
            var aatfDataList2 = GetAatfDataList(FacilityType.Aatf, fixture.Create<short>(), fixture.Create<DateTime>(), null, fixture.Create<Guid>());

            var associatedAatfs = new List<AatfDataList>()
            {
                aatfDataList1,
                aatfDataList2
            };

            var transfer = new AssociatedEntitiesViewModelTransfer()
            {
                AssociatedAatfs = associatedAatfs
            };

            var result = map.Map(transfer);

            result.AssociatedAatfs.Count.Should().Be(2);
            result.AssociatedAatfs.Should().Contain(aatfDataList2);
            result.AssociatedAatfs.Should().Contain(aatfDataList1);
        }

        private AatfDataList GetAatfDataList(FacilityType facilityType, short complianceYear, DateTime approvalDate, Guid? aatfId, Guid id)
        {
            return new AatfDataList(id,
                fixture.Create<string>(),
                fixture.Create<UKCompetentAuthorityData>(),
                fixture.Create<string>(),
                fixture.Create<AatfStatus>(),
                fixture.Create<OrganisationData>(),
                facilityType,
                complianceYear,
                aatfId ?? Guid.NewGuid(),
                approvalDate);
        }
    }
}
