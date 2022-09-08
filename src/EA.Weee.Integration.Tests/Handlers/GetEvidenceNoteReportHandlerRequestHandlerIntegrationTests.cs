namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
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
    using Requests.AatfEvidence;
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

                var outOfComplianceYearNote = EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                notes.Add(outOfComplianceYearNote);

                request = new GetEvidenceNoteReportRequest(null, null, TonnageToDisplayReportEnum.OriginalTonnages, SystemTime.Now.Year);
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
                    
                    $"E1,Draft,EA,,{note1.Aatf.Name},{note1.Aatf.ApprovalNumber},Non-household,{note1.StartDate},{note1.EndDate},{note1.Recipient.Scheme.SchemeName},{note1.Recipient.Scheme.ApprovalNumber},Site specific protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,172.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,186.000\r\n" +

                    $"E2,Submitted,EA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate},{note2.EndDate},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Actual,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +

                    $"E3,Approved,SEPA,{note3SubmittedDate},{note3.Aatf.Name},{note3.Aatf.ApprovalNumber},Household,{note3.StartDate},{note3.EndDate},{note3.Recipient.Name},,Reuse network PWP,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,182.659,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,199.215\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_Evidence notes original tonnages{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
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

                var aDifferentOrganisationNote = EvidenceNoteDbSetup.Init()
                    .WithOrganisation(originatorOrganisation3.Id)
                    .WithComplianceYear(SystemTime.Now.Year)
                    
                    .Create();

                notes.Add(aDifferentOrganisationNote);

                var outOfComplianceYearNote = EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                notes.Add(outOfComplianceYearNote);

                request = new GetEvidenceNoteReportRequest(null, originatorOrganisation1.Id, TonnageToDisplayReportEnum.OriginalTonnages, SystemTime.Now.Year);
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

                var expectedCsvData =
                    "Reference ID,Status,AATF appropriate authority,Date submitted (GMT),Submitted by AATF site name,AATF approval number,Obligation type,WEEE received start date,WEEE received end date,Recipient name,Recipient approval number,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E1,Draft,EA,,{note1.Aatf.Name},{note1.Aatf.ApprovalNumber},Non-household,{note1.StartDate},{note1.EndDate},{note1.Recipient.Scheme.SchemeName},{note1.Recipient.Scheme.ApprovalNumber},LDA protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,172.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,186.000\r\n" +

                    $"E2,Submitted,EA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate},{note2.EndDate},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Actual,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,182.659,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,199.215\r\n";

                    result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_Evidence notes original tonnages{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetTheEvidenceNoteReportWithRecipientOrganisationAndOriginalTonnages : GetEvidenceNoteReportHandlerRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalOrganisationSetup();

                //draft note
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

                //note associated with a different organisation
                var recipientOrganisation3 = OrganisationDbSetup.Init().Create();
                SchemeDbSetup.Init().WithOrganisation(recipientOrganisation3.Id).Create();

                var aDifferentOrganisationNote = EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientOrganisation3.Id)
                    .WithComplianceYear(SystemTime.Now.Year)

                    .Create();

                notes.Add(aDifferentOrganisationNote);

                var outOfComplianceYearNote = EvidenceNoteDbSetup.Init().WithComplianceYear(SystemTime.Now.Year + 1).Create();

                notes.Add(outOfComplianceYearNote);

                request = new GetEvidenceNoteReportRequest(recipientOrganisation1.Id, null, TonnageToDisplayReportEnum.OriginalTonnages, SystemTime.Now.Year);
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

                var expectedCsvData =
                    "Reference ID,Status,AATF appropriate authority,Date submitted (GMT),Submitted by AATF site name,AATF approval number,Obligation type,WEEE received start date,WEEE received end date,Recipient name,Recipient approval number,Actual or protocol,Cat 1 received (t),Cat 2 received (t),Cat 3 received (t),Cat 4 received (t),Cat 5 received (t),Cat 6 received (t),Cat 7 received (t),Cat 8 received (t),Cat 9 received (t),Cat 10 received (t),Cat 11 received (t),Cat 12 received (t),Cat 13 received (t),Cat 14 received (t),Total received (t),Cat 1 reuse (t),Cat 2 reuse (t),Cat 3 reuse (t),Cat 4 reuse (t),Cat 5 reuse (t),Cat 6 reuse (t),Cat 7 reuse (t),Cat 8 reuse (t),Cat 9 reuse (t),Cat 10 reuse (t),Cat 11 reuse (t),Cat 12 reuse (t),Cat 13 reuse (t),Cat 14 reuse (t),Total reused (t)\r\n" +

                    $"E1,Draft,EA,,{note1.Aatf.Name},{note1.Aatf.ApprovalNumber},Non-household,{note1.StartDate},{note1.EndDate},{note1.Recipient.Scheme.SchemeName},{note1.Recipient.Scheme.ApprovalNumber},LDA protocol,21.000,19.000,23.000,9.000,25.000,7.000,11.000,3.000,27.000,13.000,15.000,17.000,5.000,1.000,172.000,22.000,20.000,24.000,10.000,26.000,8.000,12.000,4.000,28.000,14.000,16.000,18.000,6.000,2.000,186.000\r\n" +

                    $"E2,Submitted,EA,{note2SubmittedDate},{note2.Aatf.Name},{note2.Aatf.ApprovalNumber},Household,{note2.StartDate},{note2.EndDate},{note2.Recipient.Scheme.SchemeName},{note2.Recipient.Scheme.ApprovalNumber},Actual,22.444,20.222,24.666,9.999,26.888,7.777,11.222,2.333,28.111,13.444,15.666,18.999,5.555,1.111,182.659,23.555,21.333,25.777,10.111,27.999,8.888,12.333,4.444,29.222,14.555,17.777,19.111,6.666,2.222,199.215\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{SystemTime.Now.Year}_Evidence notes original tonnages{SystemTime.Now.ToString(DateTimeConstants.EvidenceReportFilenameTimestampFormat)}");
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
