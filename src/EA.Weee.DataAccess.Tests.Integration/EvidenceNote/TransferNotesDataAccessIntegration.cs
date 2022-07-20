namespace EA.Weee.DataAccess.Tests.Integration.EvidenceNote
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Base;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Domain;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using NoteType = Domain.Evidence.NoteType;

    public class TransferNotesDataAccessIntegration : EvidenceNoteBaseDataAccess
    {
        [Fact]
        public async Task GetNotesToTransfer_GivenSchemeAndCategories_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                // to be found matching category, scheme and status
                var note1ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note1ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 2, null));

                context.Notes.Add(note1ToBeFound);

                // note not to be found has category but with not tonnage, matching scheme and status
                var note2ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note2ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note2ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note2ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note2ToNotBeFound);

                // note not to be found not matching scheme, matching status and category
                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation2);
                var note3ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note3ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note3ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note3ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note3ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note4ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                note4ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note4ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note4ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note4ToNotBeFound);

                // note not to be found not matching status, matching type and category
                var note5ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note5ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note5ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note5ToNotBeFound);

                // note not to be found not matching status, matching type and category
                var note6ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note6ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note6ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note7ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note7ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note7ToNotBeFound.UpdateStatus(NoteStatus.Rejected, context.GetCurrentUser(), SystemTime.Now);
                note7ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note7ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note8ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                note8ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note8ToNotBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser(), SystemTime.Now);
                note8ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note8ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note9ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                note9ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note9ToNotBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser(), SystemTime.Now);
                note9ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1, null));

                context.Notes.Add(note9ToNotBeFound);

                // to be found matching category, scheme and status
                var note10ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note10ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note10ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 1, null));
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 1, null));
                note10ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 1, null));

                context.Notes.Add(note10ToBeFound);

                // to not be found matching category but received is null and reused is not null, also with matching scheme and status
                var note11ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note11ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note11ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note11ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, null, 1));

                context.Notes.Add(note11ToNotBeFound);

                // to not be found as transfer note, matching scheme, category
                var note12ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                note12ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note12ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note12ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 1,  null));
                
                context.Notes.Add(note12ToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt(),
                    WeeeCategory.MedicalDevices.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(scheme.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year);

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
                notes.ElementAtOrDefault(0).WasteType.Should().Be(WasteType.HouseHold);
                notes.ElementAtOrDefault(1).WasteType.Should().Be(WasteType.HouseHold);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSchemeAndCategoriesAndPreviouslyTransferredNote_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                // to be found matching category, scheme and status
                var note1ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note1ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, null));

                context.Notes.Add(note1ToBeFound);

                await context.SaveChangesAsync();

                var transferTonnage = new NoteTransferTonnage(note1ToBeFound.NoteTonnage.ElementAt(0).Id, 1, null);

                var transferNote = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                transferNote.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                transferNote.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                transferNote.NoteTransferTonnage.Add(transferTonnage);

                context.Notes.Add(transferNote);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(scheme.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year);

                notes.Count().Should().Be(1);
                notes.ElementAt(0).NoteTonnage.Count().Should().Be(1);
                notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.Count.Should().Be(1);
                notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).TransferNoteId.Should().Be(transferNote.Id);
                notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).NoteTonnage.Id.Should()
                    .Be(note1ToBeFound.NoteTonnage.ElementAt(0).Id);
                notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).Received.Should().Be(1);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSchemeAndCategoriesWithNonMatchingComplianceYear_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                // to be found matching category, scheme and status and compliance year
                var note1ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year);
                note1ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note1ToBeFound);

                // note not to be found non matching compliance year
                var note2ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year - 1);
                note2ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note2ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note2ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note2ToNotBeFound);

                // note not to be found non matching compliance year
                var note3ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year + 1);
                note3ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note3ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note3ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note3ToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(scheme.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year);

                notes.Count().Should().Be(1);
                notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note2ToNotBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note3ToNotBeFound.Id));
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenEvidenceNotes_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                // to be found matching category, scheme and status and id is requested
                var note1ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note1ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note1ToBeFound);

                // to be found matching category, scheme and status and id is requested
                var note2ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note2ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note2ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note2ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note2ToBeFound);

                // note not to be found matching scheme, status but not in evidence note list
                var note3NotToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note3NotToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note3NotToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note3NotToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note3NotToBeFound);

                // Waste Type being 'NON-HOUSEHOLD' should be filtered out
                var note4NonHouseHold = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                DateTime startDate = note4NonHouseHold.StartDate;
                DateTime endDate = note4NonHouseHold.EndDate;
                var protocol = note4NonHouseHold.Protocol;
                note4NonHouseHold.Update(organisation1, startDate, endDate, wasteType: WasteType.NonHouseHold, protocol);
                note4NonHouseHold.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note4NonHouseHold.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note4NonHouseHold.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note4NonHouseHold);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(scheme.Id, categorySearch, new List<Guid>()
                {
                    note1ToBeFound.Id,
                    note2ToBeFound.Id
                }, SystemTime.Now.Year);

                notes.Count().Should().Be(2);
                notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Should().Contain(n => n.Id.Equals(note2ToBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note3NotToBeFound.Id));
                notes.ElementAtOrDefault(0).WasteType.Should().Be(WasteType.HouseHold);
                notes.ElementAtOrDefault(1).WasteType.Should().Be(WasteType.HouseHold);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenEvidenceNotesAndComplianceYear_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                // to be found matching category, scheme and status and id is requested
                var note1ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year);
                note1ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note1ToBeFound);

                // to not be found matching criteria and id is requested but no compliance year match
                var note2NotToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year - 1);
                note2NotToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note2NotToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note2NotToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note2NotToBeFound);

                // to not be found matching criteria and id is requested but no compliance year match
                var note3NotToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year + 1);
                note3NotToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note3NotToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
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
                    note2NotToBeFound.Id
                }, SystemTime.Now.Year);

                notes.Count().Should().Be(1);
                notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note2NotToBeFound.Id));
                notes.Should().NotContain(n => n.Id.Equals(note3NotToBeFound.Id));
            }
        }

        [Fact]
        public async Task AddTransferNote_NotesShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                // create note that has tonnage to be transferred
                var noteToBeTransferred1 = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToBeTransferred1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToBeTransferred1.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToBeTransferred1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));
                noteToBeTransferred1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, null));

                context.Notes.Add(noteToBeTransferred1);

                var noteToBeTransferred2 = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToBeTransferred2.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToBeTransferred2.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToBeTransferred2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 10, null));
                noteToBeTransferred2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 30, null));
                noteToBeTransferred2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 20, null));

                context.Notes.Add(noteToBeTransferred2);

                var transferOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var transferOrganisationScheme = ObligatedWeeeIntegrationCommon.CreateScheme(transferOrganisation);
                context.Schemes.Add(transferOrganisationScheme);

                await context.SaveChangesAsync();

                var transferTonnages = new List<NoteTransferTonnage>
                {
                    new NoteTransferTonnage(noteToBeTransferred1.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id, 5, 4),
                    new NoteTransferTonnage(noteToBeTransferred2.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id, 8, 7),
                    new NoteTransferTonnage(noteToBeTransferred2.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id, 4, null)
                };

                var note = await dataAccess.AddTransferNote(organisation1, transferOrganisation, transferTonnages,
                    NoteStatus.Draft, noteToBeTransferred1.ComplianceYear, context.GetCurrentUser(), SystemTime.Now);

                var refreshedTransferNote = await context.Notes.FirstOrDefaultAsync(n => n.Id.Equals(note.Id));

                refreshedTransferNote.Aatf.Should().BeNull();
                refreshedTransferNote.CreatedById.Should().Be(context.GetCurrentUser());
                refreshedTransferNote.StartDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                refreshedTransferNote.EndDate.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromSeconds(5));
                refreshedTransferNote.NoteType.Should().Be(NoteType.TransferNote);
                refreshedTransferNote.Organisation.Should().Be(organisation1);
                refreshedTransferNote.NoteStatusHistory.Count.Should().Be(0);
                refreshedTransferNote.NoteTonnage.Count.Should().Be(0);
                refreshedTransferNote.NoteTransferTonnage.Count.Should().Be(3);
                refreshedTransferNote.ComplianceYear.Should().Be(noteToBeTransferred1.ComplianceYear);

                var transferTonnage = refreshedTransferNote.NoteTransferTonnage.FirstOrDefault(nt =>
                    nt.NoteTonnageId.Equals(
                        noteToBeTransferred1.NoteTonnage
                            .First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id));
                transferTonnage.Should().NotBeNull();
                transferTonnage.Received.Should().Be(5);
                transferTonnage.Reused.Should().Be(4);
                transferTonnage = refreshedTransferNote.NoteTransferTonnage.FirstOrDefault(nt =>
                    nt.NoteTonnageId.Equals(
                        noteToBeTransferred2.NoteTonnage
                            .First(c => c.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id));
                transferTonnage.Should().NotBeNull();
                transferTonnage.Received.Should().Be(8);
                transferTonnage.Reused.Should().Be(7);
                transferTonnage = refreshedTransferNote.NoteTransferTonnage.FirstOrDefault(nt =>
                    nt.NoteTonnageId.Equals(
                        noteToBeTransferred2.NoteTonnage
                            .First(c => c.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id));
                transferTonnage.Should().NotBeNull();
                transferTonnage.Received.Should().Be(4);
                transferTonnage.Reused.Should().Be(null);
            }
        }
    }
}
