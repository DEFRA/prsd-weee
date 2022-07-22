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
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class EditNoteMapTransferTests
    {
        private readonly Fixture fixture;

        public EditNoteMapTransferTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void GivenNullSchemes_ArgumentNullExceptionExpected()
        {
            //arrange
            var exception = Record.Exception(() => new EditNoteMapTransfer(null,
                new EditEvidenceNoteViewModel(new CategoryValueTotalCalculator()), Guid.NewGuid(), Guid.NewGuid(), null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GivenEmptyOrganisationId_ArgumentExceptionExpected()
        {
            //arrange
            var exception = Record.Exception(() => new EditNoteMapTransfer(new List<OrganisationSchemeData>(),
                new EditEvidenceNoteViewModel(new CategoryValueTotalCalculator()), Guid.Empty, Guid.NewGuid(), null));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GivenEmptyAatfId_ArgumentExceptionExpected()
        {
            //arrange
            var exception = Record.Exception(() => new EditNoteMapTransfer(new List<OrganisationSchemeData>(),
                new EditEvidenceNoteViewModel(new CategoryValueTotalCalculator()), Guid.NewGuid(), Guid.Empty, null));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var schemes = fixture.CreateMany<OrganisationSchemeData>().ToList();
            var evidenceModel = fixture.Create<EditEvidenceNoteViewModel>();
            var organisationId = fixture.Create<Guid>();
            var aatfId = fixture.Create<Guid>();
            var noteData = fixture.Create<EvidenceNoteData>();

            //act
            var result = new EditNoteMapTransfer(schemes, evidenceModel, organisationId, aatfId, noteData);

            //assert
            result.Schemes.Should().BeEquivalentTo(schemes);
            result.ExistingModel.Should().Be(evidenceModel);
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.NoteData.Should().Be(noteData);
        }
    }
}
