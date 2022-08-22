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
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Requests.Shared;
    using FluentAssertions;
    using NUnit.Framework;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;

    public class GetObligationSummaryRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetASchemesObligationSummary : GetObligationSummaryRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                var scheme = SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                var obligationUpload = ObligationUploadDbSetup.Init().Create();
                //obligations
                var obligationAmounts = new List<ObligationSchemeAmount>()
                {
                    new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, 1000),
                    new ObligationSchemeAmount(WeeeCategory.MedicalDevices, 800),
                    new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, 0),
                    new ObligationSchemeAmount(WeeeCategory.ElectricalAndElectronicTools, null),
                    new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, 100),
                    new ObligationSchemeAmount(WeeeCategory.ToysLeisureAndSports, 1000.235M),
                    new ObligationSchemeAmount(WeeeCategory.AutomaticDispensers, 600),
                    new ObligationSchemeAmount(WeeeCategory.DisplayEquipment, 200),
                    new ObligationSchemeAmount(WeeeCategory.CoolingApplicancesContainingRefrigerants, null),
                    new ObligationSchemeAmount(WeeeCategory.SmallHouseholdAppliances, 20),
                    new ObligationSchemeAmount(WeeeCategory.LargeHouseholdAppliances, 567),
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 150.5M),
                    new ObligationSchemeAmount(WeeeCategory.LightingEquipment, null),
                    new ObligationSchemeAmount(WeeeCategory.MonitoringAndControlInstruments, 1),
                };
                ObligationSchemeDbSetup.Init().WithScheme(scheme.Id).WithObligationUpload(obligationUpload.Id).WithObligationAmounts(obligationAmounts).WithComplianceYear(2022).Create();

                //tonnages 1
                var tonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1, 1),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 1, 0),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 1, 1),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 1, 1),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 1, 1),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 1, 1),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 1, 1),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 1, 1),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 1, 1),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, 1),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 1, 1),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 1, 1),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 1, 0),
                };

                var note = EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(2022)
                    .WithTonnages(tonnages).Create();

                //tonnages 2
                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 50, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 200, 10),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 250, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 150, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 75, 20),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 10, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 30, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 200.789M, 100),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 125, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 100, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 80, 70),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 10, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 20, 0),
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(2022)
                    .WithTonnages(tonnages2).Create();

                //tonnages 3 - shouldnt be counted as not in the compliance year
                var tonnages3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50)
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(2023)
                    .WithTonnages(tonnages3).Create();

                //tonnages 4 - shouldnt be counted as not in the correct scheme year
                var tonnages4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 500, 50)
                };

                var recipientOrganisation2 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation2.Id).Create();
                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation2.Id)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(2023)
                    .WithTonnages(tonnages4).Create();

                //tonnages 5 - shouldnt be counted as submitted
                var tonnages5 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50)
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(2022)
                    .WithTonnages(tonnages5).Create();

                //tonnages 6 - shouldnt be counted as submitted
                var tonnages6 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50)
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithComplianceYear(2022)
                    .WithTonnages(tonnages6).Create();

                //tonnages 7 - shouldnt be counted as rejected
                var tonnages7 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50)
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Rejected, UserId.ToString())
                    .WithComplianceYear(2022)
                    .WithTonnages(tonnages7).Create();

                //tonnages 8 - shouldnt be counted as returned
                var tonnages8 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50)
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Returned, UserId.ToString())
                    .WithComplianceYear(2022)
                    .WithTonnages(tonnages8).Create();

                //tonnages 9 - shouldnt be counted as void
                var tonnages9 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50)
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Void, UserId.ToString())
                    .WithComplianceYear(2022)
                    .WithTonnages(tonnages9).Create();

                //tonnages 10 - shouldnt be counted as non household
                var tonnages10 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50)
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(2022)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithTonnages(tonnages10).Create();

                // create transfer in note 1
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id, 10, 5),
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id, 50, null)
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                {
                    t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddHours(1));
                }).WithTonnages(newTransferNoteTonnage1)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                // create transfer in note 2
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 5, 1),
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id, 7.28M, null)
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                    {
                        t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddHours(1));
                    }).WithTonnages(newTransferNoteTonnage2)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                // create transfer in note 3 not counted
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 5, 1),
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                    {
                        t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    }).WithTonnages(newTransferNoteTonnage3)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                // create transfer in note 4 not counted
                var newTransferNoteTonnage4 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 5, 1),
                };

                TransferEvidenceNoteDbSetup.Init().WithTonnages(newTransferNoteTonnage4)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                // create transfer in note 5 not counted
                var newTransferNoteTonnage5 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 5, 1),
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                    {
                        t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        t.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString(), SystemTime.UtcNow);
                    }).WithTonnages(newTransferNoteTonnage5)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                // create transfer in note 6 not counted
                var newTransferNoteTonnage6 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 5, 1),
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                    {
                        t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        t.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow);
                    }).WithTonnages(newTransferNoteTonnage6)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                // create transfer in note 7 not counted
                var newTransferNoteTonnage7 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 5, 1),
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                    {
                        t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                        t.UpdateStatus(NoteStatusDomain.Void, UserId.ToString(), SystemTime.UtcNow);
                    }).WithTonnages(newTransferNoteTonnage7)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                // create transfer in note 8 not counted
                var newTransferNoteTonnage8 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 5, 1),
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                    {
                        t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    }).WithTonnages(newTransferNoteTonnage8)
                    .WithRecipient(recipientOrganisation2.Id)
                    .Create();

                // create transfer in note 9 not counted
                var newTransferNoteTonnage9 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 5, 1),
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                    {
                        t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    }).WithTonnages(newTransferNoteTonnage9)
                    .WithComplianceYear(2023)
                    .WithRecipient(recipientOrganisation.Id)
                    .Create();

                // create transfer out
                var newTransferNoteOutTonnage = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 1, 0),
                    new NoteTransferTonnage(note.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id, 100, 0),
                };

                TransferEvidenceNoteDbSetup.Init().With(t =>
                    {
                        t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                    }).WithTonnages(newTransferNoteOutTonnage)
                    .WithOrganisation(recipientOrganisation.Id)
                    .WithRecipient(recipientOrganisation2.Id)
                    .Create();

                request = new GetObligationSummaryRequest(scheme.Id, 2022, true);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveTheExpectedData = () =>
            {
                var category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.PhotovoltaicPanels.ToInt());
                category.Obligation.Should().Be(1000);
                category.Evidence.Should().Be(101);
                category.Difference.Should().Be(-899);
                category.Reuse.Should().Be(51);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.MedicalDevices.ToInt());
                category.Obligation.Should().Be(800);
                category.Evidence.Should().Be(51);
                category.Difference.Should().Be(-749);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.GasDischargeLampsAndLedLightSources.ToInt());
                category.Obligation.Should().Be(0);
                category.Evidence.Should().Be(201);
                category.Difference.Should().Be(201);
                category.Reuse.Should().Be(11);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.ElectricalAndElectronicTools.ToInt());
                category.Obligation.Should().Be(0);
                category.Evidence.Should().Be(251);
                category.Difference.Should().Be(251);
                category.Reuse.Should().Be(1);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.ConsumerEquipment.ToInt());
                category.Obligation.Should().Be(100);
                category.Evidence.Should().Be(155);
                category.Difference.Should().Be(55);
                category.Reuse.Should().Be(2);
                category.TransferredIn.Should().Be(5);
                category.TransferredOut.Should().Be(1);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.ToysLeisureAndSports.ToInt());
                category.Obligation.Should().Be(1000.235M);
                category.Evidence.Should().Be(76);
                category.Difference.Should().Be(-924.235M);
                category.Reuse.Should().Be(21);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.AutomaticDispensers.ToInt());
                category.Obligation.Should().Be(600);
                category.Evidence.Should().Be(21);
                category.Difference.Should().Be(-579);
                category.Reuse.Should().Be(6);
                category.TransferredIn.Should().Be(10);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.DisplayEquipment.ToInt());
                category.Obligation.Should().Be(200);
                category.Evidence.Should().Be(31);
                category.Difference.Should().Be(-169);
                category.Reuse.Should().Be(1);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.CoolingApplicancesContainingRefrigerants.ToInt());
                category.Obligation.Should().Be(0);
                category.Evidence.Should().Be(201.789M);
                category.Difference.Should().Be(201.789M);
                category.Reuse.Should().Be(101);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.SmallHouseholdAppliances.ToInt());
                category.Obligation.Should().Be(20);
                category.Evidence.Should().Be(26);
                category.Difference.Should().Be(6);
                category.Reuse.Should().Be(1);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(100);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.LargeHouseholdAppliances.ToInt());
                category.Obligation.Should().Be(567);
                category.Evidence.Should().Be(101);
                category.Difference.Should().Be(-466);
                category.Reuse.Should().Be(1);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.ITAndTelecommsEquipment.ToInt());
                category.Obligation.Should().Be(150.5M);
                category.Evidence.Should().Be(81);
                category.Difference.Should().Be(-69.500M);
                category.Reuse.Should().Be(71);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.LightingEquipment.ToInt());
                category.Obligation.Should().Be(0);
                category.Evidence.Should().Be(68.280M);
                category.Difference.Should().Be(68.280M);
                category.Reuse.Should().Be(1);
                category.TransferredIn.Should().Be(57.280M);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.MonitoringAndControlInstruments.ToInt());
                category.Obligation.Should().Be(1);
                category.Evidence.Should().Be(21);
                category.Difference.Should().Be(20);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
            };
        }

        [Component]
        [Ignore("Time consuming data set only use for seeding of data")]
        public class WhenIGetASchemesObligationSummaryWithLotsOfData : GetObligationSummaryRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                // create some schemes for and some obligations over 20 years
                var schemesList = new List<Scheme>();
                var organisations = new List<Organisation>();
                var aatfs = new List<Aatf>();
                const int numberOfNotes = 2000;
                const int numberOfTransfers = 250;
                Random randomTransfer = new Random();
                Random randomAatf = new Random();
                Random tonnageRandom = new Random();

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
                            .WithComplianceYear(year).Create();
                    }

                    foreach (var organisation in organisations)
                    {
                        var notes = new List<Note>();
                        for (int i = 0; i < numberOfNotes; i++)
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

                            var newNote = EvidenceNoteDbSetup.Init().WithRecipient(organisation.Id)
                                .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                                .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                                .WithWasteType(WasteType.HouseHold)
                                .WithAatf(aatfs.ElementAt(randomAatf.Next(0, 49)).Id)
                                .WithComplianceYear(year)
                                .WithTonnages(tonnages2).Create();

                            notes.Add(newNote);
                        }

                        for (int i = 0; i < numberOfTransfers; i++)
                        {
                            // create transfer in note 1
                            var noteToTransfer = notes.ElementAt(randomTransfer.Next(0, numberOfNotes - 1));

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

                            var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                            {
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id, tonnage1,
                                    NextTonnage(tonnageRandom, tonnage1)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id, tonnage2,
                                    NextTonnage(tonnageRandom, tonnage2)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id, tonnage3,
                                    NextTonnage(tonnageRandom, tonnage3)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id, tonnage4,
                                    NextTonnage(tonnageRandom, tonnage4)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, tonnage5,
                                    NextTonnage(tonnageRandom, tonnage5)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id, tonnage6,
                                    NextTonnage(tonnageRandom, tonnage6)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id, tonnage7,
                                    NextTonnage(tonnageRandom, tonnage7)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id, tonnage8,
                                    NextTonnage(tonnageRandom, tonnage8)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id, tonnage9,
                                    NextTonnage(tonnageRandom, tonnage9)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id, tonnage10,
                                    NextTonnage(tonnageRandom, tonnage10)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id, tonnage11,
                                    NextTonnage(tonnageRandom, tonnage11)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id, tonnage12,
                                    NextTonnage(tonnageRandom, tonnage12)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id, tonnage13,
                                    NextTonnage(tonnageRandom, tonnage13)),
                                new NoteTransferTonnage(
                                    noteToTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id, tonnage14,
                                    NextTonnage(tonnageRandom, tonnage14)),
                            };

                            TransferEvidenceNoteDbSetup.Init().With(t =>
                                {
                                    t.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                                    t.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddHours(1));
                                }).WithTonnages(newTransferNoteTonnage1)
                                .WithWasteType(WasteType.HouseHold)
                                .WithComplianceYear(year)
                                .WithRecipient(organisation.Id)
                                .Create();
                        }
                    }
                }

                request = new GetObligationSummaryRequest(schemesList.ElementAt(0).Id, 2023, true);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveTheExpectedData = () =>
            {
            };
        }

        public class GetObligationSummaryRequestHandlerIntegrationTestBase : WeeeContextSpecification
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

            public static decimal NextTonnage(Random rng, decimal maxValue = 25)
            {
                double randH, randL;
                do
                {
                    randH = rng.NextDouble();
                    randL = rng.NextDouble();
                } 
                while (randH > 0.999d || randL > 0.999d);

                var randValue = (decimal)randH + ((decimal)randL / 1E14m);

                return randValue * ((maxValue - 1) + 1);
            }
        }
    }
}
