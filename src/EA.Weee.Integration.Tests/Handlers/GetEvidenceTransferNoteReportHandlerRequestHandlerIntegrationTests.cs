namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.AatfEvidence;
    using Core.Admin;
    using Core.Constants;
    using Core.Shared;
    using Domain.Evidence;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence.Reports;
    using NoteStatus = Domain.Evidence.NoteStatus;
    using Protocol = Domain.Evidence.Protocol;
    using WasteType = Domain.Evidence.WasteType;

    public class GetTransferNoteReportHandlerRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetTheTransferNoteReportWithComplianceYear : GetTransferNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalInternalSetup();

                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();
                
                var note1Tonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1, 2),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 3, 4),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5, 6),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7, 8),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9, 10),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11, 12),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13, 14),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15, 16),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 17, 18),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 19, 20),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 21, 22),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 23, 24),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 25, 26),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 27, 28),
                };

                var note1 = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(note1Tonnages)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.SiteSpecificProtocol)
                    .Create();

                notes.Add(note1);

                var transferRecipientOrganisation = OrganisationDbSetup.Init().Create();
                var transferRecipientScheme =
                    SchemeDbSetup.Init().WithOrganisation(transferRecipientOrganisation.Id).Create();

                var newTransferNoteTonnages = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id, 1, 2),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id, 3, 4),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id, 5, 6),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id, 7, 8),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id, 9, 10),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id, 11, 12),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id, 13, 14),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id, 15, 16),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id, 17, 18),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id, 19, 20),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id, 21, 22),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id, 23, 24),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id, 25, 26),
                    new NoteTransferTonnage(note1.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id, 27, 28)
                };

                var transferNote1 = TransferEvidenceNoteDbSetup.Init()
                    .WithOrganisation(recipientOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(transferRecipientOrganisation.Id)
                    .WithTonnages(newTransferNoteTonnages).Create();

                transferNotes.Add(transferNote1);
                
                request = new GetTransferNoteReportRequest(SystemTime.Now.Year, null);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                //var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                //var note2 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                //var note2SubmittedDate = note2.NoteStatusHistory
                //    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                //var note3 = Query.GetEvidenceNoteById(notes.ElementAt(2).Id);
                //var note3SubmittedDate = note3.NoteStatusHistory
                //    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var transferNote1 = Query.GetTransferEvidenceNoteById(transferNotes.ElementAt(0).Id);
                var evidenceNote1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var evidenceNote1ApprovalDate = evidenceNote1.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Approved.Value).ChangedDate;

                var expectedCsvData =
                    "Transfer reference ID,Status,Transfer approval date (GMT),Transferred by name,Transferred by approval number,Recipient name,Recipient approval number,Evidence note reference ID,Evidence note approval date (GMT),AATF evidence issued by name,AATF evidence issued by approval number,Actual or protocol,Cat 1 transferred (t),Cat 2 transferred (t),Cat 3 transferred (t),Cat 4 transferred (t),Cat 5 transferred (t),Cat 6 transferred (t),Cat 7 transferred (t),Cat 8 transferred (t),Cat 9 transferred (t),Cat 10 transferred (t),Cat 11 transferred (t),Cat 12 transferred (t),Cat 13 transferred (t),Cat 14 transferred (t),Total transferred (t),Cat 1 reuse transferred (t),Cat 2 reuse transferred (t),Cat 3 reuse transferred (t),Cat 4 reuse transferred (t),Cat 5 reuse transferred (t),Cat 6 reuse transferred (t),Cat 7 reuse transferred (t),Cat 8 reuse transferred (t),Cat 9 reuse transferred (t),Cat 10 reuse transferred (t),Cat 11 reuse transferred (t),Cat 12 reuse transferred (t),Cat 13 reuse transferred (t),Cat 14 reuse transferred (t),Total reuse transferred (t)\r\n" +

                $"T2,Draft,,{transferNote1.Organisation.Scheme.SchemeName},{transferNote1.Organisation.Scheme.ApprovalNumber},{transferNote1.Recipient.Scheme.SchemeName},{transferNote1.Recipient.Scheme.ApprovalNumber},E{evidenceNote1.Reference},{evidenceNote1ApprovalDate},{evidenceNote1.Aatf.Name},{evidenceNote1.Aatf.ApprovalNumber},Site specific protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,196.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,210.000\r\n";

                    //$"E2,Submitted,{note2SubmittedDate},Household,{note2.StartDate.ToShortDateString()},{note2.EndDate.ToShortDateString()},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Actual,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +

                    //$"E3,Approved,{note3SubmittedDate},Household,{note3.StartDate.ToShortDateString()},{note3.EndDate.ToShortDateString()},{note3.Recipient.OrganisationName},,Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_Transfer notes report{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        public class GetTransferNoteReportHandlerRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetTransferNoteReportRequest, CSVFileData> handler;
            protected static GetTransferNoteReportRequest request;
            protected static CSVFileData result;
            protected static List<Note> notes;
            protected static List<Note> transferNotes;
            protected static Fixture fixture;
            protected static string aatfApprovalNumber;
            protected static string approvedSchemeName;

            public static IntegrationTestSetupBuilder LocalInternalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData(true)
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Standard", CompetentAuthority.England);

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetTransferNoteReportRequest, CSVFileData>>();

                notes = new List<Note>();
                transferNotes = new List<Note>();

                return setup;
            }

            public static IntegrationTestSetupBuilder LocalOrganisationSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData(true)
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetTransferNoteReportRequest, CSVFileData>>();

                notes = new List<Note>();
                transferNotes = new List<Note>();

                return setup;
            }
        }
    }
}
