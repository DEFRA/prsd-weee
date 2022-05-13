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
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>());

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                // to be found matching category, scheme and status
                var note1 = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, 1));
                note1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 2, 1));

                context.Notes.Add(note1);

                var note2 = await SetupSingleNote(context, database, NoteType.EvidenceNote, scheme);
                note2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 8, null));
                note2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 10, null));

                context.Notes.Add(note2);

                await context.SaveChangesAsync();

                var noteTonnages = await dataAccess.GetTonnageByIds(new List<Guid>() { note1.NoteTonnage.ElementAt(1).Id, note2.NoteTonnage.ElementAt(0).Id });

                noteTonnages.Count().Should().Be(2);
                var noteTonnage = noteTonnages.First(nt => nt.Id.Equals(note1.NoteTonnage.ElementAt(1).Id));
                noteTonnage.Should().NotBeNull();
                noteTonnage.CategoryId.Should().Be(WeeeCategory.GasDischargeLampsAndLedLightSources);
                noteTonnage.Received.Should().Be(2);
                noteTonnage.Reused.Should().Be(1);
                noteTonnage.Note.Should().NotBeNull();
                noteTonnage.NoteTransferTonnage.Should().NotBeNull();
                noteTonnage.NoteTransferTonnage.Select(nt => nt.TransferNote).Should().NotBeNull();

                noteTonnage = noteTonnages.First(nt => nt.Id.Equals(note2.NoteTonnage.ElementAt(0).Id));
                noteTonnage.CategoryId.Should().Be(WeeeCategory.DisplayEquipment);
                noteTonnage.Received.Should().Be(8);
                noteTonnage.Reused.Should().Be(null);
                noteTonnage.Note.Should().NotBeNull();
                noteTonnage.NoteTransferTonnage.Should().NotBeNull();
                noteTonnage.NoteTransferTonnage.Select(nt => nt.TransferNote).Should().NotBeNull();
            }
        }
    }
}
