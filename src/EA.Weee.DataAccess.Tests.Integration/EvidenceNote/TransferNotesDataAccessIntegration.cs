﻿namespace EA.Weee.DataAccess.Tests.Integration.EvidenceNote
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
                ObligatedWeeeIntegrationCommon.CreateScheme(organisation2);
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
                note8ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note8ToNotBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser(), SystemTime.Now);
                note8ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note8ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note9ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                note9ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note9ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
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

                var noteToBeExcluded = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToBeExcluded.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToBeExcluded.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToBeExcluded.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));
                noteToBeExcluded.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 2, null));

                context.Notes.Add(noteToBeExcluded);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt(),
                    WeeeCategory.MedicalDevices.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, 
                    new List<Guid>() { noteToBeExcluded.Id}, SystemTime.Now.Year, null, 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(2);
                notes.Notes.Count().Should().Be(2);
                notes.Notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Notes.Should().Contain(n => n.Id.Equals(note10ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note2ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note3ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note4ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note5ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note6ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note7ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note8ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note9ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note11ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note12ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(noteToBeExcluded.Id));
                notes.Notes.ElementAtOrDefault(0).WasteType.Should().Be(WasteType.HouseHold);
                notes.Notes.ElementAtOrDefault(1).WasteType.Should().Be(WasteType.HouseHold);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSchemeAndCategoriesAndPaging_NotesShouldBeReturned()
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

                await context.SaveChangesAsync();

                // note not to be found has category but with not tonnage, matching scheme and status
                var note2ToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note2ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note2ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note2ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note2ToNotBeFound);

                // note not to be found not matching scheme, matching status and category
                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                ObligatedWeeeIntegrationCommon.CreateScheme(organisation2);
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
                note8ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note8ToNotBeFound.UpdateStatus(NoteStatus.Void, context.GetCurrentUser(), SystemTime.Now);
                note8ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null));

                context.Notes.Add(note8ToNotBeFound);

                // note not to be found not matching note type, matching status and category
                var note9ToNotBeFound = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                note9ToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note9ToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
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

                await context.SaveChangesAsync();

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
                note12ToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 1, null));

                context.Notes.Add(note12ToNotBeFound);

                // to be found matching category, scheme and status
                var note11ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note11ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note11ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note11ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 1, null));

                context.Notes.Add(note11ToBeFound);

                // to be found matching category, scheme and status
                var note12ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note12ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note12ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note12ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 1, null));

                context.Notes.Add(note12ToBeFound);

                var note1ToBeExcludedFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));
                note1ToBeExcludedFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 2, null));

                context.Notes.Add(note1ToBeExcludedFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt(),
                    WeeeCategory.MedicalDevices.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>()
                {
                    note1ToBeExcludedFound.Id
                }, SystemTime.Now.Year, null, 1, 2);

                notes.NumberOfResults.Should().Be(4);
                notes.Notes.Count().Should().Be(2);
                notes.Notes.Should().Contain(n => n.Id.Equals(note11ToBeFound.Id));
                notes.Notes.Should().Contain(n => n.Id.Equals(note12ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note10ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note1ToBeExcludedFound.Id));
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

                var noteToBeExcluded = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToBeExcluded.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToBeExcluded.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToBeExcluded.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, null));

                context.Notes.Add(noteToBeExcluded);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>()
                {
                    noteToBeExcluded.Id
                }, SystemTime.Now.Year, null, 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(1);
                notes.Notes.Count().Should().Be(1);
                notes.Notes.ElementAt(0).NoteTonnage.Count().Should().Be(1);
                notes.Notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.Count.Should().Be(1);
                notes.Notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).TransferNoteId.Should().Be(transferNote.Id);
                notes.Notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).NoteTonnage.Id.Should()
                    .Be(note1ToBeFound.NoteTonnage.ElementAt(0).Id);
                notes.Notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).Received.Should().Be(1);
                notes.Notes.Should().NotContain(n => n.Id == noteToBeExcluded.Id);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSchemeAndCategoriesAndPreviouslyTransferredNoteWithPaging_NotesShouldBeReturned()
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

                // to be found matching category, scheme and status
                var note2ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note2ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note2ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note2ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, null));

                context.Notes.Add(note2ToBeFound);

                await context.SaveChangesAsync();

                var transferTonnage1 = new NoteTransferTonnage(note1ToBeFound.NoteTonnage.ElementAt(0).Id, 1, null);

                var transferNote1 = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                transferNote1.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                transferNote1.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                transferNote1.NoteTransferTonnage.Add(transferTonnage1);

                context.Notes.Add(transferNote1);

                var transferTonnage2 = new NoteTransferTonnage(note2ToBeFound.NoteTonnage.ElementAt(0).Id, 1, null);

                var transferNote2 = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                transferNote2.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                transferNote2.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                transferNote2.NoteTransferTonnage.Add(transferTonnage2);

                // to be found matching category, scheme and status
                var note1ToBeExcludedFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 2, null));

                context.Notes.Add(note1ToBeExcludedFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, 
                    categorySearch, 
                    new List<Guid>()
                    {
                        note1ToBeExcludedFound.Id
                    },
                    SystemTime.Now.Year,
                    null,
                    2, 
                    1);

                notes.NumberOfResults.Should().Be(2);
                notes.Notes.Count().Should().Be(1);
                notes.Notes.ElementAt(0).Id.Should().Be(note1ToBeFound.Id);
                notes.Notes.ElementAt(0).NoteTonnage.Count().Should().Be(1);
                notes.Notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.Count.Should().Be(1);
                notes.Notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).TransferNoteId.Should().Be(transferNote1.Id);
                notes.Notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).NoteTonnage.Id.Should()
                    .Be(note1ToBeFound.NoteTonnage.ElementAt(0).Id);
                notes.Notes.ElementAt(0).NoteTonnage.ElementAt(0).NoteTransferTonnage.ElementAt(0).Received.Should().Be(1);
                notes.Notes.Should().NotContain(n => n.Id.Equals(note1ToBeExcludedFound.Id));
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

                // to be found matching category, scheme and status and compliance year
                var note1ToBeExcludedFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note1ToBeExcludedFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>()
                {
                    note1ToBeExcludedFound.Id
                }, SystemTime.Now.Year, null, 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(1);
                notes.Notes.Count().Should().Be(1);
                notes.Notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note2ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note3ToNotBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note1ToBeExcludedFound.Id));
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSchemeAndCategoriesWithNonMatchingComplianceYearWithPaging_NotesShouldBeReturned()
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

                await context.SaveChangesAsync();

                // to be found matching category, scheme and status and compliance year
                var note2ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year);
                note2ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note2ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note2ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note2ToBeFound);

                await context.SaveChangesAsync();

                // to be found matching category, scheme and status and compliance year
                var note3ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year);
                note3ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note3ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note3ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note3ToBeFound);

                await context.SaveChangesAsync();

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

                // to be found matching category, scheme and status and compliance year
                var note1ToBeExcludedFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, SystemTime.Now.Year);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note1ToBeExcludedFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, 
                    categorySearch, 
                    new List<Guid>()
                    {
                        note1ToBeExcludedFound.Id
                    },
                    SystemTime.Now.Year,
                    null, 2, 1);

                notes.NumberOfResults.Should().Be(3);
                notes.Notes.Count().Should().Be(1);
                notes.Notes.Should().Contain(n => n.Id.Equals(note2ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note3ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note1ToBeExcludedFound.Id));
            }
        }

        [Fact]
        public async Task GetTransferSelectedNotes_GivenEvidenceNotes_NotesShouldBeReturned()
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

                // to be found matching category, scheme and status and id is requested
                var note1ToBeExcludedFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcludedFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note1ToBeExcludedFound);

                await context.SaveChangesAsync();

                var notes = await dataAccess.GetTransferSelectedNotes(organisation1.Id, new List<Guid>()
                {
                    note1ToBeFound.Id,
                    note2ToBeFound.Id
                });

                notes.NumberOfResults.Should().Be(2);
                notes.Notes.Count().Should().Be(2);
                notes.Notes.Should().Contain(n => n.Id.Equals(note1ToBeFound.Id));
                notes.Notes.Should().Contain(n => n.Id.Equals(note2ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note3NotToBeFound.Id));
                notes.Notes.ElementAtOrDefault(0).WasteType.Should().Be(WasteType.HouseHold);
                notes.Notes.ElementAtOrDefault(1).WasteType.Should().Be(WasteType.HouseHold);
                notes.Notes.Should().NotContain(n => n.Id.Equals(note1ToBeExcludedFound.Id));
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenExcludedEvidenceNotes_NotesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                // not matches criteria but should be excluded from the results
                var note1ToBeExcluded = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note1ToBeExcluded.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcluded.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note1ToBeExcluded.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note1ToBeExcluded);

                var note2ToBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                note2ToBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                note2ToBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                note2ToBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(note2ToBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>()
                {
                    note1ToBeExcluded.Id
                }, SystemTime.Now.Year, null, 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(1);
                notes.Notes.Count().Should().Be(1);
                notes.Notes.Should().Contain(n => n.Id.Equals(note2ToBeFound.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(note1ToBeExcluded.Id));
            }
        }

        [Theory]
        [InlineData("E")]
        [InlineData("")]
        [InlineData("e")]
        public async Task GetNotesToTransfer_GivenSearchRef_NoteShouldBeReturned(string noteType)
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                // note matches criteria and search ref
                var noteToBeIncluded = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToBeIncluded.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToBeIncluded.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToBeIncluded.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(noteToBeIncluded);

                var noteToNotBeIncluded = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToNotBeIncluded.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeIncluded.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeIncluded.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(noteToNotBeIncluded);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year, $"{noteType}{noteToBeIncluded.Reference}", 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(1);
                notes.Notes.Count().Should().Be(1);
                notes.Notes.Should().Contain(n => n.Id.Equals(noteToBeIncluded.Id));
                notes.Notes.Should().NotContain(n => n.Id.Equals(noteToNotBeIncluded.Id));
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSearchRefAndNoteMatchingSearchRefIsInExcludeList_NoteShouldNotBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                var noteToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(noteToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>()
                {
                    noteToNotBeFound.Id
                }, SystemTime.Now.Year, noteToNotBeFound.Reference.ToString(), 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(0);
                notes.Notes.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSearchRefAndNoteDoesNotMatchCategory_NoteShouldNotBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                var noteToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 1, null));

                context.Notes.Add(noteToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year, noteToNotBeFound.Reference.ToString(), 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(0);
                notes.Notes.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSearchRefNotMatchingNoteType_NoteShouldNotBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                var noteToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(noteToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year, $"T{noteToNotBeFound.Reference.ToString()}", 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(0);
                notes.Notes.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSearchRefNotMatchingComplianceYear_NoteShouldNotBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                var noteToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, complianceYear: SystemTime.Now.Year);
                noteToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(noteToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year + 1, noteToNotBeFound.Reference.ToString(), 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(0);
                notes.Notes.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSearchRefNotMatchingRecipient_NoteShouldNotBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);

                var organisation2 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme2 = ObligatedWeeeIntegrationCommon.CreateScheme(organisation2);

                context.Schemes.Add(scheme);

                var noteToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1, complianceYear: SystemTime.Now.Year);
                noteToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(noteToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation2.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year, noteToNotBeFound.Reference.ToString(), 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(0);
                notes.Notes.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSearchRefNotMatchingWasteType_NoteShouldNotBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                var noteToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                ObjectInstantiator<Note>.SetProperty(n => n.WasteType, WasteType.NonHouseHold, noteToNotBeFound);
                noteToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.UpdateStatus(NoteStatus.Approved, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(noteToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year, noteToNotBeFound.Reference.ToString(), 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(0);
                notes.Notes.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GetNotesToTransfer_GivenSearchRefAndNoteIsNotApproved_NoteShouldNotBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                var noteToNotBeFound = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToNotBeFound.UpdateStatus(NoteStatus.Submitted, context.GetCurrentUser(), SystemTime.Now);
                noteToNotBeFound.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, null));

                context.Notes.Add(noteToNotBeFound);

                await context.SaveChangesAsync();

                var categorySearch = new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                };

                var notes = await dataAccess.GetNotesToTransfer(organisation1.Id, categorySearch, new List<Guid>(), SystemTime.Now.Year, noteToNotBeFound.Reference.ToString(), 1, int.MaxValue);

                notes.NumberOfResults.Should().Be(0);
                notes.Notes.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task AddTransferNote_NotesShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

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

        [Fact]
        public async Task UpdateDraftTransferNote_TransferNoteShouldBeUpdated()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, A.Fake<IUserContext>(), new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                var noteToBeTransferred1 = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                noteToBeTransferred1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, null));
                noteToBeTransferred1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 2, null));

                context.Notes.Add(noteToBeTransferred1);

                var noteToBeTransferred2 = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                
                noteToBeTransferred2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 10, null));
                noteToBeTransferred2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 30, null));

                context.Notes.Add(noteToBeTransferred2);

                var noteToBeTransferred3 = await SetupSingleNote(context, database, NoteType.EvidenceNote, organisation1);
                noteToBeTransferred3.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 10, null));

                context.Notes.Add(noteToBeTransferred3);

                var transferOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var transferOrganisationScheme = ObligatedWeeeIntegrationCommon.CreateScheme(transferOrganisation);
                context.Schemes.Add(transferOrganisationScheme);

                await context.SaveChangesAsync();

                var transferTonnages = new List<NoteTransferTonnage>
                {
                    new NoteTransferTonnage(noteToBeTransferred1.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id, 5, 4),
                    new NoteTransferTonnage(noteToBeTransferred2.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id, 8, 7),
                    new NoteTransferTonnage(noteToBeTransferred2.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id, 4, null),
                    new NoteTransferTonnage(noteToBeTransferred1.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id, 1, null)
                };

                var newTransferNote = await SetupSingleNote(context, database, NoteType.TransferNote, transferOrganisation);
                newTransferNote.NoteTransferTonnage.AddRange(transferTonnages);

                context.Notes.Add(newTransferNote);

                await context.SaveChangesAsync();

                var refreshedTransferNote = await context.Notes.FirstOrDefaultAsync(n => n.Id.Equals(newTransferNote.Id));

                var newTransferOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var newTransferOrganisationScheme = ObligatedWeeeIntegrationCommon.CreateScheme(newTransferOrganisation);
                context.Schemes.Add(newTransferOrganisationScheme);

                await context.SaveChangesAsync();

                var newTransferTonnages = new List<NoteTransferTonnage>
                {
                    new NoteTransferTonnage(noteToBeTransferred1.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id, 2, 1),
                    new NoteTransferTonnage(noteToBeTransferred3.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id, 6, 2),
                    new NoteTransferTonnage(noteToBeTransferred2.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id, 4, null)
                };

                await dataAccess.UpdateTransfer(refreshedTransferNote, newTransferOrganisation, newTransferTonnages,
                    NoteStatus.Draft, SystemTime.Now);

                refreshedTransferNote = await context.Notes.FirstOrDefaultAsync(n => n.Id.Equals(newTransferNote.Id));

                refreshedTransferNote.RecipientId.Should().Be(newTransferOrganisation.Id);
                refreshedTransferNote.NoteTransferTonnage.Count.Should().Be(3);
                refreshedTransferNote.Status.Should().Be(NoteStatus.Draft);
                var noteTonnage1 = noteToBeTransferred1.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances));
                var noteTonnage2 = noteToBeTransferred2.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LightingEquipment));
                var noteTonnage3 = noteToBeTransferred3.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports));
                
                refreshedTransferNote.NoteTransferTonnage.FirstOrDefault(nt => nt.NoteTonnageId == noteTonnage1.Id).Should().NotBeNull();
                refreshedTransferNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnage1.Id).Received.Should().Be(2);
                refreshedTransferNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnage1.Id).Reused.Should().Be(1);
                refreshedTransferNote.NoteTransferTonnage.FirstOrDefault(nt => nt.NoteTonnageId == noteTonnage2.Id).Should().NotBeNull();
                refreshedTransferNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnage2.Id).Received.Should().Be(4);
                refreshedTransferNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnage2.Id).Reused.Should().Be(null);
                refreshedTransferNote.NoteTransferTonnage.FirstOrDefault(nt => nt.NoteTonnageId == noteTonnage3.Id).Should().NotBeNull();
                refreshedTransferNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnage3.Id).Received.Should().Be(6);
                refreshedTransferNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnage3.Id).Reused.Should().Be(2);
            }
        }

        [Fact]
        public async Task UpdateDraftTransferNoteToSubmitted_TransferNoteShouldBeUpdated()
        {
            using (var database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var userContext = A.Fake<IUserContext>();
                A.CallTo(() => userContext.UserId).Returns(Guid.Parse(context.GetCurrentUser()));
                var dataAccess = new EvidenceDataAccess(database.WeeeContext, userContext, new GenericDataAccess(database.WeeeContext));

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation1);
                context.Schemes.Add(scheme);

                var noteToBeTransferred1 = await SetupSingleNote(context, database, NoteType.TransferNote, organisation1);
                noteToBeTransferred1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, null));

                context.Notes.Add(noteToBeTransferred1);

                var transferOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var transferOrganisationScheme = ObligatedWeeeIntegrationCommon.CreateScheme(transferOrganisation);
                context.Schemes.Add(transferOrganisationScheme);

                await context.SaveChangesAsync();

                var transferTonnages = new List<NoteTransferTonnage>
                {
                    new NoteTransferTonnage(noteToBeTransferred1.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id, 2, 1)
                };

                var newTransferNote = await SetupSingleNote(context, database, NoteType.TransferNote, transferOrganisation);
                newTransferNote.NoteTransferTonnage.AddRange(transferTonnages);

                context.Notes.Add(newTransferNote);

                await context.SaveChangesAsync();

                var refreshedTransferNote = await context.Notes.FirstOrDefaultAsync(n => n.Id.Equals(newTransferNote.Id));

                var newTransferOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var newTransferOrganisationScheme = ObligatedWeeeIntegrationCommon.CreateScheme(newTransferOrganisation);
                context.Schemes.Add(newTransferOrganisationScheme);

                await context.SaveChangesAsync();

                var newTransferTonnages = new List<NoteTransferTonnage>
                {
                    new NoteTransferTonnage(noteToBeTransferred1.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id, 4, 2)
                };

                await dataAccess.UpdateTransfer(refreshedTransferNote, newTransferOrganisation, newTransferTonnages,
                    NoteStatus.Submitted, SystemTime.Now);

                refreshedTransferNote = await context.Notes.FirstOrDefaultAsync(n => n.Id.Equals(newTransferNote.Id));

                refreshedTransferNote.RecipientId.Should().Be(newTransferOrganisation.Id);
                refreshedTransferNote.NoteTransferTonnage.Count.Should().Be(1);
                refreshedTransferNote.Status.Should().Be(NoteStatus.Submitted);
                refreshedTransferNote.NoteStatusHistory.Count.Should().Be(1);
                refreshedTransferNote.NoteStatusHistory.ElementAt(0).ChangedById.Should().Be(context.GetCurrentUser());
                refreshedTransferNote.NoteStatusHistory.ElementAt(0).ToStatus.Should().Be(NoteStatus.Submitted);
                refreshedTransferNote.NoteStatusHistory.ElementAt(0).FromStatus.Should().Be(NoteStatus.Draft);
                refreshedTransferNote.NoteStatusHistory.ElementAt(0).ChangedDate.Should()
                    .BeCloseTo(SystemTime.Now, TimeSpan.FromSeconds(10));
                var noteTonnage1 = noteToBeTransferred1.NoteTonnage.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances));
                refreshedTransferNote.NoteTransferTonnage.FirstOrDefault(nt => nt.NoteTonnageId == noteTonnage1.Id).Should().NotBeNull();
                refreshedTransferNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnage1.Id).Received.Should().Be(4);
                refreshedTransferNote.NoteTransferTonnage.First(nt => nt.NoteTonnageId == noteTonnage1.Id).Reused.Should().Be(2);
            }
        }
    }
}
