namespace EA.Weee.DataAccess.Tests.Integration.EvidenceNote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Base;
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

    public class NotesDataAccessGetTonnageByIdsIntegration : EvidenceNoteBaseDataAccess
    {
        [Fact]
        public async Task GetTonnageByIds_GivenIds_ShouldReturnCorrectNotes()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                // to be found matching category, scheme and status
                var note1 = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, 1));
                note1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 2, 1));

                context.Notes.Add(note1);

                var note2 = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 8, null));
                note2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 10, null));

                context.Notes.Add(note2);

                var note3 = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note3.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, null, 2));

                context.Notes.Add(note3);

                await context.SaveChangesAsync();

                var noteTonnages = await dataAccess.GetTonnageByIds(new List<Guid>()
                {
                    note1.NoteTonnage.ElementAt(1).Id, 
                    note2.NoteTonnage.ElementAt(0).Id,
                    note3.NoteTonnage.ElementAt(0).Id
                });

                noteTonnages.Count().Should().Be(3);
                var noteTonnage1 = noteTonnages.First(nt => nt.Id.Equals(note1.NoteTonnage.ElementAt(1).Id));
                noteTonnage1.Should().NotBeNull();
                noteTonnage1.CategoryId.Should().Be(WeeeCategory.GasDischargeLampsAndLedLightSources);
                noteTonnage1.Received.Should().Be(2);
                noteTonnage1.Reused.Should().Be(1);
                noteTonnage1.Note.Should().NotBeNull();
                noteTonnage1.NoteTransferTonnage.Should().NotBeNull();
                noteTonnage1.NoteTransferTonnage.Select(nt => nt.TransferNote).Should().NotBeNull();

                var noteTonnage2 = noteTonnages.First(nt => nt.Id.Equals(note2.NoteTonnage.ElementAt(0).Id));
                noteTonnage2.CategoryId.Should().Be(WeeeCategory.DisplayEquipment);
                noteTonnage2.Received.Should().Be(8);
                noteTonnage2.Reused.Should().Be(null);
                noteTonnage2.Note.Should().NotBeNull();
                noteTonnage2.NoteTransferTonnage.Should().NotBeNull();
                noteTonnage2.NoteTransferTonnage.Select(nt => nt.TransferNote).Should().NotBeNull();

                var noteTonnage3 = noteTonnages.First(nt => nt.Id.Equals(note3.NoteTonnage.ElementAt(0).Id));
                noteTonnage3.CategoryId.Should().Be(WeeeCategory.DisplayEquipment);
                noteTonnage3.Received.Should().Be(null);
                noteTonnage3.Reused.Should().Be(2);
                noteTonnage3.Note.Should().NotBeNull();
                noteTonnage3.NoteTransferTonnage.Should().NotBeNull();
                noteTonnage3.NoteTransferTonnage.Select(nt => nt.TransferNote).Should().NotBeNull();
            }
        }
    }
}
