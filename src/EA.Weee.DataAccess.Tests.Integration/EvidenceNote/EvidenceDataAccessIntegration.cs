namespace EA.Weee.DataAccess.Tests.Integration.EvidenceNote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Evidence;
    using Domain.Lookup;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class EvidenceDataAccessIntegration
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
        public async Task GetComplianceYearsForNotes_ForAllowedStatuses_ShouldReturnAListOfComplianceYears()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var note1 = NoteCommon.CreateNote(database, complianceYear: SystemTime.Now.Year);
                note1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.UtcNow);
                
                var note2 = NoteCommon.CreateNote(database, complianceYear: SystemTime.Now.Year - 1);
                note1.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.UtcNow);

                var note3 = NoteCommon.CreateNote(database, complianceYear: SystemTime.Now.Year - 2);
                note1.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser(), SystemTime.UtcNow);

                var note4 = NoteCommon.CreateNote(database, complianceYear: SystemTime.Now.Year - 3);
                note1.UpdateStatus(NoteStatus.Returned, context.GetCurrentUser(), SystemTime.UtcNow);

                var note5 = NoteCommon.CreateNote(database, complianceYear: SystemTime.Now.Year - 4);
                note1.UpdateStatus(NoteStatus.Void, context.GetCurrentUser(), SystemTime.UtcNow);

                context.Notes.Add(note1);
                context.Notes.Add(note2);
                context.Notes.Add(note3);
                context.Notes.Add(note4);
                context.Notes.Add(note5);

                await database.WeeeContext.SaveChangesAsync();

                var allowedStatuses = new List<int> { 2, 3, 4, 5, 6 };

                var complianceYearEnumerable = await dataAccess.GetComplianceYearsForNotes(allowedStatuses);

                var complianceYearList = complianceYearEnumerable.ToList();

                complianceYearList.Should().NotBeNull();
                complianceYearList[0].Should().Be(SystemTime.Now.Year);
                complianceYearList[1].Should().Be(SystemTime.Now.Year - 1);
                complianceYearList[2].Should().Be(SystemTime.Now.Year - 2);
                complianceYearList[3].Should().Be(SystemTime.Now.Year - 3);
                complianceYearList[4].Should().Be(SystemTime.Now.Year - 4);
            }
        }
    }
}
