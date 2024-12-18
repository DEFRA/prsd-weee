﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Scheme;
    using EA.Weee.Core.Shared;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class CreateNoteViewModelMapTests : SimpleUnitTestBase
    {
        private readonly CreateNoteViewModelMap map;

        public CreateNoteViewModelMapTests()
        {
            map = new CreateNoteViewModelMap();
        }

        [Fact]
        public void Map_GiveSchemesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new CreateNoteMapTransfer(null, null, Guid.NewGuid(), Guid.NewGuid(), TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new CreateNoteMapTransfer(TestFixture.CreateMany<EntityIdDisplayNameData>().ToList(), null, Guid.Empty, Guid.NewGuid(), TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenAatfIdGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new CreateNoteMapTransfer(TestFixture.CreateMany<EntityIdDisplayNameData>().ToList(), null, Guid.NewGuid(), Guid.Empty, TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenTransferWithoutViewModel_CreateNoteViewModelShouldBeReturned()
        {
            //arrange
            var schemes = TestFixture.CreateMany<EntityIdDisplayNameData>().ToList();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var complianceYear = TestFixture.Create<int>();

            var transfer = new CreateNoteMapTransfer(schemes, null, organisationId, aatfId, complianceYear);

            //act
            var result = map.Map(transfer);

            //assert
            result.Should().NotBeNull();
            result.ComplianceYear.Should().Be(complianceYear);
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.SchemeList.Should().BeEquivalentTo(schemes);
            result.ProtocolList.Should().NotBeNullOrEmpty();
            result.ProtocolList.Should().BeEquivalentTo(new SelectList(EnumHelper.GetValues(typeof(Protocol)), "Key", "Value"));
            result.WasteTypeList.Should().NotBeNullOrEmpty();
            result.WasteTypeList.Should().BeEquivalentTo(new SelectList(EnumHelper.GetValues(typeof(WasteType)), "Key", "Value"));
        }

        [Fact]
        public void Map_GivenTransferWithViewModel_CreateNoteViewModelShouldBeReturned()
        {
            //arrange
            var schemes = TestFixture.CreateMany<EntityIdDisplayNameData>().ToList();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var complianceYear = TestFixture.Create<int>();
            var model = new EditEvidenceNoteViewModel()
            {
                CategoryValues = TestFixture.CreateMany<EvidenceCategoryValue>().ToList()
            };

            var transfer = new CreateNoteMapTransfer(schemes, model, organisationId, aatfId, complianceYear);

            //act
            var result = map.Map(transfer);

            //assert
            result.Should().NotBeNull();
            result.ComplianceYear.Should().Be(complianceYear);
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.CategoryValues.Should().BeEquivalentTo(model.CategoryValues);
            result.SchemeList.Should().BeEquivalentTo(schemes);
            result.ProtocolList.Should().NotBeNullOrEmpty();
            result.ProtocolList.Should().BeEquivalentTo(new SelectList(EnumHelper.GetValues(typeof(Protocol)), "Key", "Value"));
            result.WasteTypeList.Should().NotBeNullOrEmpty();
            result.WasteTypeList.Should().BeEquivalentTo(new SelectList(EnumHelper.GetValues(typeof(WasteType)), "Key", "Value"));
        }
    }
}
