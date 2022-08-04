namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;
    using FluentAssertions;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Xunit;

    public class EditNoteMapTransferTests : SimpleUnitTestBase
    {
        [Fact]
        public void GivenNullSchemes_ArgumentNullExceptionExpected()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteData>();
            
            //act
            var exception = Record.Exception(() => new EditNoteMapTransfer(null,
                new EditEvidenceNoteViewModel(new CategoryValueTotalCalculator()), Guid.NewGuid(), Guid.NewGuid(), noteData, TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GivenEmptyOrganisationId_ArgumentExceptionExpected()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteData>();

            //act
            var exception = Record.Exception(() => new EditNoteMapTransfer(new List<OrganisationSchemeData>(),
                new EditEvidenceNoteViewModel(new CategoryValueTotalCalculator()), Guid.Empty, Guid.NewGuid(), noteData, TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GivenEmptyAatfId_ArgumentExceptionExpected()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteData>();

            //act
            var exception = Record.Exception(() => new EditNoteMapTransfer(new List<OrganisationSchemeData>(),
                new EditEvidenceNoteViewModel(new CategoryValueTotalCalculator()), Guid.NewGuid(), Guid.Empty, noteData, TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var schemes = TestFixture.CreateMany<OrganisationSchemeData>().ToList();
            var evidenceModel = TestFixture.Create<EditEvidenceNoteViewModel>();
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var noteData = TestFixture.Create<EvidenceNoteData>();
            var complianceYear = TestFixture.Create<int>();

            //act
            var result = new EditNoteMapTransfer(schemes, evidenceModel, organisationId, aatfId, noteData, complianceYear);

            //assert
            result.Schemes.Should().BeEquivalentTo(schemes);
            result.ExistingModel.Should().Be(evidenceModel);
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.NoteData.Should().Be(noteData);
            result.ComplianceYear.Should().Be(complianceYear);
        }
    }
}
