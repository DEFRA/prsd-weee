namespace EA.Weee.Integration.Tests.Handlers
{
    using Autofac;
    using Base;
    using Builders;
    using Core.Shared;
    using Domain.Obligation;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Lookup;
    using NUnit.Specifications;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Domain.Scheme;
    using FluentAssertions;
    using Requests.AatfEvidence.Reports;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;

    public class GetSchemeObligationAndEvidenceTotalsReportHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetASchemesObligationAndEvidenceTotals : GetSchemeObligationAndEvidenceTotalsReportHandlerIntegrationTestBase
        {
            private static Scheme scheme;
            private readonly Establish context = () =>
            {
                LocalSetup();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

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
                ObligationSchemeDbSetup.Init()
                                       .WithScheme(scheme.Id)
                                       .WithObligationUpload(obligationUpload.Id)
                                       .WithObligationAmounts(obligationAmounts)
                                       .WithComplianceYear(2022)
                                       .Create();

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

                //tonnages 10 - shouldnt be counted as non household * THIS SHOULD BE INCLUDED IN THE REUSE !!!!!!!
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

                var recipientAsOriginatorTonnages = new List<NoteTonnage>()
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

                EvidenceNoteDbSetup.Init().WithOrganisation(recipientOrganisation.Id)
                    .WithTonnages(recipientAsOriginatorTonnages)
                    .WithWasteType(WasteType.HouseHold)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                request = new GetSchemeObligationAndEvidenceTotalsReportRequest(scheme.Id, null, 2022);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveTheExpectedData = () =>
            {
                result.FileContent.Should().Be($@"PCS name,PCS approval number,Category,Household obligation (tonnes),Household evidence (tonnes),Household reuse (tonnes),Transferred out (tonnes),Transferred In (tonnes),Difference (tonnes),Non household evidence (tonnes),Non household reuse (tonnes)
{scheme.SchemeName},{scheme.ApprovalNumber},1. Large household appliances,567.000,101.000,1.000,0.000,0.000,-466.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},2. Small household appliances,20.000,26.000,1.000,100.000,0.000,6.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},3. IT and telecommunications equipment,150.500,81.000,71.000,0.000,0.000,-69.500,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},4. Consumer equipment,100.000,155.000,2.000,1.000,5.000,55.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},5. Lighting equipment,0.000,68.280,1.000,0.000,57.280,68.280,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},6. Electrical and electronic tools,0.000,251.000,1.000,0.000,0.000,251.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},""7. Toys, leisure and sports equipment"",1000.235,76.000,21.000,0.000,0.000,-924.235,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},8. Medical devices,800.000,51.000,0.000,0.000,0.000,-749.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},9. Monitoring and control instruments,1.000,21.000,0.000,0.000,0.000,20.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},10. Automatic dispensers,600.000,21.000,6.000,0.000,10.000,-579.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},11. Display equipment,200.000,31.000,1.000,0.000,0.000,-169.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},12. Appliances containing refrigerants,0.000,201.789,101.000,0.000,0.000,201.789,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},13. Gas discharge lamps and LED light sources,0.000,201.000,11.000,0.000,0.000,201.000,0.000,0.000
{scheme.SchemeName},{scheme.ApprovalNumber},14. Photovoltaic panels,1000.000,101.000,51.000,0.000,0.000,-899.000,100.000,50.000
{scheme.SchemeName},{scheme.ApprovalNumber},Total (tonnes),7110.470,2136.349,359.000,202.000,144.560,-4974.121,100.000,50.000
{scheme.SchemeName},{scheme.ApprovalNumber},Category 2-10 summary,2671.735,750.280,97.000,101.000,72.280,-1921.455,0.000,0.000
");
            };
        }

        public class GetSchemeObligationAndEvidenceTotalsReportHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetSchemeObligationAndEvidenceTotalsReportRequest, CSVFileData> handler;
            protected static GetSchemeObligationAndEvidenceTotalsReportRequest request;
            protected static CSVFileData result;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings()
                    .WithInternalUserAccess(false);

                Query.SetupUserWithRole(UserId.ToString(), "Standard", CompetentAuthority.England);

                handler = Container.Resolve<IRequestHandler<GetSchemeObligationAndEvidenceTotalsReportRequest, CSVFileData>>();
            }
        }
    }
}
