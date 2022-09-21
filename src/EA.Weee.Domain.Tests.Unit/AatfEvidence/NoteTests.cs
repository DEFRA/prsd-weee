﻿namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.Helpers;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Domain.Scheme;
    using Evidence;
    using FakeItEasy;
    using FluentAssertions;
    using Lookup;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Xunit;

    public class NoteTests : SimpleUnitTestBase
    {
        private Organisation organisation;
        private Organisation recipientOrganisation;
        private DateTime startDate;
        private DateTime endDate;
        private WasteType wasteType;
        private Protocol protocol;
        private Aatf aatf;
        private string createdBy;
        private IEnumerable<NoteTonnage> tonnages;
        private readonly IEnumerable<NoteTransferTonnage> transferTonnages;
        private NoteStatus status;
        private readonly short complianceYear;

        public NoteTests()
        {
            organisation = A.Fake<Organisation>();
            recipientOrganisation = A.Fake<Organisation>();
            startDate = DateTime.Now.AddDays(1);
            endDate = DateTime.Now.AddDays(2);
            wasteType = TestFixture.Create<WasteType>();
            protocol = TestFixture.Create<Protocol>();
            aatf = A.Fake<Aatf>();
            createdBy = TestFixture.Create<string>();
            tonnages = TestFixture.CreateMany<NoteTonnage>();
            transferTonnages = TestFixture.CreateMany<NoteTransferTonnage>();
            status = NoteStatus.Draft;
            complianceYear = TestFixture.Create<short>();
        }

        [Fact]
        public void Note_Constructor_GivenNullOrganisationArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(null, A.Fake<Organisation>(),
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                A.Fake<Aatf>(),
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferNote_Constructor_GivenNullOrganisationArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(null, A.Fake<Organisation>(),
                "created",
                transferTonnages.ToList(),
                complianceYear));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Note_Constructor_GivenNullSchemeArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), null,
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                A.Fake<Aatf>(),
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferNote_Constructor_GivenNullSchemeArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), null,
                "created",
                transferTonnages.ToList(),
                complianceYear));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Note_Constructor_GivenDefaultStartDateArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Organisation>(),
                DateTime.MinValue,
                DateTime.Now,
                null,
                null,
                A.Fake<Aatf>(),
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Note_Constructor_GivenDefaultEndDateArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Organisation>(),
                DateTime.Now,
                DateTime.MinValue,
                null,
                null,
                A.Fake<Aatf>(),
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentException>();
        }
        
        [Fact]
        public void Note_Constructor_GivenNullAatfArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Organisation>(),
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                null,
                "created",
                new List<NoteTonnage>()));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Note_Constructor_GivenNullTonnagesArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Organisation>(),
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                null,
                "created",
                null));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferNote_Constructor_GivenNullTransferTonnagesArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Organisation>(),
                "created",
                null,
                complianceYear));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferNote_Constructor_GivenNullTransferCategoriesArgumentNullExceptionExpected()
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Organisation>(),
                "created",
                null,
                complianceYear));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void TransferNote_Constructor_GivenInvalidComplianceYearArgumentExceptionExpected(short complianceYear)
        {
            var result = Record.Exception(() => new Note(A.Fake<Organisation>(), A.Fake<Organisation>(),
                "created",
                transferTonnages.ToList(),
                complianceYear));

            result.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Note_Constructor_GivenDraftEvidenceNoteValues_PropertiesShouldBeSet()
        {
            var date = DateTime.Now;
            SystemTime.Freeze(date);
            status = NoteStatus.Draft;

            var result = CreateNote();

            ShouldBeEqualTo(result, date);
            SystemTime.Unfreeze();
        }

        [Fact]
        public void Note_Constructor_GivenDraftTransferEvidenceNoteValues_PropertiesShouldBeSet()
        {
            var date = TestFixture.Create<DateTime>();
            SystemTime.Freeze(date);
            
            var result = CreateTransferNote();

            TransferNoteShouldBeEqualTo(result, date);
            SystemTime.Unfreeze();
        }
        
        [Fact]
        public void UpdateStatus_GivenDraftToSubmittedStatusUpdate_StatusShouldBeUpdated()
        {
            //arrange
            var date = new DateTime(2022, 4, 1);

            var note = CreateNote();
            
            //act
            note.UpdateStatus(NoteStatus.Submitted, "user", date);

            //asset
            note.Status.Should().Be(NoteStatus.Submitted);
        }

        [Fact]
        public void UpdateStatus_GivenSubmittedToReturnedStatusUpdate_StatusShouldBeUpdated()
        {
            //arrange
            var date = new DateTime(2022, 4, 1);
            var note = CreateNote();

            //act
            note.UpdateStatus(NoteStatus.Submitted, "user", date);
            note.UpdateStatus(NoteStatus.Returned, "user", date);

            //asset
            note.Status.Should().Be(NoteStatus.Returned);
        }

        [Fact]
        public void UpdateStatus_GivenReturnedToSubmittedStatusUpdate_StatusShouldBeUpdated()
        {
            //arrange
            var date = new DateTime(2022, 4, 1);
            var note = CreateNote();

            //act
            note.UpdateStatus(NoteStatus.Submitted, "user", date);
            note.UpdateStatus(NoteStatus.Approved, "user", date);
            note.UpdateStatus(NoteStatus.Returned, "user", date);
            note.UpdateStatus(NoteStatus.Submitted, "user", date);

            //asset
            note.Status.Should().Be(NoteStatus.Submitted);
        }

        [Fact]
        public void UpdateStatus_GivenDraftToSubmittedStatusUpdate_ShouldAddStatusHistory()
        {
            //arrange
            var date = new DateTime(2022, 4, 1);
            var note = CreateNote();

            //act
            note.UpdateStatus(NoteStatus.Submitted, "user", date);

            //asset
            note.NoteStatusHistory.Count.Should().Be(1);
            note.NoteStatusHistory.ElementAt(0).ChangedById.Should().Be("user");
            note.NoteStatusHistory.ElementAt(0).FromStatus.Should().Be(NoteStatus.Draft);
            note.NoteStatusHistory.ElementAt(0).ToStatus.Should().Be(NoteStatus.Submitted);
            note.NoteStatusHistory.ElementAt(0).ChangedDate.Should().Be(date);
        }

        [Fact]
        public void UpdateStatus_GivenReturnedToSubmittedStatusUpdate_ShouldAddStatusHistory()
        {
            //arrange
            var date = new DateTime(2022, 4, 1);
            var note = CreateNote();

            //act
            note.UpdateStatus(NoteStatus.Submitted, "user", date);
            note.UpdateStatus(NoteStatus.Returned, "user", date);
            note.UpdateStatus(NoteStatus.Submitted, "user", date);

            //asset
            note.NoteStatusHistory.Count.Should().Be(3);
            note.NoteStatusHistory.ElementAt(2).ChangedById.Should().Be("user");
            note.NoteStatusHistory.ElementAt(2).FromStatus.Should().Be(NoteStatus.Returned);
            note.NoteStatusHistory.ElementAt(2).ToStatus.Should().Be(NoteStatus.Submitted);
            note.NoteStatusHistory.ElementAt(2).ChangedDate.Should().Be(date);
        }

        [Fact]
        public void UpdateStatus_GivenSubmittedToReturnedStatusUpdate_ShouldAddStatusHistory()
        {
            //arrange
            var date = new DateTime(2022, 4, 1);
            var note = CreateNote();

            //act
            note.UpdateStatus(NoteStatus.Submitted, "user", date);
            note.UpdateStatus(NoteStatus.Returned, "user", date);

            //asset
            note.NoteStatusHistory.Count.Should().Be(2);
            note.NoteStatusHistory.ElementAt(1).ChangedById.Should().Be("user");
            note.NoteStatusHistory.ElementAt(1).FromStatus.Should().Be(NoteStatus.Submitted);
            note.NoteStatusHistory.ElementAt(1).ToStatus.Should().Be(NoteStatus.Returned);
            note.NoteStatusHistory.ElementAt(1).ChangedDate.Should().Be(date);
        }

        [Fact]
        public void UpdateStatus_GivenDraftStatusToDraftStatusUpdate_InvalidOperationExpected()
        {
            //arrange
            var note = CreateNote();

            //act
            var result = Record.Exception(() => note.UpdateStatus(NoteStatus.Draft, "user", TestFixture.Create<DateTime>()));

            //asset
            result.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void UpdateStatus_GivenSubmittedStatusToSubmittedStatusUpdate_InvalidOperationExpected()
        {
            //arrange
            var note = CreateNote();
            note.UpdateStatus(NoteStatus.Submitted, "user", TestFixture.Create<DateTime>());

            //act
            var result = Record.Exception(() => note.UpdateStatus(NoteStatus.Submitted, "user", TestFixture.Create<DateTime>()));

            //asset
            result.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Update_GivenNullSchemeArgumentNullExceptionExpected()
        {
            //arrange
            var note = CreateNote();

            //act
            var result = Record.Exception(() =>
                note.Update(null, DateTime.Now, DateTime.Now, WasteType.HouseHold, Protocol.SiteSpecificProtocol));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Update_GivenDefaultStartDateArgumentExceptionExpected()
        {
            //arrange
            var note = CreateNote();

            //act
            var result = Record.Exception(() =>
                note.Update(A.Fake<Organisation>(), DateTime.MinValue, DateTime.Now, WasteType.HouseHold, Protocol.SiteSpecificProtocol));

            //assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Update_GivenDefaultEndDateArgumentExceptionExpected()
        {
            //arrange
            var note = CreateNote();

            //act
            var result = Record.Exception(() =>
                note.Update(A.Fake<Organisation>(), DateTime.Now, DateTime.MinValue, WasteType.HouseHold, Protocol.SiteSpecificProtocol));

            //assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Update_GivenUpdateValue_NoteShouldBeUpdated()
        {
            //arrange
            organisation = A.Fake<Organisation>();
            recipientOrganisation = A.Fake<Organisation>();
            startDate = DateTime.Now.AddDays(1);
            endDate = DateTime.Now.AddDays(2);
            wasteType = WasteType.HouseHold;
            protocol = Protocol.LdaProtocol;
            aatf = A.Fake<Aatf>();
            createdBy = TestFixture.Create<string>();
            tonnages = TestFixture.CreateMany<NoteTonnage>();
            status = NoteStatus.Draft;

            var note = new Note(organisation, recipientOrganisation, startDate, endDate, wasteType, protocol, aatf, createdBy, tonnages.ToList());

            var updatedScheme = A.Fake<Organisation>();
            var updatedStartDate = DateTime.Now;
            var updatedEndDate = DateTime.Now.AddDays(4);
            var updatedWasteType = WasteType.NonHouseHold;
            var updatedProtocol = Protocol.Actual;

            //act
            note.Update(updatedScheme, updatedStartDate, updatedEndDate, updatedWasteType, updatedProtocol);

            //assert
            note.Recipient.Should().Be(updatedScheme);
            note.StartDate.Should().Be(updatedStartDate);
            note.EndDate.Should().Be(updatedEndDate);
            note.WasteType.Should().Be(updatedWasteType);
            note.Protocol.Should().Be(updatedProtocol);
        }

        [Fact]
        public void FilteredNoteTonnage_GivenReceivedValues_FilteredTonnageShouldBeAsExpected()
        {
            //arrange
            var note = CreateNote();
            note.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 2, 1));
            note.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 3, 1));
            note.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, null, 1));
            note.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, 1));

            //act
            var tonnage = note.FilteredNoteTonnage(new List<int>() { WeeeCategory.ITAndTelecommsEquipment.ToInt(), WeeeCategory.ElectricalAndElectronicTools.ToInt() });

            //assert
            tonnage.Count.Should().Be(2);
            tonnage.Should().Contain(t => t.CategoryId == WeeeCategory.ITAndTelecommsEquipment && t.Received == 2);
            tonnage.Should().Contain(t => t.CategoryId == WeeeCategory.ElectricalAndElectronicTools && t.Received == 3);
            tonnage.Should().NotContain(t => t.Received == null);
            tonnage.Should().NotContain(t => t.CategoryId == WeeeCategory.LightingEquipment);
            tonnage.Should().NotContain(t => t.CategoryId == WeeeCategory.ConsumerEquipment);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void SetApprovedRecipientAddress_GivenEmptyAddress_ArgumentExceptionExpected(string address)
        {
            //arrange
            var note = CreateNote();

            //act
            var exception = Record.Exception(() => note.SetApprovedRecipientAddress("name", address));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void SetApprovedRecipientAddress_GivenEmptyName_ArgumentExceptionExpected(string name)
        {
            //arrange
            var note = CreateNote();

            //act
            var exception = Record.Exception(() => note.SetApprovedRecipientAddress(name, "address"));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void SetApprovedRecipientAddress_GivenNullName_ArgumentNullExceptionExpected()
        {
            //arrange
            var note = CreateNote();

            //act
            var exception = Record.Exception(() => note.SetApprovedRecipientAddress(null, "address"));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void SetApprovedRecipientAddress_GivenNullAddress_ArgumentNullExceptionExpected()
        {
            //arrange
            var note = CreateNote();

            //act
            var exception = Record.Exception(() => note.SetApprovedRecipientAddress("name", null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void SetApprovedRecipientAddress_GivenNameAndAddress_ApprovedDetailsShouldBeSet()
        {
            //arrange
            var note = CreateNote();
            const string address = "address";
            const string schemeName = "name";

            //act
            note.SetApprovedRecipientAddress(schemeName, address);

            //assert
            note.ApprovedRecipientSchemeName.Should().Be(schemeName);
            note.ApprovedRecipientAddress.Should().Be(address);
        }

        private void ShouldBeEqualTo(Note result, DateTime date)
        {
            result.Organisation.Should().Be(organisation);
            result.Recipient.Should().Be(recipientOrganisation);
            result.Aatf.Should().Be(aatf);
            result.WasteType.Should().Be(wasteType);
            result.Protocol.Should().Be(protocol);
            result.StartDate.Should().Be(startDate);
            result.EndDate.Should().Be(endDate);
            result.NoteTonnage.Should().BeEquivalentTo(tonnages);
            result.NoteStatusHistory.Should().BeEmpty();
            result.CreatedDate.Should().Be(date);
            result.CreatedById.Should().Be(createdBy);
            result.NoteType.Should().Be(NoteType.EvidenceNote);
            result.Status.Should().Be(status);
            result.ComplianceYear.Should().Be(startDate.Year);
        }

        private void TransferNoteShouldBeEqualTo(Note result, DateTime date)
        {
            result.Organisation.Should().Be(organisation);
            result.Recipient.Should().Be(recipientOrganisation);
            result.Aatf.Should().BeNull();
            result.WasteType.Should().Be(WasteType.HouseHold);
            result.Protocol.Should().BeNull();
            result.StartDate.Should().Be(date);
            result.EndDate.Should().Be(date);
            result.NoteTonnage.Should().BeEmpty();
            result.NoteStatusHistory.Should().BeEmpty();
            result.NoteTransferTonnage.Should().BeEquivalentTo(transferTonnages);
            result.CreatedDate.Should().Be(date);
            result.CreatedById.Should().Be(createdBy);
            result.NoteType.Should().Be(NoteType.TransferNote);
            result.Status.Should().Be(status);
        }

        public Note CreateNote()
        {
            return new Note(organisation,
                recipientOrganisation,
                startDate,
                endDate,
                wasteType,
                protocol,
                aatf,
                createdBy,
                tonnages.ToList());
        }

        public Note CreateTransferNote()
        {
            return new Note(organisation,
                recipientOrganisation,
                createdBy,
                transferTonnages.ToList(),
                complianceYear);
        }
    }
}
