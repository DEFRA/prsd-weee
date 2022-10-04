namespace EA.Weee.DataAccess.Tests.Integration.EvidenceNote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Evidence;
    using Domain.Lookup;
    using EA.Weee.DataAccess.Tests.Integration.Base;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class EvidenceDataAccessIntegration : EvidenceNoteBaseDataAccess
    {
        [Fact]
        public async Task Update_ShouldUpdateNote()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var noteTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, 2),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null)
                };

                var note = NoteCommon.CreateNote(database, 
                    null, 
                    null, 
                    null,
                    WasteType.HouseHold,
                    Protocol.Actual,
                    noteTonnages);

                context.Notes.Add(note);

                await database.WeeeContext.SaveChangesAsync();

                var organisation = Domain.Organisation.Organisation.CreateRegisteredCompany("new", "1234567", "trading");
                var updateScheme = new Domain.Scheme.Scheme(organisation);

                context.Organisations.Add(organisation);
                context.Schemes.Add(updateScheme);

                await context.SaveChangesAsync();

                var startDate = DateTime.Now;
                var endDate = DateTime.Now.AddDays(1);

                var findNote = await dataAccess.GetNoteById(note.Id);

                noteTonnages.ElementAt(0).UpdateValues(null, null);
                noteTonnages.ElementAt(1).UpdateValues(2, 1);

                await dataAccess.Update(findNote, organisation, startDate, endDate, WasteType.NonHouseHold, Protocol.SmwProtocol,
                    noteTonnages,
                    NoteStatus.Submitted, SystemTime.UtcNow);

                await context.SaveChangesAsync();

                var updatedNote = await dataAccess.GetNoteById(note.Id);

                updatedNote.Recipient.Should().Be(organisation);
                updatedNote.WasteType.Should().Be(WasteType.NonHouseHold);
                updatedNote.Protocol.Should().Be(Protocol.SmwProtocol);
                updatedNote.StartDate.Should().Be(startDate);
                updatedNote.EndDate.Should().Be(endDate);
                updatedNote.Status.Should().Be(NoteStatus.Submitted);
                updatedNote.NoteTonnage.ElementAt(0).Received.Should().Be(null);
                updatedNote.NoteTonnage.ElementAt(0).Reused.Should().Be(null);
                updatedNote.NoteTonnage.ElementAt(1).Received.Should().Be(2);
                updatedNote.NoteTonnage.ElementAt(1).Reused.Should().Be(1);
            }
        }

        [Fact]
        public async Task GetComplianceYearsForNotes_WithNotesDraftStatusAndStatusNotAccepted_EmptyListShouldBeReturn()
        {
            // arrange
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var draftNote1 = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year);
                var draftNote2 = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year - 1);
                var draftNote3 = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year - 2);
                
                var alreadyExistingNotes = GetExistingNotesInDb(context);

                var listOfExistingComplianceYears = alreadyExistingNotes?.Result.Select(x => x.ComplianceYear).Distinct().ToList();

                // act
                context.Notes.Add(draftNote1);
                context.Notes.Add(draftNote2);
                context.Notes.Add(draftNote3);

                await context.SaveChangesAsync();

                var actionList = await dataAccess.GetComplianceYearsForNotes(new List<int> { 2, 3, 4, 5, 6 });

                // assert
                var result = actionList.Except(listOfExistingComplianceYears).ToList();

                result.Should().BeEmpty();
                result.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetRecipientOrganisations_GivenOrganisationIdAndComplianceYear_RecipientOrganisationsShouldBeReturned()
        {
            // arrange
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation1, SystemTime.UtcNow.Year, false,
                    false);
                var recipientOrganisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation1);
                var recipientOrganisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation2);

                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var aatfNoMatch = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation2, SystemTime.UtcNow.Year, false,
                    false);

                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var note1Match = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, aatf: aatf, organisation: organisation1, recipientOrganisation: recipientOrganisation1);
                var note2NoMatchCy = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year - 1, aatf: aatf, organisation: organisation1);
                var note3NoMatchCy = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year - 2, aatf: aatf, organisation: organisation1);
                var note4NoMatchOrganisation = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, aatf: aatfNoMatch, organisation: organisation2);
                var note5Match = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, aatf: aatf, organisation: organisation1, recipientOrganisation: recipientOrganisation2);

                // act
                context.Schemes.Add(scheme1);
                context.Schemes.Add(scheme2);

                await context.SaveChangesAsync();

                context.Notes.Add(note1Match);
                context.Notes.Add(note2NoMatchCy);
                context.Notes.Add(note3NoMatchCy);
                context.Notes.Add(note4NoMatchOrganisation);
                context.Notes.Add(note5Match);

                await context.SaveChangesAsync();

                var recipientList = await dataAccess.GetRecipientOrganisations(aatf.Id, SystemTime.UtcNow.Year);

                // assert
                recipientList.Count.Should().Be(2);
                recipientList.Should().Contain(r => r.Id == recipientOrganisation1.Id);
                recipientList.Should().Contain(r => r.Id == recipientOrganisation2.Id);
            }
        }

        [Fact]
        public async Task GetRecipientOrganisations_GivenComplianceYear_RecipientOrganisationsShouldBeReturned()
        {
            // arrange
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation1);
                var recipientOrganisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation2);

                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var note1Match = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation1);
                var note2NoMatchCy = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year - 1, organisation: organisation1);
                var note3NoMatchCy = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year - 2, organisation: organisation1);
                var note4Match = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation2);
                var note5Match = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation2);

                // act
                context.Schemes.Add(scheme1);
                context.Schemes.Add(scheme2);

                await context.SaveChangesAsync();

                context.Notes.Add(note1Match);
                context.Notes.Add(note2NoMatchCy);
                context.Notes.Add(note3NoMatchCy);
                context.Notes.Add(note4Match);
                context.Notes.Add(note5Match);

                await context.SaveChangesAsync();

                var recipientList = await dataAccess.GetRecipientOrganisations(null, SystemTime.UtcNow.Year);

                // assert
                recipientList.Count.Should().BeGreaterOrEqualTo(3);
                recipientList.Should().Contain(r => r.Id == recipientOrganisation1.Id);
                recipientList.Should().Contain(r => r.Id == recipientOrganisation2.Id);
                recipientList.Should().Contain(r => r.Id == note4Match.Recipient.Id);
            }
        }

        [Fact]
        public async Task HasApprovedWasteHouseHoldEvidence_GivenApprovedWasteEvidenceNotes_ShouldReturnTrue()
        {
            // arrange
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation1);

                var note1Match = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation1, wasteType: WasteType.HouseHold);
                note1Match.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), SystemTime.Now);
                note1Match.UpdateStatus(NoteStatus.Approved, userContext.UserId.ToString(), SystemTime.Now);

                // act
                context.Schemes.Add(scheme1);
                
                await context.SaveChangesAsync();

                context.Notes.Add(note1Match);

                await context.SaveChangesAsync();

                var hasApprovedWaste = await dataAccess.HasApprovedWasteHouseHoldEvidence(recipientOrganisation1.Id, SystemTime.UtcNow.Year);

                // assert
                hasApprovedWaste.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HasApprovedWasteHouseHoldEvidence_GivenIncorrectComplianceYear_ShouldReturnFalse()
        {
            // arrange
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation1);

                var note1Match = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year + 1, organisation: organisation1, recipientOrganisation: recipientOrganisation1, wasteType: WasteType.HouseHold);
                note1Match.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), SystemTime.Now);
                note1Match.UpdateStatus(NoteStatus.Approved, userContext.UserId.ToString(), SystemTime.Now);

                // act
                context.Schemes.Add(scheme1);

                await context.SaveChangesAsync();

                context.Notes.Add(note1Match);

                await context.SaveChangesAsync();

                var hasApprovedWaste = await dataAccess.HasApprovedWasteHouseHoldEvidence(organisation1.Id, SystemTime.UtcNow.Year);

                // assert
                hasApprovedWaste.Should().BeFalse();
            }
        }

        [Fact]
        public async Task HasApprovedWasteHouseHoldEvidence_GivenIncorrectWasteTypeEvidenceNotes_ShouldReturnFalse()
        {
            // arrange
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation1);

                var note1Match = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation1, wasteType: WasteType.NonHouseHold);
                note1Match.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), SystemTime.Now);
                note1Match.UpdateStatus(NoteStatus.Approved, userContext.UserId.ToString(), SystemTime.Now);

                // act
                context.Schemes.Add(scheme1);

                await context.SaveChangesAsync();

                context.Notes.Add(note1Match);

                await context.SaveChangesAsync();

                var hasApprovedWaste = await dataAccess.HasApprovedWasteHouseHoldEvidence(organisation1.Id, SystemTime.UtcNow.Year);

                // assert
                hasApprovedWaste.Should().BeFalse();
            }
        }

        [Fact]
        public async Task HasApprovedWasteHouseHoldEvidence_GivenIncorrectStatusEvidenceNotes_ShouldReturnFalse()
        {
            // arrange
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme1 = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation1);

                var note1NoMatch = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation1, wasteType: WasteType.HouseHold);
                note1NoMatch.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), SystemTime.Now);

                var note2NoMatch = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation1, wasteType: WasteType.HouseHold);
                note2NoMatch.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), SystemTime.Now);
                note2NoMatch.UpdateStatus(NoteStatus.Rejected, userContext.UserId.ToString(), SystemTime.Now);

                var note3NoMatch = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation1, wasteType: WasteType.HouseHold);
                note3NoMatch.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), SystemTime.Now);
                note3NoMatch.UpdateStatus(NoteStatus.Returned, userContext.UserId.ToString(), SystemTime.Now);

                var note4NoMatch = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation1, wasteType: WasteType.HouseHold);
                note4NoMatch.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), SystemTime.Now);
                note4NoMatch.UpdateStatus(NoteStatus.Rejected, userContext.UserId.ToString(), SystemTime.Now);

                var note5NoMatch = NoteCommon.CreateNote(database, complianceYear: SystemTime.UtcNow.Year, organisation: organisation1, recipientOrganisation: recipientOrganisation1, wasteType: WasteType.HouseHold);
                note5NoMatch.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString(), SystemTime.Now);
                note5NoMatch.UpdateStatus(NoteStatus.Approved, userContext.UserId.ToString(), SystemTime.Now);
                note5NoMatch.UpdateStatus(NoteStatus.Void, userContext.UserId.ToString(), SystemTime.Now);

                // act
                context.Schemes.Add(scheme1);

                await context.SaveChangesAsync();

                context.Notes.Add(note1NoMatch);
                context.Notes.Add(note2NoMatch);
                context.Notes.Add(note3NoMatch);
                context.Notes.Add(note4NoMatch);
                context.Notes.Add(note5NoMatch);

                await context.SaveChangesAsync();

                var hasApprovedWaste = await dataAccess.HasApprovedWasteHouseHoldEvidence(organisation1.Id, SystemTime.UtcNow.Year);

                // assert
                hasApprovedWaste.Should().BeFalse();
            }
        }
    }
}