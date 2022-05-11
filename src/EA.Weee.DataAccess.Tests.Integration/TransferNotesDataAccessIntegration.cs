namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;

    public class TransferNotesDataAccessIntegration
    {
        [Fact]
        public async Task GetNotesToTransfer_GivenSchemeAndCategories_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                // to be found matching category, scheme and status
                var note1ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note1ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note1ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 2, null));

                context.Notes.Add(note1ToBeFound);

                // note not to be found has category but with not tonnage, matching scheme and status
                var note2ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note2ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note2ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note2ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note2ToNotBeFound);

                // note not to be found not matching scheme, matching status and category
                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation2);
                var note3ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme2);
                note3ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note3ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note3ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note3ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note4ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note4ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note4ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note4ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note4ToNotBeFound);

                // note not to be found not matching status, matching type and category
                var note5ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note5ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note5ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note5ToNotBeFound);

                // note not to be found not matching status, matching type and category
                var note6ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note6ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note6ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note7ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note7ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note7ToNotBeFound.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser());
                note7ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note7ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note8ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note8ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note8ToNotBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser());
                note8ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note8ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note9ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note9ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note9ToNotBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser());
                note9ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1, null));

                context.Notes.Add(note9ToNotBeFound);

                // to be found matching category, scheme and status
                var note10ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note10ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note10ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 1, null));
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 1, null));
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 1, null));

                context.Notes.Add(note10ToBeFound);

                // to not be found matching category but received is null and reused is not null, also with matching scheme and status
                var note11ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note11ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note11ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note11ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, null, 1));

                context.Notes.Add(note11ToNotBeFound);

                // to not be found as transfer note, matching scheme, category
                var note12ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, scheme);
                note12ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note12ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note12ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 1,  null));

                context.Notes.Add(note12ToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt(),
                    WeeeCategory.MedicalDevices.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(scheme.Id, categorySearch, new List<Guid>());

                notes.Count().Should().Be(2);
                notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Should().Contain(n => n.Id.Equals(note10ToBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note2ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note3ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note4ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note5ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note6ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note7ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note8ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note9ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note11ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note12ToNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenEvidenceNotes_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                // to be found matching category, scheme and status and id is requested
                var note1ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note1ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note1ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note1ToBeFound);

                // to be found matching category, scheme and status and id is requested
                var note2ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note2ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note2ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note2ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note2ToBeFound);

                // note not to be found matching scheme, status but not in evidence note list
                var note3NotToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note3NotToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser());
                note3NotToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser());
                note3NotToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note3NotToBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(scheme.Id, categorySearch, new List<Guid>()
                {
                    note1ToBeFound.Id,
                    note2ToBeFound.Id
                });

                notes.Count().Should().Be(2);
                notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Should().Contain(n => n.Id.Equals(note2ToBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note3NotToBeFound.Id));
            }
        }

        private async Task<Note> SetupSingleNote(WeeeContext context, 
            DatabaseWrapper database, 
            NoteType noteType = null,
            EA.Weee.Domain.Scheme.Scheme scheme = null)
        {
            var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();

            context.Organisations.Add(organisation);

            var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

            context.Aatfs.Add(aatf1);

            await database.WeeeContext.SaveChangesAsync();

            if (noteType == null)
            {
                noteType = NoteType.EvidenceNote;
            }

            if (scheme == null)
            {
                scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);
            }

            var note1 = NoteCommon.CreateNote(database, organisation, scheme, aatf1, noteType: noteType);

            context.Notes.Add(note1);

            await database.WeeeContext.SaveChangesAsync();
            return note1;
        }
    }
}
