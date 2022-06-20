namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using FluentAssertions;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Xunit;

    public class EvidenceNotesViewModelTransferTests
    {
        private readonly Fixture fixture;

        public EvidenceNotesViewModelTransferTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Map_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.NewGuid(), Guid.NewGuid(), null, SystemTime.Now));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.Empty, Guid.NewGuid(), fixture.CreateMany<EvidenceNoteData>().ToList(), SystemTime.Now));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIdGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.NewGuid(), Guid.Empty, fixture.CreateMany<EvidenceNoteData>().ToList(), SystemTime.Now));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }
    }
}
