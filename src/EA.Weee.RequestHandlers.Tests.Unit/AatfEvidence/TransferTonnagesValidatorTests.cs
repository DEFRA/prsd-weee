namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Tests.Unit.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Lookup;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfEvidence;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class TransferTonnagesValidatorTests
    {
        private readonly TransferTonnagesValidator validator;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly Fixture fixture;

        public TransferTonnagesValidatorTests()
        {
            fixture = new Fixture();

            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();

            validator = new TransferTonnagesValidator(evidenceDataAccess);
        }

        [Fact]
        public async Task Validate_GivenTransferValues_NoteTonnageShouldBeRetrieved()
        {
            //arrange
            var transferValues = TransferTonnageValues();

            //act
            await validator.Validate(transferValues);

            //assert
            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(
                A<List<Guid>>.That.Matches(m => m.SequenceEqual(transferValues.Select(t => t.TransferTonnageId).ToList())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Validate_GivenAnyExistingTonnageHasNullReceived_InvalidOperationExceptionExpected()
        {
            //arrange
            var transferValues = TransferTonnageValues();

            var tonnage = new NoteTonnage(fixture.Create<WeeeCategory>(), null, 1);
            
            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { tonnage });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public async Task Validate_GivenRequestedTonnageIsAvailableAsNoOtherTransfers_NoExceptionExpected()
        {
            var transferTonnageId1 = Guid.NewGuid();
            var transferTonnageValue1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferTonnageValue1.TransferTonnageId).Returns(transferTonnageId1);
            A.CallTo(() => transferTonnageValue1.FirstTonnage).Returns(10);

            var transferValues = new List<TransferTonnageValue>()
            {
                transferTonnageValue1
            };

            var noteTonnage = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage.Id).Returns(transferTonnageId1);
            A.CallTo(() => noteTonnage.Received).Returns(10);

            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { noteTonnage });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues));

            //assert
            exception.Should().BeNull();
        }

        [Fact]
        public async Task Validate_GivenRequestedTonnageIsAvailableWithOtherTransfers_NoExceptionExpected()
        {
            var transferValueTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.TransferTonnageId).Returns(transferValueTonnageId1);
            A.CallTo(() => transferValuesTonnage1.FirstTonnage).Returns(1);
            var transferValues = new List<TransferTonnageValue>()
            {
                transferValuesTonnage1
            };

            var approvedNote = A.Fake<Note>();
            A.CallTo(() => approvedNote.Status).Returns(NoteStatus.Approved);
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage1.Received).Returns(5);
            A.CallTo(() => noteTransferTonnage1.TransferNote).Returns(approvedNote);
            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage2.Received).Returns(5);
            A.CallTo(() => noteTransferTonnage2.TransferNote).Returns(approvedNote);
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };
            
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => noteTonnage1.Received).Returns(11);
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { noteTonnage1 });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues));

            //assert
            exception.Should().BeNull();
        }

        [Fact]
        public async Task Validate_GivenRequestedTonnageIsNoLongerAvailable_InvalidOperationExceptionExpected()
        {
            var transferValueTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.TransferTonnageId).Returns(transferValueTonnageId1);
            A.CallTo(() => transferValuesTonnage1.FirstTonnage).Returns(1);
            var transferValues = new List<TransferTonnageValue>()
            {
                transferValuesTonnage1
            };

            var approvedNote = A.Fake<Note>();
            A.CallTo(() => approvedNote.Status).Returns(NoteStatus.Approved);
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage1.Received).Returns(5);
            A.CallTo(() => noteTransferTonnage1.TransferNote).Returns(approvedNote);
            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage2.Received).Returns(5);
            A.CallTo(() => noteTransferTonnage2.TransferNote).Returns(approvedNote);
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => noteTonnage1.Received).Returns(10);
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { noteTonnage1 });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task Validate_GivenRequestedTonnageIsAvailableAsNoteIsNoteTransferIsNotApproved_NoExceptionExpected(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Approved))
            {
                return;
            }

            var transferValueTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.TransferTonnageId).Returns(transferValueTonnageId1);
            A.CallTo(() => transferValuesTonnage1.FirstTonnage).Returns(1);
            var transferValues = new List<TransferTonnageValue>()
            {
                transferValuesTonnage1
            };

            var approvedNote = A.Fake<Note>();
            var notApprovedNote = A.Fake<Note>();
            A.CallTo(() => approvedNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => approvedNote.Status).Returns(status);
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage1.Received).Returns(5);
            A.CallTo(() => noteTransferTonnage1.TransferNote).Returns(approvedNote);
            var noteTransferTonnage2 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage2.Received).Returns(5);
            A.CallTo(() => noteTransferTonnage2.TransferNote).Returns(notApprovedNote);
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => noteTonnage1.Received).Returns(10);
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { noteTonnage1 });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues));

            //assert
            exception.Should().BeNull();
        }

        private List<TransferTonnageValue> TransferTonnageValues()
        {
            var transferValues = new List<TransferTonnageValue>()
            {
                fixture.Create<TransferTonnageValue>(),
                fixture.Create<TransferTonnageValue>()
            };
            return transferValues;
        }
    }
}
