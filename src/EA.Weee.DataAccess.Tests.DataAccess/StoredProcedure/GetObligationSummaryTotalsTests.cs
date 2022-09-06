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
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class GetObligationSummaryTotalsTests
    {
        private readonly Fixture fixture;

        public GetObligationSummaryTotalsTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Execute_GivenBothNullSchemeIdAndNullOrganisationId_ErrorShouldBeRaised()
        {
            // Act
            using (var db = new DatabaseWrapper())
            {
                try
                {
                    await db.EvidenceStoredProcedures.GetObligationEvidenceSummaryTotals(null, null, 2022);
                }
                catch (ArgumentException e)
                {
                    e.Message.Should().Contain("pcsId and orgId cannot be both null");
                }
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovedNoteAndTonnageData_DataShouldBeReturned()
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

                var note1 = ApprovedNote(db, organisation1, recipientOrganisation, aatf1, 1000);

                var tonnageCount = 0;
                foreach (var value in Enum.GetValues(typeof(WeeeCategory)))
                {
                    note1.NoteTonnage.Add(new NoteTonnage((WeeeCategory)value, tonnageCount++, tonnageCount++));
                }

                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1000);

                totals.Count.Should().Be(14);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Received.Should().Be(0);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.LargeHouseholdAppliances)).Reused.Should().Be(1);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Received.Should().Be(2);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.SmallHouseholdAppliances)).Reused.Should().Be(3);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Received.Should().Be(4);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.ITAndTelecommsEquipment)).Reused.Should().Be(5);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Received.Should().Be(6);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.ConsumerEquipment)).Reused.Should().Be(7);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.LightingEquipment)).Received.Should().Be(8);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.LightingEquipment)).Reused.Should().Be(9);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Received.Should().Be(10);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools)).Reused.Should().Be(11);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Received.Should().Be(12);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.ToysLeisureAndSports)).Reused.Should().Be(13);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.MedicalDevices)).Received.Should().Be(14);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.MedicalDevices)).Reused.Should().Be(15);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Received.Should().Be(16);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.MonitoringAndControlInstruments)).Reused.Should().Be(17);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Received.Should().Be(18);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.AutomaticDispensers)).Reused.Should().Be(19);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Received.Should().Be(20);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.DisplayEquipment)).Reused.Should().Be(21);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Received.Should().Be(22);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.CoolingApplicancesContainingRefrigerants)).Reused.Should().Be(23);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Received.Should().Be(24);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.GasDischargeLampsAndLedLightSources)).Reused.Should().Be(25);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Received.Should().Be(26);
                totals.First(c => c.CategoryId.Equals(WeeeCategory.PhotovoltaicPanels)).Reused.Should().Be(27);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovedNoteWithNoTonnages_EmptyDataShouldBeReturned()
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

                totals.Count.Should().Be(14);
                ShouldHaveEmptyTotals(totals);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithApprovedNoteAndDifferentAatfRequested_EmptyDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);

                context.Aatfs.Add(aatf1);
                context.Aatfs.Add(aatf2);

                var recipientOrganisation = Organisation.CreateRegisteredCompany("Test Organisation", "1234565");
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                context.Schemes.Add(scheme);

                await db.WeeeContext.SaveChangesAsync();

                var note1 = ApprovedNote(db, organisation1, recipientOrganisation, aatf1, 1234);
                var note2 = ApprovedNote(db, organisation1, recipientOrganisation, aatf2, 1234);
                var tonnageCount = 0;
                foreach (var value in Enum.GetValues(typeof(WeeeCategory)))
                {
                    note1.NoteTonnage.Add(new NoteTonnage((WeeeCategory)value, tonnageCount++, tonnageCount++));
                }
                
                note2.NoteTonnage.Clear();
                context.Notes.Add(note1);
                context.Notes.Add(note2);

                await db.WeeeContext.SaveChangesAsync();

                var totalsAatf1 = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1234);
                var totalsAatf2 = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf2.Id, 1234);

                totalsAatf2.Count.Should().Be(14);
                ShouldHaveEmptyTotals(totalsAatf2);
                totalsAatf1.Count.Should().Be(14);
                foreach (var aatfEvidenceSummaryTotalsData in totalsAatf1)
                {
                    aatfEvidenceSummaryTotalsData.Received.Should().NotBeNull();
                    aatfEvidenceSummaryTotalsData.Reused.Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithManyApprovedNotes_DataShouldBeReturned()
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

                var notes = new List<Note>();

                for (var i = 0; i < 500; i++)
                {
                    var note1 = ApprovedNote(db, organisation1, recipientOrganisation, aatf1, 1500);

                    foreach (var value in Enum.GetValues(typeof(WeeeCategory)))
                    {
                        var convertedValue = (int)value + i;
                        note1.NoteTonnage.Add(new NoteTonnage((WeeeCategory)value, convertedValue, convertedValue + 1));
                    }

                    notes.Add(note1);
                    context.Notes.Add(note1);
                }

                await db.WeeeContext.SaveChangesAsync();

                var watch = new Stopwatch();
                watch.Start();
                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1500);
                watch.Stop();

                watch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
                foreach (var value in Enum.GetValues(typeof(WeeeCategory)))
                {
                    var categoryValue = (int)value;

                    var values = totals.First(t => t.CategoryId.ToInt().Equals(categoryValue));
                    var notesTonnage = notes.SelectMany(n => n.NoteTonnage.Where(nt => nt.CategoryId.ToInt().Equals(categoryValue)));
                    
                    values.Received.Should().Be(notesTonnage.Sum(nt => nt.Received));
                    values.Reused.Should().Be(notesTonnage.Sum(nt => nt.Reused));
                }

                totals.Count.Should().Be(14);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithManyApprovedNotesInSameComplianceYear_TonnageShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                context.Organisations.Add(organisation1);

                context.Organisations.Add(organisation1);

                var recipientOrganisation = Organisation.CreateRegisteredCompany("Test Organisation", "1234565");
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(recipientOrganisation);

                context.Schemes.Add(scheme);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var notes = new List<Note>();

                for (var i = 0; i < 5; i++)
                {
                    var note = ApprovedNote(db, organisation1, recipientOrganisation, aatf1, 2000);
                    foreach (var value in Enum.GetValues(typeof(WeeeCategory)))
                    {
                        note.NoteTonnage.Add(new NoteTonnage((WeeeCategory)value, 2, 1));
                    }
                    notes.Add(note);
                    context.Notes.Add(note);
                }

                var note5 = ApprovedNote(db, organisation1, recipientOrganisation, aatf1, 2010);
                foreach (var value in Enum.GetValues(typeof(WeeeCategory)))
                {
                    note5.NoteTonnage.Add(new NoteTonnage((WeeeCategory)value, 20, 10));
                }
                notes.Add(note5);
                context.Notes.Add(note5);

                await db.WeeeContext.SaveChangesAsync();

                var totals1 = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 2000);
                decimal? totalReceived1 = 0.0m;
                decimal? totalReused1 = 0.0m;
                foreach (var totalsData in totals1)
                {
                    totalReceived1 += totalsData.Received;
                    totalReused1 += totalsData.Reused;
                }

                totals1.Count.Should().Be(14);
                totalReceived1.Should().Be(140.00m);
                totalReused1.Should().Be(70.00m);

                var totals2 = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 2010);
                decimal? totalReceived2 = 0.0m;
                decimal? totalReused2 = 0.0m;
                foreach (var totalsData in totals2)
                {
                    totalReceived2 += totalsData.Received;
                    totalReused2 += totalsData.Reused;
                }
                totals2.Count.Should().Be(14);
                totalReceived2.Should().Be(280.00m);
                totalReused2.Should().Be(140.00m);
            }
        }

        private static void ShouldHaveEmptyTotals(List<AatfEvidenceSummaryTotalsData> totalsAatf1)
        {
            foreach (var aatfEvidenceSummaryTotalsData in totalsAatf1)
            {
                aatfEvidenceSummaryTotalsData.Received.Should().BeNull();
                aatfEvidenceSummaryTotalsData.Reused.Should().BeNull();
            }
        }

        private static Note ApprovedNote(DatabaseWrapper db, Organisation organisation1, Organisation recipientOrganisation, Aatf aatf1, int complianceYear)
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

            note.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            note.UpdateStatus(NoteStatus.Approved, db.WeeeContext.GetCurrentUser(), SystemTime.UtcNow);
            return note;
        }
    }
}
