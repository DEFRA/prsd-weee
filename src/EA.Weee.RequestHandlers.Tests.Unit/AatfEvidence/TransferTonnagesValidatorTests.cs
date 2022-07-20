namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Lookup;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfEvidence;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class TransferTonnagesValidatorTests : SimpleUnitTestBase
    {
        private readonly TransferTonnagesValidator validator;
        private readonly IEvidenceDataAccess evidenceDataAccess;

        public TransferTonnagesValidatorTests()
        {
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
                A<List<Guid>>.That.Matches(m => m.SequenceEqual(transferValues.Select(t => t.Id).ToList())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Validate_GivenAnyExistingTonnageHasNullReceived_InvalidOperationExceptionExpected()
        {
            //arrange
            var transferValues = TransferTonnageValues();

            var tonnage = new NoteTonnage(TestFixture.Create<WeeeCategory>(), null, 1);
            
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
            var noteTonnageId = Guid.NewGuid();
            var transferTonnageValue1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferTonnageValue1.Id).Returns(noteTonnageId);
            A.CallTo(() => transferTonnageValue1.FirstTonnage).Returns(10);

            var transferValues = new List<TransferTonnageValue>()
            {
                transferTonnageValue1
            };

            var noteTonnage = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage.Id).Returns(noteTonnageId);
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
            var noteTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.Id).Returns(noteTonnageId1);
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
            A.CallTo(() => noteTonnage1.Id).Returns(noteTonnageId1);
            A.CallTo(() => noteTonnage1.Received).Returns(11);
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { noteTonnage1 });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues));

            //assert
            exception.Should().BeNull();
        }

        /// <summary>
        /// Test checks that when a new transfer is made that when tonnage is no longer still available to be transferred an exception is thrown
        /// </summary>
        [Fact]
        public async Task Validate_GivenRequestedTonnageIsNoLongerAvailable_InvalidOperationExceptionExpected()
        {
            //create the selected transfer tonnages
            var transferValueTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => transferValuesTonnage1.FirstTonnage).Returns(1);
            var transferValues = new List<TransferTonnageValue>()
            {
                transferValuesTonnage1
            };

            var noteTransferTonnage1 = CreateNoteWithTonnage(5, NoteStatus.Approved);
            var noteTransferTonnage2 = CreateNoteWithTonnage(5, NoteStatus.Approved);

            // create the transfers that have already happened
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            // setup the tonnages for the new transfer
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

        /// <summary>
        /// Test checks that when a new transfer is made that when tonnage is available as notes have been rejected or voided so no exception should be thrown
        /// </summary>
        [Fact]
        public async Task Validate_GivenRequestedTonnageIsOnNotesWithRejectedOrVoidStatusAndIsAvailable_NoExceptionExpected()
        {
            //create the selected transfer tonnages
            var transferValueTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => transferValuesTonnage1.FirstTonnage).Returns(10);
            var transferValues = new List<TransferTonnageValue>()
            {
                transferValuesTonnage1
            };

            var noteTransferTonnage1 = CreateNoteWithTonnage(5, NoteStatus.Rejected);
            var noteTransferTonnage2 = CreateNoteWithTonnage(5, NoteStatus.Void);

            // create the transfers that have already happened
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            // setup the tonnages for the new transfer
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

        /// <summary>
        /// Test checks that when a existing transfer note is being updated and that when tonnage is no longer still available to be transferred an exception is thrown
        /// </summary>
        [Fact]
        public async Task Validate_GivenUpdatingATransferNoteAndRequestedTonnageIsNoLongerAvailable_InvalidOperationExceptionExpected()
        {
            var transferNoteId = TestFixture.Create<Guid>();

            //create the selected transfer tonnages
            var transferValueTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => transferValuesTonnage1.FirstTonnage).Returns(6);
            var transferValues = new List<TransferTonnageValue>()
            {
                transferValuesTonnage1
            };

            var noteTransferTonnage1 = CreateNoteWithTonnage(5, NoteStatus.Approved);
            var transferNoteBeingUpdatedTonnage = CreateNoteWithTonnage(5, NoteStatus.Approved, transferNoteId);

            // create the transfers that have already happened
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                transferNoteBeingUpdatedTonnage
            };

            // setup the tonnages for the new transfer
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => noteTonnage1.Received).Returns(10);
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { noteTonnage1 });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues, transferNoteId));

            //assert
            exception.Should().BeOfType<InvalidOperationException>();
        }

        /// <summary>
        /// Test checks that when a existing transfer note is being updated and that when tonnage is still available to be transferred no exception is thrown
        /// </summary>
        [Fact]
        public async Task Validate_GivenUpdatingATransferNoteAndRequestedTonnageIsAvailable_NoExceptionExpected()
        {
            var transferNoteId = TestFixture.Create<Guid>();

            //create the selected transfer tonnages
            var transferValueTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => transferValuesTonnage1.FirstTonnage).Returns(5);
            var transferValues = new List<TransferTonnageValue>()
            {
                transferValuesTonnage1
            };

            var noteTransferTonnage1 = CreateNoteWithTonnage(5, NoteStatus.Approved);
            var transferNoteBeingUpdatedTonnage = CreateNoteWithTonnage(5, NoteStatus.Approved, transferNoteId);

            // create the transfers that have already happened
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                transferNoteBeingUpdatedTonnage
            };

            // setup the tonnages for the new transfer
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.Id).Returns(transferValueTonnageId1);
            A.CallTo(() => noteTonnage1.Received).Returns(10);
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnages);
            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { noteTonnage1 });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues, transferNoteId));

            //assert
            exception.Should().BeNull();
        }

        /// <summary>
        /// Test checks that when a new transfer is made that when tonnage is still available to be transferred no exception is thrown
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task Validate_GivenRequestedTonnageIsAvailableAsNoteIsNoteTransferIsNotExcludedStatus_NoExceptionExpected(NoteStatus status)
        {
            var excludedStatuses = new List<NoteStatus>() { NoteStatus.Rejected, NoteStatus.Void };
            if (!excludedStatuses.Contains(status))
            {
                return;
            }

            //create the selected transfer tonnages
            var noteTonnageId1 = Guid.NewGuid();
            var transferValuesTonnage1 = A.Fake<TransferTonnageValue>();
            A.CallTo(() => transferValuesTonnage1.Id).Returns(noteTonnageId1);
            A.CallTo(() => transferValuesTonnage1.FirstTonnage).Returns(1);
            var transferValues = new List<TransferTonnageValue>()
            {
                transferValuesTonnage1
            };

            var noteTransferTonnage1 = CreateNoteWithTonnage(5, NoteStatus.Approved);
            var noteTransferTonnage2 = CreateNoteWithTonnage(5, status);

            // create the transfers that have already happened
            var noteTransferTonnages = new List<NoteTransferTonnage>()
            {
                noteTransferTonnage1,
                noteTransferTonnage2
            };

            // setup the tonnages for the new transfer
            var noteTonnage1 = A.Fake<NoteTonnage>();
            A.CallTo(() => noteTonnage1.Id).Returns(noteTonnageId1);
            A.CallTo(() => noteTonnage1.Received).Returns(10);
            A.CallTo(() => noteTonnage1.NoteTransferTonnage).Returns(noteTransferTonnages);

            A.CallTo(() => evidenceDataAccess.GetTonnageByIds(A<List<Guid>>._))
                .Returns(new List<NoteTonnage>() { noteTonnage1 });

            //act
            var exception = await Record.ExceptionAsync(async () => await validator.Validate(transferValues));

            //assert
            exception.Should().BeNull();
        }

        private static NoteTransferTonnage CreateNoteWithTonnage(int tonnage, NoteStatus status, Guid? transferNoteId = null)
        {
            var note = A.Fake<Note>();
            A.CallTo(() => note.Status).Returns(status);
            var noteTransferTonnage1 = A.Fake<NoteTransferTonnage>();
            A.CallTo(() => noteTransferTonnage1.Received).Returns(tonnage);
            A.CallTo(() => noteTransferTonnage1.TransferNote).Returns(note);
            if (transferNoteId.HasValue)
            {
                A.CallTo(() => noteTransferTonnage1.TransferNoteId).Returns(transferNoteId.Value);
            }
            return noteTransferTonnage1;
        }

        private List<TransferTonnageValue> TransferTonnageValues()
        {
            var transferValues = new List<TransferTonnageValue>()
            {
                TestFixture.Create<TransferTonnageValue>(),
                TestFixture.Create<TransferTonnageValue>()
            };
            return transferValues;
        }
    }
}
