namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using AutoFixture;
    using Core.Helpers;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class GetAatfEvidenceSummaryTotalsTests
    {
        [Fact]
        public async Task Execute_GivenAatfWithData_DataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var originatingOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                context.Schemes.Add(scheme);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, originatingOrganisation);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var draftNote1 = DraftNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, 2));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 3));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 3, 4));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 4, 5));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 5, 6));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 6, 7));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 7, 8));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 8, 9));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 9, 10));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 10, 11));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 11, 12));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 12, 13));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 13, 14));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 14, 15));

                context.Notes.Add(draftNote1);

                var draftNote2 = DraftNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, 2));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 3));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 3, 4));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 4, 5));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 5, 6));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 6, 7));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 7, 8));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 8, 9));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 9, 10));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 10, 11));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 11, 12));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 12, 13));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 13, 14));
                draftNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 14, 15));

                context.Notes.Add(draftNote2);

                var submittedNote1 = SubmittedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 10, 20));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 20, 30));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 30, 40));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 40, 50));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 50, 60));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 60, 70));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 70, 80));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 80, 90));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 90, 100));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 110));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 110, 120));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 120, 130));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 130, 140));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 140, 150));

                context.Notes.Add(submittedNote1);

                var submittedNote2 = SubmittedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 10, 20));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 20, 30));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 30, 40));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 40, 50));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 50, 60));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 60, 70));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 70, 80));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 80, 90));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 90, 100));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 110));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 110, 120));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 120, 130));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 130, 140));
                submittedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 140, 150));

                context.Notes.Add(submittedNote2);

                var approvedNote1 = ApprovedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 100, 200));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 200, 300));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 300, 400));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 400, 500));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 500, 600));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 600, 700));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 700, 800));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 800, 900));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 900, 1000));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1000, 1100));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1100, 1200));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 1200, 1300));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 1300, 1400));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 1400, 1500));

                context.Notes.Add(approvedNote1);
                
                var approvedNote2 = ApprovedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 100, 200));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 200, 300));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 300, 400));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 400, 500));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 500, 600));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 600, 700));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 700, 800));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 800, 900));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 900, 1000));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1000, 1100));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1100, 1200));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 1200, 1300));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 1300, 1400));
                approvedNote2.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 1400, 1500));

                context.Notes.Add(approvedNote2);

                // not included in any total
                var voidNote = VoidNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 10000, 10000));
                voidNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 10000, 10000));

                context.Notes.Add(voidNote);

                // not included in any total
                var rejectedNote = RejectedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 10000, 10000));
                rejectedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 10000, 10000));

                context.Notes.Add(rejectedNote);

                // not included in any total
                var returnedNote = ReturnedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 10000, 10000));
                returnedNote.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 10000, 10000));

                context.Notes.Add(returnedNote);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, SystemTime.UtcNow.Year);

                totals.Count.Should().Be(15);
                totals.ElementAt(0).CategoryName.Should().Be($"{WeeeCategory.LargeHouseholdAppliances.ToInt()}. {WeeeCategory.LargeHouseholdAppliances.ToDisplayString()}");
                totals.ElementAt(0).ApprovedReceived.Should().Be(200); 
                totals.ElementAt(0).ApprovedReused.Should().Be(400);
                totals.ElementAt(0).DraftReceived.Should().Be(2);
                totals.ElementAt(0).DraftReused.Should().Be(4);
                totals.ElementAt(0).SubmittedReceived.Should().Be(20);
                totals.ElementAt(0).SubmittedReused.Should().Be(40);
                totals.ElementAt(1).CategoryName.Should().Be($"{WeeeCategory.SmallHouseholdAppliances.ToInt()}. {WeeeCategory.SmallHouseholdAppliances.ToDisplayString()}"); 
                totals.ElementAt(1).ApprovedReceived.Should().Be(1800); 
                totals.ElementAt(1).ApprovedReused.Should().Be(2000);
                totals.ElementAt(1).DraftReceived.Should().Be(18);
                totals.ElementAt(1).DraftReused.Should().Be(20);
                totals.ElementAt(1).SubmittedReceived.Should().Be(180);
                totals.ElementAt(1).SubmittedReused.Should().Be(200);
                totals.ElementAt(2).CategoryName.Should().Be($"{WeeeCategory.ITAndTelecommsEquipment.ToInt()}. {WeeeCategory.ITAndTelecommsEquipment.ToDisplayString()}");
                totals.ElementAt(2).ApprovedReceived.Should().Be(1400);
                totals.ElementAt(2).ApprovedReused.Should().Be(1600);
                totals.ElementAt(2).DraftReceived.Should().Be(14);
                totals.ElementAt(2).DraftReused.Should().Be(16);
                totals.ElementAt(2).SubmittedReceived.Should().Be(140);
                totals.ElementAt(2).SubmittedReused.Should().Be(160);
                totals.ElementAt(3).CategoryName.Should().Be($"{WeeeCategory.ConsumerEquipment.ToInt()}. {WeeeCategory.ConsumerEquipment.ToDisplayString()}");
                totals.ElementAt(3).ApprovedReceived.Should().Be(800);
                totals.ElementAt(3).ApprovedReused.Should().Be(1000);
                totals.ElementAt(3).DraftReceived.Should().Be(8);
                totals.ElementAt(3).DraftReused.Should().Be(10);
                totals.ElementAt(3).SubmittedReceived.Should().Be(80);
                totals.ElementAt(3).SubmittedReused.Should().Be(100);
                totals.ElementAt(4).CategoryName.Should().Be($"{WeeeCategory.LightingEquipment.ToInt()}. {WeeeCategory.LightingEquipment.ToDisplayString()}");
                totals.ElementAt(4).ApprovedReceived.Should().Be(2600); 
                totals.ElementAt(4).ApprovedReused.Should().Be(2800);
                totals.ElementAt(4).DraftReceived.Should().Be(26);
                totals.ElementAt(4).DraftReused.Should().Be(28);
                totals.ElementAt(4).SubmittedReceived.Should().Be(260);
                totals.ElementAt(4).SubmittedReused.Should().Be(280);
                totals.ElementAt(5).CategoryName.Should().Be($"{WeeeCategory.ElectricalAndElectronicTools.ToInt()}. {WeeeCategory.ElectricalAndElectronicTools.ToDisplayString()}");
                totals.ElementAt(5).ApprovedReceived.Should().Be(2200);
                totals.ElementAt(5).ApprovedReused.Should().Be(2400);
                totals.ElementAt(5).DraftReceived.Should().Be(22);
                totals.ElementAt(5).DraftReused.Should().Be(24);
                totals.ElementAt(5).SubmittedReceived.Should().Be(220);
                totals.ElementAt(5).SubmittedReused.Should().Be(240);
                totals.ElementAt(6).CategoryName.Should().Be($"{WeeeCategory.ToysLeisureAndSports.ToInt()}. {WeeeCategory.ToysLeisureAndSports.ToDisplayString()}");
                totals.ElementAt(6).ApprovedReceived.Should().Be(1600); 
                totals.ElementAt(6).ApprovedReused.Should().Be(1800);
                totals.ElementAt(6).DraftReceived.Should().Be(16);
                totals.ElementAt(6).DraftReused.Should().Be(18);
                totals.ElementAt(6).SubmittedReceived.Should().Be(160);
                totals.ElementAt(6).SubmittedReused.Should().Be(180);
                totals.ElementAt(7).CategoryName.Should().Be($"{WeeeCategory.MedicalDevices.ToInt()}. {WeeeCategory.MedicalDevices.ToDisplayString()}");
                totals.ElementAt(7).ApprovedReceived.Should().Be(600); 
                totals.ElementAt(7).ApprovedReused.Should().Be(800);
                totals.ElementAt(7).DraftReceived.Should().Be(6);
                totals.ElementAt(7).DraftReused.Should().Be(8);
                totals.ElementAt(7).SubmittedReceived.Should().Be(60);
                totals.ElementAt(7).SubmittedReused.Should().Be(80);
                totals.ElementAt(8).CategoryName.Should().Be($"{WeeeCategory.MonitoringAndControlInstruments.ToInt()}. {WeeeCategory.MonitoringAndControlInstruments.ToDisplayString()}");
                totals.ElementAt(8).ApprovedReceived.Should().Be(2800); 
                totals.ElementAt(8).ApprovedReused.Should().Be(3000);
                totals.ElementAt(8).DraftReceived.Should().Be(28);
                totals.ElementAt(8).DraftReused.Should().Be(30);
                totals.ElementAt(8).SubmittedReceived.Should().Be(280);
                totals.ElementAt(8).SubmittedReused.Should().Be(300);
                totals.ElementAt(9).CategoryName.Should().Be($"{WeeeCategory.AutomaticDispensers.ToInt()}. {WeeeCategory.AutomaticDispensers.ToDisplayString()}");
                totals.ElementAt(9).ApprovedReceived.Should().Be(400); 
                totals.ElementAt(9).ApprovedReused.Should().Be(600);
                totals.ElementAt(9).DraftReceived.Should().Be(4);
                totals.ElementAt(9).DraftReused.Should().Be(6);
                totals.ElementAt(9).SubmittedReceived.Should().Be(40);
                totals.ElementAt(9).SubmittedReused.Should().Be(60);
                totals.ElementAt(10).CategoryName.Should().Be($"{WeeeCategory.DisplayEquipment.ToInt()}. {WeeeCategory.DisplayEquipment.ToDisplayString()}");
                totals.ElementAt(10).ApprovedReceived.Should().Be(1200); 
                totals.ElementAt(10).ApprovedReused.Should().Be(1400);
                totals.ElementAt(10).DraftReceived.Should().Be(12);
                totals.ElementAt(10).DraftReused.Should().Be(14);
                totals.ElementAt(10).SubmittedReceived.Should().Be(120);
                totals.ElementAt(10).SubmittedReused.Should().Be(140);
                totals.ElementAt(11).CategoryName.Should().Be($"{WeeeCategory.CoolingApplicancesContainingRefrigerants.ToInt()}. {WeeeCategory.CoolingApplicancesContainingRefrigerants.ToDisplayString()}");
                totals.ElementAt(11).ApprovedReceived.Should().Be(1000);
                totals.ElementAt(11).ApprovedReused.Should().Be(1200);
                totals.ElementAt(11).DraftReceived.Should().Be(10);
                totals.ElementAt(11).DraftReused.Should().Be(12);
                totals.ElementAt(11).SubmittedReceived.Should().Be(100);
                totals.ElementAt(11).SubmittedReused.Should().Be(120);
                totals.ElementAt(12).CategoryName.Should().Be($"{WeeeCategory.GasDischargeLampsAndLedLightSources.ToInt()}. {WeeeCategory.GasDischargeLampsAndLedLightSources.ToDisplayString()}");
                totals.ElementAt(12).ApprovedReceived.Should().Be(2400); 
                totals.ElementAt(12).ApprovedReused.Should().Be(2600);
                totals.ElementAt(12).DraftReceived.Should().Be(24);
                totals.ElementAt(12).DraftReused.Should().Be(26);
                totals.ElementAt(12).SubmittedReceived.Should().Be(240);
                totals.ElementAt(12).SubmittedReused.Should().Be(260);
                totals.ElementAt(13).CategoryName.Should().Be($"{WeeeCategory.PhotovoltaicPanels.ToInt()}. {WeeeCategory.PhotovoltaicPanels.ToDisplayString()}");
                totals.ElementAt(13).ApprovedReceived.Should().Be(2000);
                totals.ElementAt(13).ApprovedReused.Should().Be(2200);
                totals.ElementAt(13).DraftReceived.Should().Be(20);
                totals.ElementAt(13).DraftReused.Should().Be(22);
                totals.ElementAt(13).SubmittedReceived.Should().Be(200);
                totals.ElementAt(13).SubmittedReused.Should().Be(220);
                totals.ElementAt(14).CategoryName.Should().Be("Total (tonnes)");
                totals.ElementAt(14).ApprovedReceived.Should().Be(21000); 
                totals.ElementAt(14).ApprovedReused.Should().Be(23800);
                totals.ElementAt(14).DraftReceived.Should().Be(210);
                totals.ElementAt(14).DraftReused.Should().Be(238);
                totals.ElementAt(14).SubmittedReceived.Should().Be(2100);
                totals.ElementAt(14).SubmittedReused.Should().Be(2380);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfNoTonnages_EmptyDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);

                context.Aatfs.Add(aatf1);

                var recipientOrganisation = Organisation.CreateRegisteredCompany("Test Organisation", "1234565");
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                context.Schemes.Add(scheme);

                await db.WeeeContext.SaveChangesAsync();

                var note1 = ApprovedNote(db, organisation1, recipientOrganisation, aatf1, 1957);

                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1957);

                totals.Count.Should().Be(15);
                ShouldHaveEmptyTotals(totals);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithDataAndDifferentAatfRequested_EmptyDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var originatingOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                context.Schemes.Add(scheme);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, originatingOrganisation);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var draftNote1 = DraftNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, 2));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 3));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 3, 4));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 4, 5));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 5, 6));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 6, 7));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 7, 8));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 8, 9));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 9, 10));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 10, 11));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 11, 12));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 12, 13));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 13, 14));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 14, 15));

                context.Notes.Add(draftNote1);

                var submittedNote1 = SubmittedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 10, 20));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 20, 30));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 30, 40));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 40, 50));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 50, 60));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 60, 70));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 70, 80));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 80, 90));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 90, 100));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 110));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 110, 120));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 120, 130));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 130, 140));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 140, 150));

                context.Notes.Add(submittedNote1);

                var approvedNote1 = ApprovedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 100, 200));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 200, 300));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 300, 400));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 400, 500));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 500, 600));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 600, 700));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 700, 800));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 800, 900));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 900, 1000));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1000, 1100));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1100, 1200));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 1200, 1300));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 1300, 1400));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 1400, 1500));

                context.Notes.Add(approvedNote1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(Guid.NewGuid(), SystemTime.UtcNow.Year);

                totals.Count.Should().Be(15);
                ShouldHaveEmptyTotals(totals);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithDataAndDifferentComplianceYear_EmptyDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var originatingOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var recipientOrganisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                context.Schemes.Add(scheme);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, originatingOrganisation);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var draftNote1 = DraftNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, 2));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 3));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 3, 4));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 4, 5));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 5, 6));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 6, 7));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 7, 8));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 8, 9));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 9, 10));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 10, 11));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 11, 12));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 12, 13));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 13, 14));
                draftNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 14, 15));

                context.Notes.Add(draftNote1);

                var submittedNote1 = SubmittedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 10, 20));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 20, 30));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 30, 40));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 40, 50));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 50, 60));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 60, 70));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 70, 80));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 80, 90));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 90, 100));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 110));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 110, 120));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 120, 130));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 130, 140));
                submittedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 140, 150));

                context.Notes.Add(submittedNote1);

                var approvedNote1 = ApprovedNote(db, originatingOrganisation, recipientOrganisation, aatf1, SystemTime.UtcNow.Year);
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 100, 200));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.AutomaticDispensers, 200, 300));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MedicalDevices, 300, 400));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ConsumerEquipment, 400, 500));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 500, 600));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.DisplayEquipment, 600, 700));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 700, 800));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 800, 900));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 900, 1000));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 1000, 1100));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 1100, 1200));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 1200, 1300));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.LightingEquipment, 1300, 1400));
                approvedNote1.NoteTonnage.Add(new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 1400, 1500));

                context.Notes.Add(approvedNote1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, SystemTime.UtcNow.Year - 1);

                totals.Count.Should().Be(15);
                ShouldHaveEmptyTotals(totals);
            }
        }

        private static void ShouldHaveEmptyTotals(List<AatfEvidenceSummaryTotalsData> totalsAatf1)
        {
            totalsAatf1.ElementAt(0).ApprovedReceived.Should().BeNull(); //LargeHouseholdAppliances
            totalsAatf1.ElementAt(0).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(0).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(0).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(0).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(0).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(1).ApprovedReceived.Should().BeNull(); //SmallHouseholdAppliances
            totalsAatf1.ElementAt(1).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(1).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(1).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(1).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(1).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(2).ApprovedReceived.Should().BeNull(); //ITAndTelecommsEquipment
            totalsAatf1.ElementAt(2).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(2).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(2).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(2).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(2).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(3).ApprovedReceived.Should().BeNull(); //ConsumerEquipment
            totalsAatf1.ElementAt(3).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(3).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(3).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(3).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(3).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(4).ApprovedReceived.Should().BeNull(); //LightingEquipment
            totalsAatf1.ElementAt(4).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(4).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(4).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(4).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(4).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(5).ApprovedReceived.Should().BeNull(); //ElectricalAndElectronicTools
            totalsAatf1.ElementAt(5).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(5).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(5).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(5).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(5).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(6).ApprovedReceived.Should().BeNull(); //ToysLeisureAndSports
            totalsAatf1.ElementAt(6).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(6).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(6).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(6).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(6).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(7).ApprovedReceived.Should().BeNull(); //MedicalDevices
            totalsAatf1.ElementAt(7).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(7).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(7).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(7).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(7).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(8).ApprovedReceived.Should().BeNull(); //MonitoringAndControlInstruments
            totalsAatf1.ElementAt(8).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(8).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(8).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(8).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(8).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(9).ApprovedReceived.Should().BeNull(); //AutomaticDispensers
            totalsAatf1.ElementAt(9).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(9).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(9).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(9).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(9).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(10).ApprovedReceived.Should().BeNull(); //DisplayEquipment
            totalsAatf1.ElementAt(10).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(10).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(10).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(10).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(10).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(11).ApprovedReceived.Should().BeNull(); //CoolingApplicancesContainingRefrigerants
            totalsAatf1.ElementAt(11).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(11).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(11).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(11).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(11).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(12).ApprovedReceived.Should().BeNull(); //GasDischargeLampsAndLedLightSources
            totalsAatf1.ElementAt(12).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(12).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(12).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(12).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(12).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(13).ApprovedReceived.Should().BeNull(); //PhotovoltaicPanels
            totalsAatf1.ElementAt(13).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(13).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(13).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(13).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(13).SubmittedReused.Should().BeNull();
            totalsAatf1.ElementAt(14).ApprovedReceived.Should().BeNull(); //totals
            totalsAatf1.ElementAt(14).ApprovedReused.Should().BeNull();
            totalsAatf1.ElementAt(14).DraftReceived.Should().BeNull();
            totalsAatf1.ElementAt(14).DraftReused.Should().BeNull();
            totalsAatf1.ElementAt(14).SubmittedReceived.Should().BeNull();
            totalsAatf1.ElementAt(14).SubmittedReused.Should().BeNull();
        }

        private static Note ApprovedNote(DatabaseWrapper db, Organisation organisation1, Organisation recipientOrganisation, Aatf aatf1, int complianceYear)
        {
            var note = Note(db, organisation1, recipientOrganisation, aatf1, complianceYear);

            note.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            note.UpdateStatus(NoteStatus.Approved, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            return note;
        }

        private static Note Note(DatabaseWrapper db, Organisation organisation1, Organisation recipientOrganisation, Aatf aatf1,
            int complianceYear)
        {
            var note = new Note(organisation1,
                recipientOrganisation,
                DateTime.UtcNow,
                DateTime.UtcNow,
                WasteType.HouseHold,
                Protocol.Actual,
                aatf1,
                db.WeeeContext.GetCurrentUser().ToString(),
                new List<NoteTonnage>())
            {
                ComplianceYear = complianceYear
            };
            return note;
        }

        private static Note SubmittedNote(DatabaseWrapper db, Organisation organisation1, Organisation recipientOrganisation, Aatf aatf1, int complianceYear)
        {
            var note = Note(db, organisation1, recipientOrganisation, aatf1, complianceYear);

            note.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            return note;
        }

        private static Note VoidNote(DatabaseWrapper db, Organisation organisation1, Organisation recipientOrganisation, Aatf aatf1, int complianceYear)
        {
            var note = Note(db, organisation1, recipientOrganisation, aatf1, complianceYear);

            note.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            note.UpdateStatus(NoteStatus.Approved, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            note.UpdateStatus(NoteStatus.Void, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            return note;
        }

        private static Note RejectedNote(DatabaseWrapper db, Organisation organisation1, Organisation recipientOrganisation, Aatf aatf1, int complianceYear)
        {
            var note = Note(db, organisation1, recipientOrganisation, aatf1, complianceYear);

            note.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            note.UpdateStatus(NoteStatus.Approved, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            note.UpdateStatus(NoteStatus.Rejected, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            return note;
        }

        private static Note ReturnedNote(DatabaseWrapper db, Organisation organisation1, Organisation recipientOrganisation, Aatf aatf1, int complianceYear)
        {
            var note = Note(db, organisation1, recipientOrganisation, aatf1, complianceYear);

            note.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            note.UpdateStatus(NoteStatus.Approved, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            note.UpdateStatus(NoteStatus.Returned, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            return note;
        }

        private static Note DraftNote(DatabaseWrapper db, Organisation organisation1, Organisation recipientOrganisation, Aatf aatf1, int complianceYear)
        {
            var note = Note(db, organisation1, recipientOrganisation, aatf1, complianceYear);

            return note;
        }
    }
}
