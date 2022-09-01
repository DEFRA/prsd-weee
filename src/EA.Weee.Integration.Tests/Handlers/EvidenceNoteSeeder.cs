namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Builders;
    using Core.Admin.Obligation;
    using Core.Shared;
    using Domain.AatfReturn;
    using Domain.Obligation;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Lookup;
    using NUnit.Framework;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Requests.Shared;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;

    public class EvidenceNoteSeeder : IntegrationTestBase
    {
        [Component]
        [Ignore("Time consuming data set only use for seeding of data")]
        public class SeedTheDataBaseWithLotsOfNotes : EvidenceNoteSeederBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                // create some schemes for and some obligations over 20 years
                var schemesList = new List<Scheme>();
                var organisations = new List<Organisation>();
                var aatfs = new List<Aatf>();
                const int numberOfNotes = 900;
                const int numberOfTransferredInX = 150;
                const int numberOfTransfersOut = 50;
                Random randomTransferOut = new Random();
                Random randomTransferIn = new Random();
                Random randomAatf = new Random();
                Random tonnageRandom = new Random();
                Random randomStatus = new Random();
                Random randomWastType = new Random();
                Random randomApprovedNote = new Random();
                Random randomRecipient = new Random();

                var noteStatuses = Enumeration.GetAll<NoteStatus>().ToList();
                var wasteTypes = Enum.GetValues(typeof(WasteType));

                for (var i = 0; i < 50; i++)
                {
                    var aatfOrganisation = OrganisationDbSetup.Init().Create();
                    var aatf = AatfDbSetup.Init().WithOrganisation(aatfOrganisation.Id).Create();
                    aatfs.Add(aatf);
                }

                for (int i = 0; i < 40; i++)
                {
                    var randomOrganisation = OrganisationDbSetup.Init().Create();
                    var randomScheme = SchemeDbSetup.Init().WithOrganisation(randomOrganisation.Id).Create();
                    organisations.Add(randomOrganisation);
                    schemesList.Add(randomScheme);
                }

                // create 5 years of old data for each scheme
                for (var year = 2020; year <= 2025; year++)
                {
                    var notesInComplianceYear = new List<Note>();

                    foreach (var scheme in schemesList)
                    {
                        var randomObligationScheme = ObligationUploadDbSetup.Init().Create();

                        var tonnage1 = NextTonnage(tonnageRandom);
                        var tonnage2 = NextTonnage(tonnageRandom);
                        var tonnage3 = NextTonnage(tonnageRandom);
                        var tonnage4 = NextTonnage(tonnageRandom);
                        var tonnage5 = NextTonnage(tonnageRandom);
                        var tonnage6 = NextTonnage(tonnageRandom);
                        var tonnage7 = NextTonnage(tonnageRandom);
                        var tonnage8 = NextTonnage(tonnageRandom);
                        var tonnage9 = NextTonnage(tonnageRandom);
                        var tonnage10 = NextTonnage(tonnageRandom);
                        var tonnage11 = NextTonnage(tonnageRandom);
                        var tonnage12 = NextTonnage(tonnageRandom);
                        var tonnage13 = NextTonnage(tonnageRandom);
                        var tonnage14 = NextTonnage(tonnageRandom);

                        var justSomeExtraYears = new List<ObligationSchemeAmount>()
                        {
                            new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, tonnage1),
                            new ObligationSchemeAmount(WeeeCategory.MedicalDevices, tonnage2),
                            new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, tonnage3),
                            new ObligationSchemeAmount(WeeeCategory.ElectricalAndElectronicTools, tonnage4),
                            new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, tonnage5),
                            new ObligationSchemeAmount(WeeeCategory.ToysLeisureAndSports, tonnage6),
                            new ObligationSchemeAmount(WeeeCategory.AutomaticDispensers, tonnage7),
                            new ObligationSchemeAmount(WeeeCategory.DisplayEquipment, tonnage8),
                            new ObligationSchemeAmount(WeeeCategory.CoolingApplicancesContainingRefrigerants, tonnage9),
                            new ObligationSchemeAmount(WeeeCategory.SmallHouseholdAppliances, tonnage10),
                            new ObligationSchemeAmount(WeeeCategory.LargeHouseholdAppliances, tonnage11),
                            new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, tonnage12),
                            new ObligationSchemeAmount(WeeeCategory.LightingEquipment, tonnage13),
                            new ObligationSchemeAmount(WeeeCategory.MonitoringAndControlInstruments, tonnage14),
                        };

                        ObligationSchemeDbSetup.Init().WithScheme(scheme.Id)
                            .WithObligationUpload(randomObligationScheme.Id)
                            .WithObligationAmounts(justSomeExtraYears)
                            .WithComplianceYear(year)
                            .Create();
                    }

                    foreach (var organisation in organisations)
                    {
                        var notes = new List<Note>();
                        for (int i = 0; i < randomApprovedNote.Next(numberOfNotes - 200, numberOfNotes - 1); i++)
                        {
                            var tonnage1 = NextTonnage(tonnageRandom);
                            var tonnage2 = NextTonnage(tonnageRandom);
                            var tonnage3 = NextTonnage(tonnageRandom);
                            var tonnage4 = NextTonnage(tonnageRandom);
                            var tonnage5 = NextTonnage(tonnageRandom);
                            var tonnage6 = NextTonnage(tonnageRandom);
                            var tonnage7 = NextTonnage(tonnageRandom);
                            var tonnage8 = NextTonnage(tonnageRandom);
                            var tonnage9 = NextTonnage(tonnageRandom);
                            var tonnage10 = NextTonnage(tonnageRandom);
                            var tonnage11 = NextTonnage(tonnageRandom);
                            var tonnage12 = NextTonnage(tonnageRandom);
                            var tonnage13 = NextTonnage(tonnageRandom);
                            var tonnage14 = NextTonnage(tonnageRandom);

                            var tonnages2 = new List<NoteTonnage>()
                            {
                                new NoteTonnage(WeeeCategory.PhotovoltaicPanels, tonnage1, NextTonnage(tonnageRandom, tonnage1 - 5)),
                                new NoteTonnage(WeeeCategory.MedicalDevices, tonnage2, NextTonnage(tonnageRandom, tonnage2 - 5)),
                                new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, tonnage3, NextTonnage(tonnageRandom, tonnage3 - 5)),
                                new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, tonnage4, NextTonnage(tonnageRandom, tonnage4 - 5)),
                                new NoteTonnage(WeeeCategory.ConsumerEquipment, tonnage5, NextTonnage(tonnageRandom, tonnage5 - 5)),
                                new NoteTonnage(WeeeCategory.ToysLeisureAndSports, tonnage6, NextTonnage(tonnageRandom, tonnage6 - 5)),
                                new NoteTonnage(WeeeCategory.AutomaticDispensers, tonnage7, NextTonnage(tonnageRandom, tonnage7 - 5)),
                                new NoteTonnage(WeeeCategory.DisplayEquipment, tonnage8, NextTonnage(tonnageRandom, tonnage8 - 5)),
                                new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, tonnage9, NextTonnage(tonnageRandom, tonnage9 - 5)),
                                new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, tonnage10, NextTonnage(tonnageRandom, tonnage10 - 5)),
                                new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, tonnage11, NextTonnage(tonnageRandom, tonnage11 - 5)),
                                new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, tonnage12, NextTonnage(tonnageRandom, tonnage12 - 5)),
                                new NoteTonnage(WeeeCategory.LightingEquipment, tonnage13, NextTonnage(tonnageRandom, tonnage13 - 5)),
                                new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, tonnage14, NextTonnage(tonnageRandom, tonnage14 - 5)),
                            };

                            var newNote = EvidenceNoteDbSetup.Init()
                                .WithRecipient(organisation.Id)
                                .WithStatus(NoteStatus.Submitted, UserId.ToString())
                                .WithStatus(NoteStatus.Approved, UserId.ToString())
                                .WithWasteType(WasteType.HouseHold)
                                .WithAatf(aatfs.ElementAt(randomAatf.Next(0, 49)).Id)
                                .WithComplianceYear(year)
                                .WithTonnages(tonnages2).Create();

                            notes.Add(newNote);
                            notesInComplianceYear.Add(newNote);
                        }

                        //create some random notes with status and waste type
                        for (int i = 0; i < 100; i++)
                        {
                            var tonnage1 = NextTonnage(tonnageRandom);
                            var tonnage2 = NextTonnage(tonnageRandom);
                            var tonnage3 = NextTonnage(tonnageRandom);
                            var tonnage4 = NextTonnage(tonnageRandom);
                            var tonnage5 = NextTonnage(tonnageRandom);
                            var tonnage6 = NextTonnage(tonnageRandom);
                            var tonnage7 = NextTonnage(tonnageRandom);
                            var tonnage8 = NextTonnage(tonnageRandom);
                            var tonnage9 = NextTonnage(tonnageRandom);
                            var tonnage10 = NextTonnage(tonnageRandom);
                            var tonnage11 = NextTonnage(tonnageRandom);
                            var tonnage12 = NextTonnage(tonnageRandom);
                            var tonnage13 = NextTonnage(tonnageRandom);
                            var tonnage14 = NextTonnage(tonnageRandom);

                            var tonnages2 = new List<NoteTonnage>()
                            {
                                new NoteTonnage(WeeeCategory.PhotovoltaicPanels, tonnage1, NextTonnage(tonnageRandom, tonnage1)),
                                new NoteTonnage(WeeeCategory.MedicalDevices, tonnage2, NextTonnage(tonnageRandom, tonnage2)),
                                new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, tonnage3, NextTonnage(tonnageRandom, tonnage3)),
                                new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, tonnage4, NextTonnage(tonnageRandom, tonnage4)),
                                new NoteTonnage(WeeeCategory.ConsumerEquipment, tonnage5, NextTonnage(tonnageRandom, tonnage5)),
                                new NoteTonnage(WeeeCategory.ToysLeisureAndSports, tonnage6, NextTonnage(tonnageRandom, tonnage6)),
                                new NoteTonnage(WeeeCategory.AutomaticDispensers, tonnage7, NextTonnage(tonnageRandom, tonnage7)),
                                new NoteTonnage(WeeeCategory.DisplayEquipment, tonnage8, NextTonnage(tonnageRandom, tonnage8)),
                                new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, tonnage9, NextTonnage(tonnageRandom, tonnage9)),
                                new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, tonnage10, NextTonnage(tonnageRandom, tonnage10)),
                                new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, tonnage11, NextTonnage(tonnageRandom, tonnage11)),
                                new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, tonnage12, NextTonnage(tonnageRandom, tonnage12)),
                                new NoteTonnage(WeeeCategory.LightingEquipment, tonnage13, NextTonnage(tonnageRandom, tonnage13)),
                                new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, tonnage14, NextTonnage(tonnageRandom, tonnage14)),
                            };

                            var status = noteStatuses.ElementAt(randomStatus.Next(noteStatuses.Count - 1));
                            var wasteType = (WasteType)wasteTypes.GetValue(randomWastType.Next(wasteTypes.Length));

                            var newNote = EvidenceNoteDbSetup.Init().WithRecipient(organisation.Id)
                                .WithStatusUpdate(status)
                                .WithWasteType(wasteType)
                                .WithAatf(aatfs.ElementAt(randomAatf.Next(0, 49)).Id)
                                .WithComplianceYear(year)
                                .WithTonnages(tonnages2).Create();

                            notes.Add(newNote);
                        }

                        for (int i = 0; i < numberOfTransferredInX; i++)
                        {
                            var noteToTransfer = notes.ElementAt(randomTransferOut.Next(0, notes.Count - 1));

                            var noteTonnage1 = noteToTransfer.NoteTonnage.First(
                                nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels));
                            var noteTonnage2 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.MedicalDevices));
                            var noteTonnage3 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources));
                            var noteTonnage4 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools));
                            var noteTonnage5 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));
                            var noteTonnage6 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports));
                            var noteTonnage7 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers));
                            var noteTonnage8 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.DisplayEquipment));
                            var noteTonnage9 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants));
                            var noteTonnage10 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances));
                            var noteTonnage11 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances));
                            var noteTonnage12 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment));
                            var noteTonnage13 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.LightingEquipment));
                            var noteTonnage14 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments));

                            var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                            {
                                new NoteTransferTonnage(
                                    noteTonnage1.Id, NextTonnage(tonnageRandom, noteTonnage1.Received - 3), NextTonnage(tonnageRandom, noteTonnage1.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage2.Id, NextTonnage(tonnageRandom, noteTonnage2.Received - 3), NextTonnage(tonnageRandom, noteTonnage2.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage3.Id, NextTonnage(tonnageRandom, noteTonnage3.Received - 3), NextTonnage(tonnageRandom, noteTonnage3.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage4.Id, NextTonnage(tonnageRandom, noteTonnage4.Received - 3), NextTonnage(tonnageRandom, noteTonnage4.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage5.Id, NextTonnage(tonnageRandom, noteTonnage5.Received - 3), NextTonnage(tonnageRandom, noteTonnage5.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage6.Id, NextTonnage(tonnageRandom, noteTonnage6.Received - 3), NextTonnage(tonnageRandom, noteTonnage6.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage7.Id, NextTonnage(tonnageRandom, noteTonnage7.Received - 3), NextTonnage(tonnageRandom, noteTonnage7.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage8.Id, NextTonnage(tonnageRandom, noteTonnage8.Received - 3), NextTonnage(tonnageRandom, noteTonnage8.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage9.Id, NextTonnage(tonnageRandom, noteTonnage9.Received - 3), NextTonnage(tonnageRandom, noteTonnage9.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage10.Id, NextTonnage(tonnageRandom, noteTonnage10.Received - 3), NextTonnage(tonnageRandom, noteTonnage10.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage11.Id, NextTonnage(tonnageRandom, noteTonnage11.Received - 3), NextTonnage(tonnageRandom, noteTonnage11.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage12.Id, NextTonnage(tonnageRandom, noteTonnage12.Received - 3), NextTonnage(tonnageRandom, noteTonnage12.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage13.Id, NextTonnage(tonnageRandom, noteTonnage13.Received - 3), NextTonnage(tonnageRandom, noteTonnage13.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage14.Id, NextTonnage(tonnageRandom, noteTonnage14.Received - 3), NextTonnage(tonnageRandom, noteTonnage14.Reused - 2)),
                            };

                            var transferNoteOrg = OrganisationDbSetup.Init().Create();

                            TransferEvidenceNoteDbSetup.Init().With(t =>
                                {
                                    t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                                    t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddHours(1));
                                }).WithTonnages(newTransferNoteTonnage1)
                                .WithOrganisation(transferNoteOrg.Id)
                                .WithWasteType(WasteType.HouseHold)
                                .WithComplianceYear(year)
                                .WithRecipient(organisation.Id)
                                .Create();
                        }
                    }

                    foreach (var organisation in organisations)
                    {
                        for (int i = 0; i < numberOfTransfersOut; i++)
                        {
                            var filteredNotes = notesInComplianceYear.Where(n => n.RecipientId != organisation.Id && 
                                n.NoteType.Value == NoteType.EvidenceNote.Value).ToList();

                            var noteToTransfer = filteredNotes.ElementAt(randomTransferIn.Next(0, filteredNotes.Count - 1));

                            var noteTonnage1 = noteToTransfer.NoteTonnage.First(
                                nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels));
                            var noteTonnage2 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.MedicalDevices));
                            var noteTonnage3 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources));
                            var noteTonnage4 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools));
                            var noteTonnage5 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment));
                            var noteTonnage6 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports));
                            var noteTonnage7 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers));
                            var noteTonnage8 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.DisplayEquipment));
                            var noteTonnage9 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants));
                            var noteTonnage10 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances));
                            var noteTonnage11 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances));
                            var noteTonnage12 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment));
                            var noteTonnage13 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.LightingEquipment));
                            var noteTonnage14 = noteToTransfer.NoteTonnage.First(nt =>
                                nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments));

                            var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                            {
                                new NoteTransferTonnage(
                                    noteTonnage1.Id, NextTonnage(tonnageRandom, noteTonnage1.Received - 3), NextTonnage(tonnageRandom, noteTonnage1.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage2.Id, NextTonnage(tonnageRandom, noteTonnage2.Received - 3), NextTonnage(tonnageRandom, noteTonnage2.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage3.Id, NextTonnage(tonnageRandom, noteTonnage3.Received - 3), NextTonnage(tonnageRandom, noteTonnage3.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage4.Id, NextTonnage(tonnageRandom, noteTonnage4.Received - 3), NextTonnage(tonnageRandom, noteTonnage4.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage5.Id, NextTonnage(tonnageRandom, noteTonnage5.Received - 3), NextTonnage(tonnageRandom, noteTonnage5.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage6.Id, NextTonnage(tonnageRandom, noteTonnage6.Received - 3), NextTonnage(tonnageRandom, noteTonnage6.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage7.Id, NextTonnage(tonnageRandom, noteTonnage7.Received - 3), NextTonnage(tonnageRandom, noteTonnage7.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage8.Id, NextTonnage(tonnageRandom, noteTonnage8.Received - 3), NextTonnage(tonnageRandom, noteTonnage8.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage9.Id, NextTonnage(tonnageRandom, noteTonnage9.Received - 3), NextTonnage(tonnageRandom, noteTonnage9.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage10.Id, NextTonnage(tonnageRandom, noteTonnage10.Received - 3), NextTonnage(tonnageRandom, noteTonnage10.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage11.Id, NextTonnage(tonnageRandom, noteTonnage11.Received - 3), NextTonnage(tonnageRandom, noteTonnage11.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage12.Id, NextTonnage(tonnageRandom, noteTonnage12.Received - 3), NextTonnage(tonnageRandom, noteTonnage12.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage13.Id, NextTonnage(tonnageRandom, noteTonnage13.Received - 3), NextTonnage(tonnageRandom, noteTonnage13.Reused - 2)),
                                new NoteTransferTonnage(
                                    noteTonnage14.Id, NextTonnage(tonnageRandom, noteTonnage14.Received - 3), NextTonnage(tonnageRandom, noteTonnage14.Reused - 2)),
                            };

                            var filterSchemes = schemesList.Where(s => s.OrganisationId != organisation.Id).ToList();
                            var recipient = filterSchemes.ElementAt(randomRecipient.Next(0, filterSchemes.Count - 1));

                            TransferEvidenceNoteDbSetup.Init().With(t =>
                            {
                                t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                                t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddHours(1));
                            }).WithTonnages(newTransferNoteTonnage1)
                                .WithOrganisation(organisation.Id)
                                .WithWasteType(WasteType.HouseHold)
                                .WithComplianceYear(year)
                                .WithRecipient(recipient.OrganisationId)
                                .Create();
                        }
                    }
                }
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveTheExpectedData = () =>
            {
            };
        }

        public class EvidenceNoteSeederBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetObligationSummaryRequest, ObligationEvidenceSummaryData> handler;
            protected static GetObligationSummaryRequest request;
            protected static ObligationEvidenceSummaryData result;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings()
                    .WithInternalUserAccess(false);

                Query.SetupUserWithRole(UserId.ToString(), "Standard", CompetentAuthority.England);

                handler = Container.Resolve<IRequestHandler<GetObligationSummaryRequest, ObligationEvidenceSummaryData>>();
            }

            public static decimal NextTonnage(Random rng, decimal? maxValue = 25)
            {
                var convertedValue = maxValue.HasValue ? (maxValue.Value < 0 ? 0 : maxValue.Value) : 0;
                double randH, randL;
                do
                {
                    randH = rng.NextDouble();
                    randL = rng.NextDouble();
                }
                while (randH > 0.999d || randL > 0.999d);

                var randValue = (decimal)randH + ((decimal)randL / 1E14m);

                return randValue * ((convertedValue - 1) + 1);
            }
        }
    }
}
