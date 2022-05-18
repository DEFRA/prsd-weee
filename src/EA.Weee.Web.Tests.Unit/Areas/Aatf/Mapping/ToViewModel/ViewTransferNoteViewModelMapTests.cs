namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Tests.Unit.Helpers;
    using FluentAssertions;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Xunit;

    public class ViewTransferNoteViewModelMapTests
    {
        private readonly Fixture fixture;
        private readonly ViewTransferNoteViewModelMap map;

        public ViewTransferNoteViewModelMapTests()
        {
            fixture = new Fixture();
            map = new ViewTransferNoteViewModelMap();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSource_ViewTransferNoteViewModelShouldBeReturned()
        {
            //arrange
            var source = fixture.Create<ViewTransferNoteViewModelMapTransfer>();

            //act
            var model = map.Map(source);

            //assert
            model.Should().NotBeNull();
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSource_PropertiesShouldBeSet()
        {
            //arrange
            var source = fixture.Create<ViewTransferNoteViewModelMapTransfer>();

            //act
            var model = map.Map(source);

            //assert
            model.SchemeId.Should().Be(source.SchemeId);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenSourceNoteType_PropertiesShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                    fixture.Build<TransferEvidenceNoteData>().With(t1 => t1.Type, NoteType.Transfer).Create(),
                    null);

            //act
            var model = map.Map(source);

            //assert
            model.Type.Should().Be(source.TransferEvidenceNoteData.Type);
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferNoteViewModelMap_GivenSourceNoteStatus_PropertiesShouldBeSet(NoteStatus status)
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>().With(t1 => t1.Status, status).Create(),
                null);

            //act
            var model = map.Map(source);

            //assert
            model.Status.Should().Be(source.TransferEvidenceNoteData.Status);
        }

        [Fact]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationAndNoteIsDraft_SuccessMessageShouldBeSet()
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
                    .With(t => t.Status, NoteStatus.Draft)
                    .With(t => t.Type, NoteType.Transfer)
                    .With(t => t.Reference, 1).Create(),
                true);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should()
                .Be($"You have successfully saved the transfer evidence note with reference ID {source.TransferEvidenceNoteData.Type.ToDisplayString()}{source.TransferEvidenceNoteData.Reference} as a draft");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        public void ViewTransferNoteViewModelMap_GivenDisplayNotificationIsFalse_SuccessMessageShouldBeEmpty(bool? display)
        {
            //arrange
            var source = new ViewTransferNoteViewModelMapTransfer(fixture.Create<Guid>(),
                fixture.Build<TransferEvidenceNoteData>()
                    .With(t2 => t2.Status, NoteStatus.Draft)
                    .With(t1 => t1.Reference, 1).Create(),
                display);

            //act
            var model = map.Map(source);

            //assert
            model.SuccessMessage.Should().BeNullOrEmpty();
        }
    }
}
