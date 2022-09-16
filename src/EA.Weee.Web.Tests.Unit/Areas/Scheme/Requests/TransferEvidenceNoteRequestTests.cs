namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Requests
{
    using AutoFixture;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceNoteRequestTests : SimpleUnitTestBase
    {
        [Fact]
        public void UpdateSelectedNotes_GivenNoExistingNotesAndNoDeselectedNotes()
        {
            //arrange
            var request = new TransferEvidenceNoteRequest();

            var noteIds = new List<Guid>() { TestFixture.Create<Guid>(), TestFixture.Create<Guid>() };

            //act
            request.UpdateSelectedNotes(noteIds);

            //assert
            request.EvidenceNoteIds.Should().BeEquivalentTo(noteIds);
        }

        [Fact]
        public void UpdateSelectedNotes_GivenExistingNotesAndNoDeselectedNotes()
        {
            //arrange
            var request = new TransferEvidenceNoteRequest();

            var existingNoteIds = new List<Guid>() { TestFixture.Create<Guid>() };
            var noteIds = new List<Guid>() { TestFixture.Create<Guid>(), TestFixture.Create<Guid>() };

            request.EvidenceNoteIds.AddRange(existingNoteIds);

            //act
            request.UpdateSelectedNotes(noteIds);

            //assert
            request.EvidenceNoteIds.Should().BeEquivalentTo(existingNoteIds.Union(noteIds));
        }

        [Fact]
        public void UpdateSelectedNotes_GivenNoExistingNotesButDeselectedNote_UpdatedNotesShouldNotContainDeselected()
        {
            //arrange
            var request = new TransferEvidenceNoteRequest();

            var deselectedNote = TestFixture.Create<Guid>();
            var notDeselected = TestFixture.Create<Guid>();

            var noteIds = new List<Guid>() { notDeselected, deselectedNote };

            request.DeselectedEvidenceNoteIds.Add(deselectedNote);

            //act
            request.UpdateSelectedNotes(noteIds);

            //assert
            request.EvidenceNoteIds.Should().HaveCount(1);
            request.EvidenceNoteIds.Should().Contain(notDeselected);
            request.EvidenceNoteIds.Should().NotContain(deselectedNote);
        }

        [Fact]
        public void UpdateSelectedNotes_GivenExistingNotesButDeselectedNote_UpdatedNotesShouldNotContainDeselected()
        {
            //arrange
            var request = new TransferEvidenceNoteRequest();

            var deselectedNote = TestFixture.Create<Guid>();
            var notDeselected = TestFixture.Create<Guid>();
            var existingNoteIds = new List<Guid>() { TestFixture.Create<Guid>() };
            var noteIds = new List<Guid>() { notDeselected, deselectedNote };

            request.EvidenceNoteIds.AddRange(existingNoteIds);
            request.DeselectedEvidenceNoteIds.Add(deselectedNote);

            //act
            request.UpdateSelectedNotes(noteIds);

            //assert
            request.EvidenceNoteIds.Should().HaveCount(2);
            request.EvidenceNoteIds.Should().Contain(notDeselected);
            request.EvidenceNoteIds.Should().Contain(existingNoteIds.ElementAt(0));
            request.EvidenceNoteIds.Should().NotContain(deselectedNote);
        }
    }
}
