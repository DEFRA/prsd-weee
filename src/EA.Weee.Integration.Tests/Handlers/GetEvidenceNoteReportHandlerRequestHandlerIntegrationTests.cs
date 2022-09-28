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

    public class GetEvidenceNoteReportHandlerRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetTheEvidenceNoteReportWithNoOrganisationParametersAndOriginalTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalInternalSetup();

                //draft note
                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();

                var tonnages1 = new List<NoteTonnage>()
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

                var draftNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages1)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.SiteSpecificProtocol)
                    .Create();

                notes.Add(draftNote);

                //submitted note
                var recipientOrganisation2 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation2.Id).Create();
                var originatorOrganisation2 = OrganisationDbSetup.Init().Create();
                var aatf2 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation2.Id).Create();

                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, null, null),
                    new NoteTonnage(WeeeCategory.MedicalDevices, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, null, null),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, null, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, null, null),
                    new NoteTonnage(WeeeCategory.LightingEquipment, null, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, null, null),
                };

                var submitted = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf2.Id)
                    .WithTonnages(tonnages2)
                    .WithOrganisation(originatorOrganisation2.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation2.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.Actual)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .Create();

                notes.Add(submitted);

                //approved note
                var recipientOrganisation3 = Query.GetBalancingSchemeId();
                var originatorOrganisation3 = OrganisationDbSetup.Init().Create();
                var aatf3 = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .Create();

                var tonnages3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approved = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf3.Id)
                    .WithTonnages(tonnages3)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation3)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approved);

                //approved note with approved changes, should show those stored on note
                approvedSchemeName = fixture.Create<string>();
                var recipientOrganisation4 = OrganisationDbSetup.Init().Create();
                var schemeApproved2 = SchemeDbSetup.Init().WithOrganisation(recipientOrganisation4.Id).Create();
                var originatorOrganisation4 = OrganisationDbSetup.Init().Create();
                var aatf4 = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(originatorOrganisation4.Id)
                    .Create();

                var tonnages4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedNote2 = EvidenceNoteDbSetup.Init()
                    .WithApprovedSchemeName(approvedSchemeName)
                    .WithAatf(aatf4.Id)
                    .WithTonnages(tonnages4)
                    .WithOrganisation(originatorOrganisation4.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation4.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedNote2);

                // out of compliance year
                EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                request = new GetEvidenceNoteReportRequest(null, null, null, TonnageToDisplayReportEnum.OriginalTonnages, SystemTime.Now.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var note2 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                var note2SubmittedDate = note2.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var note3 = Query.GetEvidenceNoteById(notes.ElementAt(2).Id);
                var note3SubmittedDate = note3.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var note4 = Query.GetEvidenceNoteById(notes.ElementAt(3).Id);
                var note4SubmittedDate = note4.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var expectedCsvData =
                    "Reference ID,Status,AATF appropriate authority,Date submitted (GMT),Submitted by AATF site name,AATF approval number,Obligation type,WEEE received start date,WEEE received end date,Recipient name,Recipient approval number,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" + 
                    
                    $"E1,Draft,EA,,{note1.Aatf.Name},{note1.Aatf.ApprovalNumber},Non-household,{note1.StartDate.ToShortDateString()},{note1.EndDate.ToShortDateString()},{note1.Recipient.Scheme.SchemeName},{note1.Recipient.Scheme.ApprovalNumber},Site specific protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,196.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,210.000\r\n" +

                    $"E2,Submitted,EA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate.ToShortDateString()},{note2.EndDate.ToShortDateString()},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Actual,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +

                    $"E3,Approved,SEPA,{note3SubmittedDate},{note3.Aatf.Name},{note3.Aatf.ApprovalNumber},Household,{note3.StartDate.ToShortDateString()},{note3.EndDate.ToShortDateString()},{note3.Recipient.Name},,Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n" +

                    $"E4,Approved,SEPA,{note4SubmittedDate},{note4.Aatf.Name},{note4.Aatf.ApprovalNumber},Household,{note4.StartDate.ToShortDateString()},{note4.EndDate.ToShortDateString()},{approvedSchemeName},{note4.Recipient.Scheme.ApprovalNumber},Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_Evidence notes original tonnages{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetTheEvidenceNoteReportWithAatfParameterAndOriginalTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalOrganisationSetup();

                //draft note
                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, originatorOrganisation1.Id).Create();

                aatfApprovalNumber = aatf1.ApprovalNumber;

                var tonnages1 = new List<NoteTonnage>()
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

                var draftNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages1)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.SiteSpecificProtocol)
                    .Create();

                notes.Add(draftNote);

                //submitted note
                var recipientOrganisation2 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation2.Id).Create();

                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, null, null),
                    new NoteTonnage(WeeeCategory.MedicalDevices, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, null, null),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, null, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, null, null),
                    new NoteTonnage(WeeeCategory.LightingEquipment, null, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, null, null),
                };

                var submitted = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages2)
                    .WithOrganisation(aatf1.OrganisationId)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation2.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.Actual)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .Create();

                notes.Add(submitted);

                //approved note
                var recipientOrganisation3 = Query.GetBalancingSchemeId();
                var tonnages3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approved = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages3)
                    .WithOrganisation(aatf1.OrganisationId)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation3)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approved);

                EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                var notMatchingTonnage = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var notMatchingNote = EvidenceNoteDbSetup.Init()
                    .WithTonnages(notMatchingTonnage)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation3)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(notMatchingNote);

                request = new GetEvidenceNoteReportRequest(null, null, aatf1.Id, TonnageToDisplayReportEnum.OriginalTonnages, SystemTime.Now.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var note2 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                var note2SubmittedDate = note2.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var note3 = Query.GetEvidenceNoteById(notes.ElementAt(2).Id);
                var note3SubmittedDate = note3.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var expectedCsvData =
                    "Reference ID,Status,Date submitted (GMT),Obligation type,WEEE received start date,WEEE received end date,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E1,Draft,,Non-household,{note1.StartDate.ToShortDateString()},{note1.EndDate.ToShortDateString()},Site specific protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,196.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,210.000\r\n" +

                    $"E2,Submitted,{note2SubmittedDate},Household,{note2.StartDate.ToShortDateString()},{note2.EndDate.ToShortDateString()},Actual,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +

                    $"E3,Approved,{note3SubmittedDate},Household,{note3.StartDate.ToShortDateString()},{note3.EndDate.ToShortDateString()},Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_{aatfApprovalNumber}_Evidence notes report{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetTheEvidenceNoteReportWithNoOrganisationParametersAndNetTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalInternalSetup();

                //draft note
                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();

                var tonnages1 = new List<NoteTonnage>()
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

                var draftNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages1)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.SiteSpecificProtocol)
                    .Create();

                notes.Add(draftNote);

                //submitted note
                var recipientOrganisation2 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation2.Id).Create();
                var originatorOrganisation2 = OrganisationDbSetup.Init().Create();
                var aatf2 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation2.Id).Create();

                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, null, null),
                    new NoteTonnage(WeeeCategory.MedicalDevices, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, null, null),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, null, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, null, null),
                    new NoteTonnage(WeeeCategory.LightingEquipment, null, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, null, null),
                };

                var submitted = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf2.Id)
                    .WithTonnages(tonnages2)
                    .WithOrganisation(originatorOrganisation2.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation2.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.Actual)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .Create();

                notes.Add(submitted);

                //approved note with no transfer notes
                var recipientOrganisation3 = Query.GetBalancingSchemeId();
                var originatorOrganisation3 = OrganisationDbSetup.Init().Create();
                var aatf3 = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .Create();

                var tonnages3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approved = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf3.Id)
                    .WithTonnages(tonnages3)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation3)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approved);

                //approved note with transfer notes
                var recipientOrganisation4 = Query.GetBalancingSchemeId();
                var originatorOrganisation4 = OrganisationDbSetup.Init().Create();
                var aatf4 = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .Create();

                var tonnages4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedWithTransfer = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf4.Id)
                    .WithTonnages(tonnages4)
                    .WithOrganisation(originatorOrganisation4.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation4)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedWithTransfer);

                // create transfer from approved note
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id, 
                        0.1M, 0.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        0.3M, 0.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        0.5M, 0.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        0.7M, 0.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        0.9M, 1.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        2.1M, 3.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        4.3M, 5.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        6.5M, 7.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        8.7M, 9.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        10.9M, 11.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        12.2M, 13.3M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        14.4M, 15.5M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        16.6M, 17.7M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        18.8M, 19.9M),
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(newTransferNoteTonnage1).Create();

                // create transfer from submitted note so wont be included in the total
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        0.1M, 0.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        0.3M, 0.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        0.5M, 0.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        0.7M, 0.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        0.9M, 1.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        2.1M, 3.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        4.3M, 5.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        6.5M, 7.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        8.7M, 9.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        10.9M, 11.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        12.2M, 13.3M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        14.4M, 15.5M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        16.6M, 17.7M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        18.8M, 19.9M),
                };

                TransferEvidenceNoteDbSetup.Init().WithStatus(NoteStatus.Submitted, UserId.ToString()).WithTonnages(newTransferNoteTonnage2).Create();

                // create approved transfer with null values against approved note
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        null, null),
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(newTransferNoteTonnage3).Create();

                //approved note with approved scheme note details
                approvedSchemeName = fixture.Create<string>();
                var approvedNoteWithSchemeNameUpdateOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(approvedNoteWithSchemeNameUpdateOrganisation.Id).Create();
                var approvedNoteWithOriginatorOrganisation = OrganisationDbSetup.Init().Create();
                var aatfForNoteWithSchemeNameUpdate = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(approvedNoteWithOriginatorOrganisation.Id)
                    .Create();

                var approvedNoteWithSchemeNameUpdateTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedNoteWithSchemeNameUpdate = EvidenceNoteDbSetup.Init()
                    .WithApprovedSchemeName(approvedSchemeName)
                    .WithAatf(aatfForNoteWithSchemeNameUpdate.Id)
                    .WithTonnages(approvedNoteWithSchemeNameUpdateTonnages)
                    .WithOrganisation(approvedNoteWithOriginatorOrganisation.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(approvedNoteWithSchemeNameUpdateOrganisation.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedNoteWithSchemeNameUpdate);

                EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                request = new GetEvidenceNoteReportRequest(null, null, null, TonnageToDisplayReportEnum.Net, SystemTime.Now.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var note2 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                var note2SubmittedDate = note2.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var note3 = Query.GetEvidenceNoteById(notes.ElementAt(2).Id);
                var note3SubmittedDate = note3.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var note4 = Query.GetEvidenceNoteById(notes.ElementAt(3).Id);
                var note4SubmittedDate = note4.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var note5 = Query.GetEvidenceNoteById(notes.ElementAt(4).Id);
                var note5SubmittedDate = note5.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var expectedCsvData =
                    "Reference ID,Status,AATF appropriate authority,Date submitted (GMT),Submitted by AATF site name,AATF approval number,Obligation type,WEEE received start date,WEEE received end date,Recipient name,Recipient approval number,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E1,Draft,EA,,{note1.Aatf.Name},{note1.Aatf.ApprovalNumber},Non-household,{note1.StartDate.ToShortDateString()},{note1.EndDate.ToShortDateString()},{note1.Recipient.Scheme.SchemeName},{note1.Recipient.Scheme.ApprovalNumber},Site specific protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,196.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,210.000\r\n" +

                    $"E2,Submitted,EA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate.ToShortDateString()},{note2.EndDate.ToShortDateString()},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Actual,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +

                    $"E3,Approved,SEPA,{note3SubmittedDate},{note3.Aatf.Name},{note3.Aatf.ApprovalNumber},Household,{note3.StartDate.ToShortDateString()},{note3.EndDate.ToShortDateString()},{note3.Recipient.Name},,Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n" +

                    $"E4,Approved,SEPA,{note4SubmittedDate},{note4.Aatf.Name},{note4.Aatf.ApprovalNumber},Household,{note4.StartDate.ToShortDateString()},{note4.EndDate.ToShortDateString()},{note4.Recipient.Name},,Reuse network PWP,10.244,9.322,10.266,9.099,10.288,7.077,9.122,2.033,9.311,9.144,9.166,10.299,5.055,1.011,111.437,10.255,10.233,10.277,9.011,10.299,8.088,9.133,4.044,9.322,9.155,10.177,9.311,6.066,2.022,117.393\r\n" +

                    $"E8,Approved,SEPA,{note5SubmittedDate},{note5.Aatf.Name},{note5.Aatf.ApprovalNumber},Household,{note5.StartDate.ToShortDateString()},{note5.EndDate.ToShortDateString()},{approvedSchemeName},{note5.Recipient.Scheme.ApprovalNumber},Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_Evidence notes net of transfer{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetTheEvidenceNoteReportWithOriginatingOrganisationAndOriginalTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalOrganisationSetup();

                //draft note
                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, originatorOrganisation1.Id).Create();

                var tonnages1 = new List<NoteTonnage>()
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

                var draftNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages1)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.LdaProtocol)
                    .Create();

                notes.Add(draftNote);

                //submitted note
                var recipientOrganisation2 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation2.Id).Create();

                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var submitted = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages2)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation2.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.Actual)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .Create();

                notes.Add(submitted);

                //note associated with a different organisation
                var originatorOrganisation3 = OrganisationDbSetup.Init().Create();

                EvidenceNoteDbSetup.Init()
                    .WithOrganisation(originatorOrganisation3.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .Create();

                // out of compliance year not included
                EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                //approved note with approved scheme name
                approvedSchemeName = fixture.Create<string>();
                var approvedNoteWithSchemeNameUpdateOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(approvedNoteWithSchemeNameUpdateOrganisation.Id).Create();

                var approvedNoteWithSchemeNameUpdateTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedNoteWithSchemeNameUpdate = EvidenceNoteDbSetup.Init()
                    .WithApprovedSchemeName(approvedSchemeName)
                    .WithAatf(aatf1.Id)
                    .WithTonnages(approvedNoteWithSchemeNameUpdateTonnages)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(approvedNoteWithSchemeNameUpdateOrganisation.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedNoteWithSchemeNameUpdate);

                request = new GetEvidenceNoteReportRequest(null, originatorOrganisation1.Id, null, TonnageToDisplayReportEnum.OriginalTonnages, SystemTime.Now.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var note2 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                var note2SubmittedDate = note2.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var note3 = Query.GetEvidenceNoteById(notes.ElementAt(2).Id);
                var note3SubmittedDate = note3.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var expectedCsvData =
                    "Reference ID,Status,AATF appropriate authority,Date submitted (GMT),Submitted by AATF site name,AATF approval number,Obligation type,WEEE received start date,WEEE received end date,Recipient name,Recipient approval number,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E1,Draft,EA,,{note1.Aatf.Name},{note1.Aatf.ApprovalNumber},Non-household,{note1.StartDate.ToShortDateString()},{note1.EndDate.ToShortDateString()},{note1.Recipient.Scheme.SchemeName},{note1.Recipient.Scheme.ApprovalNumber},LDA protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,196.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,210.000\r\n" +

                    $"E2,Submitted,EA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate.ToShortDateString()},{note2.EndDate.ToShortDateString()},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Actual,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n" +

                    $"E5,Approved,EA,{note3SubmittedDate},{note3.Aatf.Name},{note3.Aatf.ApprovalNumber},Household,{note3.StartDate.ToShortDateString()},{note3.EndDate.ToShortDateString()},{approvedSchemeName},{note3.Recipient.Scheme.ApprovalNumber},Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_Evidence notes original tonnages{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetTheEvidenceNoteReportWithOriginatingOrganisationAndNetTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalOrganisationSetup();

                //draft note
                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, originatorOrganisation1.Id).Create();

                var tonnages1 = new List<NoteTonnage>()
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

                var draftNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages1)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.SiteSpecificProtocol)
                    .Create();

                notes.Add(draftNote);

                //submitted note against a different organisation
                var recipientOrganisation2 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation2.Id).Create();
                var originatorOrganisation2 = OrganisationDbSetup.Init().Create();
                var aatf2 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation2.Id).Create();

                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, null, null),
                    new NoteTonnage(WeeeCategory.MedicalDevices, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, null, null),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, null, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, null, null),
                    new NoteTonnage(WeeeCategory.LightingEquipment, null, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, null, null),
                };

                EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf2.Id)
                    .WithTonnages(tonnages2)
                    .WithOrganisation(originatorOrganisation2.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation2.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.Actual)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .Create();

                //approved note with no transfer notes
                var tonnages3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approved = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages3)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation2.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approved);

                //approved note with transfer notes
                var recipientOrganisation4 = Query.GetBalancingSchemeId();
                var aatf4 = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .Create();

                var tonnages4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedWithTransfer = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf4.Id)
                    .WithTonnages(tonnages4)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation4)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedWithTransfer);

                // create transfer from approved note
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        0.1M, 0.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        0.3M, 0.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        0.5M, 0.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        0.7M, 0.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        0.9M, 1.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        2.1M, 3.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        4.3M, 5.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        6.5M, 7.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        8.7M, 9.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        10.9M, 11.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        12.2M, 13.3M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        14.4M, 15.5M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        16.6M, 17.7M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        18.8M, 19.9M),
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(newTransferNoteTonnage1).Create();

                // create submitted transfer from approved note so wont be included in the total
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        0.1M, 0.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        0.3M, 0.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        0.5M, 0.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        0.7M, 0.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        0.9M, 1.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        2.1M, 3.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        4.3M, 5.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        6.5M, 7.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        8.7M, 9.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        10.9M, 11.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        12.2M, 13.3M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        14.4M, 15.5M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        16.6M, 17.7M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        18.8M, 19.9M),
                };

                TransferEvidenceNoteDbSetup.Init().WithStatus(NoteStatus.Submitted, UserId.ToString()).WithTonnages(newTransferNoteTonnage2).Create();

                // create another approved transfer from note 1 with null values
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        null, null),
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(newTransferNoteTonnage3).Create();

                EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();
                EvidenceNoteDbSetup.Init().Create();

                //approved note with approved scheme name
                approvedSchemeName = fixture.Create<string>();
                var approvedNoteWithSchemeNameUpdateOrganisation = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(approvedNoteWithSchemeNameUpdateOrganisation.Id).Create();

                var approvedNoteWithSchemeNameUpdateTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedNoteWithSchemeNameUpdate = EvidenceNoteDbSetup.Init()
                    .WithApprovedSchemeName(approvedSchemeName)
                    .WithAatf(aatf1.Id)
                    .WithTonnages(approvedNoteWithSchemeNameUpdateTonnages)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(approvedNoteWithSchemeNameUpdateOrganisation.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedNoteWithSchemeNameUpdate);

                request = new GetEvidenceNoteReportRequest(null, originatorOrganisation1.Id, null, TonnageToDisplayReportEnum.Net, SystemTime.Now.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var note2 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                var note3 = Query.GetEvidenceNoteById(notes.ElementAt(2).Id);
                var note4 = Query.GetEvidenceNoteById(notes.ElementAt(3).Id);
                var note2SubmittedDate = note2.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;
                var note3SubmittedDate = note3.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;
                var note4SubmittedDate = note4.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var expectedCsvData =
                    "Reference ID,Status,AATF appropriate authority,Date submitted (GMT),Submitted by AATF site name,AATF approval number,Obligation type,WEEE received start date,WEEE received end date,Recipient name,Recipient approval number,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E1,Draft,EA,,{note1.Aatf.Name},{note1.Aatf.ApprovalNumber},Non-household,{note1.StartDate.ToShortDateString()},{note1.EndDate.ToShortDateString()},{note1.Recipient.Scheme.SchemeName},{note1.Recipient.Scheme.ApprovalNumber},Site specific protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,196.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,210.000\r\n" +

                    $"E3,Approved,EA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate.ToShortDateString()},{note2.EndDate.ToShortDateString()},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n" +

                    $"E4,Approved,SEPA,{note3SubmittedDate},{note3.Aatf.Name},{note3.Aatf.ApprovalNumber},Household,{note3.StartDate.ToShortDateString()},{note3.EndDate.ToShortDateString()},{note3.Recipient.Name},,Reuse network PWP,10.244,9.322,10.266,9.099,10.288,7.077,9.122,2.033,9.311,9.144,9.166,10.299,5.055,1.011,111.437,10.255,10.233,10.277,9.011,10.299,8.088,9.133,4.044,9.322,9.155,10.177,9.311,6.066,2.022,117.393\r\n" +

                    $"E10,Approved,EA,{note4SubmittedDate},{note4.Aatf.Name},{note4.Aatf.ApprovalNumber},Household,{note4.StartDate.ToShortDateString()},{note4.EndDate.ToShortDateString()},{approvedSchemeName},{note4.Recipient.Scheme.ApprovalNumber},Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_Evidence notes net of transfer{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetTheEvidenceNoteReportWithRecipientOrganisationAndOriginalTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalOrganisationSetup();

                //draft note should not be included for the recipient
                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation1.Id).Create();

                var tonnages1 = new List<NoteTonnage>()
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

                var draftNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages1)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.LdaProtocol)
                    .Create();

                notes.Add(draftNote);

                //submitted note
                var originatorOrganisation2 = OrganisationDbSetup.Init().Create();
                var aatf2 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation2.Id).Create();

                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var submitted = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf2.Id)
                    .WithTonnages(tonnages2)
                    .WithOrganisation(originatorOrganisation2.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.Actual)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .Create();

                notes.Add(submitted);

                //returned note
                var tonnages3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, null, null),
                    new NoteTonnage(WeeeCategory.MedicalDevices, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, null, null),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, null, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, null, null),
                    new NoteTonnage(WeeeCategory.LightingEquipment, null, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, null, null),
                };

                var returned = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf2.Id)
                    .WithTonnages(tonnages3)
                    .WithOrganisation(originatorOrganisation2.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Returned, UserId.ToString())
                    .Create();

                notes.Add(returned);

                //note associated with a different organisation
                var recipientOrganisation3 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation3.Id).Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation3.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .Create();

                EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                //approved note with approved scheme name
                approvedSchemeName = fixture.Create<string>();
                var approvedNoteWithOriginatorOrganisation = OrganisationDbSetup.Init().Create();
                var aatfForNoteWithSchemeNameUpdate = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(approvedNoteWithOriginatorOrganisation.Id)
                    .Create();

                var approvedNoteWithSchemeNameUpdateTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedNoteWithSchemeNameUpdate = EvidenceNoteDbSetup.Init()
                    .WithApprovedSchemeName(approvedSchemeName)
                    .WithAatf(aatfForNoteWithSchemeNameUpdate.Id)
                    .WithTonnages(approvedNoteWithSchemeNameUpdateTonnages)
                    .WithOrganisation(approvedNoteWithOriginatorOrganisation.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedNoteWithSchemeNameUpdate);

                request = new GetEvidenceNoteReportRequest(recipientOrganisation1.Id, null, null, TonnageToDisplayReportEnum.OriginalTonnages, SystemTime.Now.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var note2 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                var note3 = Query.GetEvidenceNoteById(notes.ElementAt(2).Id);
                var note4 = Query.GetEvidenceNoteById(notes.ElementAt(3).Id);
                var note2SubmittedDate = note2.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;
                var note3SubmittedDate = note3.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;
                var note4SubmittedDate = note4.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var expectedCsvData =
                    "Reference ID,Status,AATF appropriate authority,Date submitted (GMT),Submitted by AATF site name,AATF approval number,Obligation type,WEEE received start date,WEEE received end date,Recipient name,Recipient approval number,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E2,Submitted,EA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate.ToShortDateString()},{note2.EndDate.ToShortDateString()},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Actual,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n" +

                    $"E3,Returned,EA,{note3SubmittedDate},{note3.Aatf.Name},{note3.Aatf.ApprovalNumber},Non-household,{note3.StartDate.ToShortDateString()},{note3.EndDate.ToShortDateString()},{note3.Recipient.Scheme.SchemeName},{note3.Recipient.Scheme.ApprovalNumber},Reuse network PWP,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +

                    $"E6,Approved,SEPA,{note4SubmittedDate},{note4.Aatf.Name},{note4.Aatf.ApprovalNumber},Household,{note4.StartDate.ToShortDateString()},{note4.EndDate.ToShortDateString()},{approvedSchemeName},{note4.Recipient.Scheme.ApprovalNumber},Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_{note1.Recipient.Scheme.ApprovalNumber}_Evidence notes original tonnages{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetTheEvidenceNoteReportWithRecipientOrganisationAndNetTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalOrganisationSetup();

                //draft note should not be included for the recipient
                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, recipientOrganisation1.Id).Create();

                var tonnages1 = new List<NoteTonnage>()
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

                var draftNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages1)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.SiteSpecificProtocol)
                    .Create();

                notes.Add(draftNote);

                //submitted note
                var recipientOrganisation2 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation2.Id).Create();
                var originatorOrganisation2 = OrganisationDbSetup.Init().Create();
                var aatf2 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation2.Id).Create();

                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, null, null),
                    new NoteTonnage(WeeeCategory.MedicalDevices, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, null, null),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, null, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, null, null),
                    new NoteTonnage(WeeeCategory.LightingEquipment, null, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, null, null),
                };

                EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf2.Id)
                    .WithTonnages(tonnages2)
                    .WithOrganisation(originatorOrganisation2.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation2.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.Actual)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .Create();

                //approved note with no transfer notes
                var recipientOrganisation3 = Query.GetBalancingSchemeId();
                var originatorOrganisation3 = OrganisationDbSetup.Init().Create();
                var aatf3 = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .Create();

                var tonnages3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf3.Id)
                    .WithTonnages(tonnages3)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation3)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                //approved note with transfer notes
                var originatorOrganisation4 = OrganisationDbSetup.Init().Create();
                var aatf4 = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .Create();

                var tonnages4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedWithTransfer = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf4.Id)
                    .WithTonnages(tonnages4)
                    .WithOrganisation(originatorOrganisation4.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedWithTransfer);

                // create transfer from approved note
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        0.1M, 0.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        0.3M, 0.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        0.5M, 0.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        0.7M, 0.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        0.9M, 1.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        2.1M, 3.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        4.3M, 5.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        6.5M, 7.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        8.7M, 9.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        10.9M, 11.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        12.2M, 13.3M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        14.4M, 15.5M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        16.6M, 17.7M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        18.8M, 19.9M),
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(newTransferNoteTonnage1).Create();

                // create transfer from submitted note so wont be included in the total
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        0.1M, 0.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        0.3M, 0.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        0.5M, 0.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        0.7M, 0.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        0.9M, 1.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        2.1M, 3.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        4.3M, 5.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        6.5M, 7.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        8.7M, 9.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        10.9M, 11.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        12.2M, 13.3M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        14.4M, 15.5M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        16.6M, 17.7M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        18.8M, 19.9M),
                };

                TransferEvidenceNoteDbSetup.Init().WithStatus(NoteStatus.Submitted, UserId.ToString()).WithTonnages(newTransferNoteTonnage2).Create();

                // create approved transfer with null values against approved note
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        null, null),
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(newTransferNoteTonnage3).Create();

                EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                // should not be part of the CSV
                EvidenceNoteDbSetup.Init().Create();

                var returnNoteTonnages = new List<NoteTonnage>()
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

                var returnedNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(returnNoteTonnages)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.SiteSpecificProtocol)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Returned, UserId.ToString())
                    .Create();

                notes.Add(returnedNote);

                //approved note with approved scheme name
                approvedSchemeName = fixture.Create<string>();
                var approvedNoteWithOriginatorOrganisation = OrganisationDbSetup.Init().Create();
                var aatfForNoteWithSchemeNameUpdate = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(approvedNoteWithOriginatorOrganisation.Id)
                    .Create();

                var approvedNoteWithSchemeNameUpdateTonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedNoteWithSchemeNameUpdate = EvidenceNoteDbSetup.Init()
                    .WithApprovedSchemeName(approvedSchemeName)
                    .WithAatf(aatfForNoteWithSchemeNameUpdate.Id)
                    .WithTonnages(approvedNoteWithSchemeNameUpdateTonnages)
                    .WithOrganisation(approvedNoteWithOriginatorOrganisation.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedNoteWithSchemeNameUpdate);

                request = new GetEvidenceNoteReportRequest(recipientOrganisation1.Id, null, null, TonnageToDisplayReportEnum.Net, SystemTime.Now.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var note2 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                var note3 = Query.GetEvidenceNoteById(notes.ElementAt(2).Id);
                var note4 = Query.GetEvidenceNoteById(notes.ElementAt(3).Id);
                var note2SubmittedDate = note2.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;
                var note3SubmittedDate = note3.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;
                var note4SubmittedDate = note4.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var expectedCsvData =
                    "Reference ID,Status,AATF appropriate authority,Date submitted (GMT),Submitted by AATF site name,AATF approval number,Obligation type,WEEE received start date,WEEE received end date,Recipient name,Recipient approval number,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E4,Approved,SEPA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate.ToShortDateString()},{note2.EndDate.ToShortDateString()},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Reuse network PWP,10.244,9.322,10.266,9.099,10.288,7.077,9.122,2.033,9.311,9.144,9.166,10.299,5.055,1.011,111.437,10.255,10.233,10.277,9.011,10.299,8.088,9.133,4.044,9.322,9.155,10.177,9.311,6.066,2.022,117.393\r\n" +

                    $"E10,Returned,EA,{note3SubmittedDate},{note3.Aatf.Name},{note3.Aatf.ApprovalNumber},Non-household,{note3.StartDate.ToShortDateString()},{note3.EndDate.ToShortDateString()},{note3.Recipient.Scheme.SchemeName},{note3.Recipient.Scheme.ApprovalNumber},Site specific protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,196.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,210.000\r\n" +

                    $"E11,Approved,SEPA,{note4SubmittedDate},{note4.Aatf.Name},{note4.Aatf.ApprovalNumber},Household,{note4.StartDate.ToShortDateString()},{note4.EndDate.ToShortDateString()},{approvedSchemeName},{note4.Recipient.Scheme.ApprovalNumber},Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,208.437,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,223.993\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_{note1.Recipient.Scheme.ApprovalNumber}_Evidence notes net of transfer{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetTheEvidenceNoteReportWithAatfParameterAndNetTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalOrganisationSetup();

                //draft note
                var recipientOrganisation1 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation1.Id).Create();
                var originatorOrganisation1 = OrganisationDbSetup.Init().Create();
                var aatf1 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation1.Id).Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, originatorOrganisation1.Id).Create();

                aatfApprovalNumber = aatf1.ApprovalNumber;

                var tonnages1 = new List<NoteTonnage>()
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

                var draftNote = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages1)
                    .WithOrganisation(originatorOrganisation1.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation1.Id)
                    .WithWasteType(WasteType.NonHouseHold)
                    .WithProtocol(Protocol.SiteSpecificProtocol)
                    .Create();

                notes.Add(draftNote);

                //submitted note
                var recipientOrganisation2 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation2.Id).Create();
                var originatorOrganisation2 = OrganisationDbSetup.Init().Create();
                var aatf2 = AatfDbSetup.Init().WithOrganisation(originatorOrganisation2.Id).Create();

                var tonnages2 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, null, null),
                    new NoteTonnage(WeeeCategory.MedicalDevices, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, null, null),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, null, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, null, null),
                    new NoteTonnage(WeeeCategory.LightingEquipment, null, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, null, null),
                };

                var submitted = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf2.Id)
                    .WithTonnages(tonnages2)
                    .WithOrganisation(originatorOrganisation2.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation2.Id)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.Actual)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .Create();

                //approved note with no transfer notes
                var recipientOrganisation3 = Query.GetBalancingSchemeId();
                var originatorOrganisation3 = OrganisationDbSetup.Init().Create();
                var aatf3 = AatfDbSetup.Init()
                    .WithAppropriateAuthority(CompetentAuthority.Scotland)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .Create();

                var tonnages3 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf3.Id)
                    .WithTonnages(tonnages3)
                    .WithOrganisation(originatorOrganisation3.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation3)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                //approved note with transfer notes
                var recipientOrganisation4 = Query.GetBalancingSchemeId();

                var tonnages4 = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1.111M, 2.222M),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 2.333M, 4.444M),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5.555M, 6.666M),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 7.777M, 8.888M),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 9.999M, 10.111M),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 11.222M, 12.333M),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 13.444M, 14.555M),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 15.666M, 17.777M),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 18.999M, 19.111M),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 20.222M, 21.333M),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 22.444M, 23.555M),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 24.666M, 25.777M),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 26.888M, 27.999M),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 28.111M, 29.222M),
                };

                var approvedWithTransfer = EvidenceNoteDbSetup.Init()
                    .WithAatf(aatf1.Id)
                    .WithTonnages(tonnages4)
                    .WithOrganisation(aatf1.OrganisationId)
                    .WithComplianceYear(SystemTime.Now.Year)
                    .WithRecipient(recipientOrganisation4)
                    .WithWasteType(WasteType.HouseHold)
                    .WithProtocol(Protocol.ReuseNetworkPwp)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .Create();

                notes.Add(approvedWithTransfer);

                // create transfer from approved note
                var newTransferNoteTonnage1 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        0.1M, 0.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        0.3M, 0.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        0.5M, 0.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        0.7M, 0.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        0.9M, 1.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        2.1M, 3.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        4.3M, 5.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        6.5M, 7.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        8.7M, 9.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        10.9M, 11.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        12.2M, 13.3M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        14.4M, 15.5M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        16.6M, 17.7M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        18.8M, 19.9M),
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(newTransferNoteTonnage1).Create();

                // create transfer from submitted note so wont be included in the total
                var newTransferNoteTonnage2 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        0.1M, 0.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        0.3M, 0.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        0.5M, 0.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        0.7M, 0.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        0.9M, 1.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        2.1M, 3.2M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        4.3M, 5.4M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        6.5M, 7.6M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        8.7M, 9.8M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        10.9M, 11.1M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        12.2M, 13.3M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        14.4M, 15.5M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        16.6M, 17.7M),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        18.8M, 19.9M),
                };

                TransferEvidenceNoteDbSetup.Init().WithStatus(NoteStatus.Submitted, UserId.ToString()).WithTonnages(newTransferNoteTonnage2).Create();

                // create approved transfer with null values against approved note
                var newTransferNoteTonnage3 = new List<NoteTransferTonnage>()
                {
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MedicalDevices)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.LightingEquipment)).Id,
                        null, null),
                    new NoteTransferTonnage(approvedWithTransfer.NoteTonnage.First(nt => nt.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Id,
                        null, null),
                };

                TransferEvidenceNoteDbSetup.Init()
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithTonnages(newTransferNoteTonnage3).Create();

                EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();
                EvidenceNoteDbSetup.Init().Create();

                request = new GetEvidenceNoteReportRequest(null, null, aatf1.Id, TonnageToDisplayReportEnum.Net, SystemTime.Now.Year);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var note1 = Query.GetEvidenceNoteById(notes.ElementAt(0).Id);
                var note4 = Query.GetEvidenceNoteById(notes.ElementAt(1).Id);
                var note4SubmittedDate = note4.NoteStatusHistory
                    .First(s => s.ToStatus.Value == NoteStatus.Submitted.Value).ChangedDate;

                var expectedCsvData =
                    "Reference ID,Status,Date submitted (GMT),Obligation type,WEEE received start date,WEEE received end date,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E1,Draft,,Non-household,{note1.StartDate.ToShortDateString()},{note1.EndDate.ToShortDateString()},Site specific protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,196.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,210.000\r\n" +

                    $"E4,Approved,{note4SubmittedDate},Household,{note4.StartDate.ToShortDateString()},{note4.EndDate.ToShortDateString()},Reuse network PWP,10.244,9.322,10.266,9.099,10.288,7.077,9.122,2.033,9.311,9.144,9.166,10.299,5.055,1.011,111.437,10.255,10.233,10.277,9.011,10.299,8.088,9.133,4.044,9.322,9.155,10.177,9.311,6.066,2.022,117.393\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_{aatfApprovalNumber}_Evidence notes report{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        public class GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNoteReportRequest, CSVFileData> handler;
            protected static GetEvidenceNoteReportRequest request;
            protected static CSVFileData result;
            protected static List<Note> notes;
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
                handler = Container.Resolve<IRequestHandler<GetEvidenceNoteReportRequest, CSVFileData>>();

                notes = new List<Note>();

                return setup;
            }

            public static IntegrationTestSetupBuilder LocalOrganisationSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData(true)
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetEvidenceNoteReportRequest, CSVFileData>>();

                notes = new List<Note>();

                return setup;
            }
        }
    }
}
