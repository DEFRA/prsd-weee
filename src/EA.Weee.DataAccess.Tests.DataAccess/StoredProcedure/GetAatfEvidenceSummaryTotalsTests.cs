namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using AutoFixture;
    using Core.Helpers;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using EA.Weee.Core.Tests.Unit.Helpers;
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
        private readonly Fixture fixture;

        public GetAatfEvidenceSummaryTotalsTests()
        {
            fixture = new Fixture();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task Execute_GivenAatfWithDraftNote_NoDataShouldBeReturned(NoteStatus noteStatus)
        {
            if (noteStatus.Equals(NoteStatus.Approved))
            {
                return;
            }

            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(db, organisation1, null, aatf1);
                if (!noteStatus.Equals(NoteStatus.Draft))
                {
                    if (noteStatus.Equals(NoteStatus.Submitted))
                    {
                        note1.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser());
                    }
                    else
                    {
                        note1.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser());
                        note1.UpdateStatus(noteStatus, db.WeeeContext.GetCurrentUser());
                    }
                }

                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, (short)SystemTime.UtcNow.Year);

                totals.Count.Should().Be(14);
                ShouldHaveEmptyTotals(totals);
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

                await db.WeeeContext.SaveChangesAsync();

                var note1 = ApprovedNote(db, organisation1, aatf1);

                var tonnageCount = 0;
                foreach (var value in Enum.GetValues(typeof(WeeeCategory)))
                {
                    note1.NoteTonnage.Add(new NoteTonnage((WeeeCategory)value, tonnageCount++, tonnageCount++));
                }

                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, (short)SystemTime.UtcNow.Year);

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

                await db.WeeeContext.SaveChangesAsync();

                var note1 = ApprovedNote(db, organisation1, aatf1);

                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, (short)SystemTime.UtcNow.Year);

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

                await db.WeeeContext.SaveChangesAsync();

                var note1 = ApprovedNote(db, organisation1, aatf1);
                var note2 = ApprovedNote(db, organisation1, aatf2);
                var tonnageCount = 0;
                foreach (var value in Enum.GetValues(typeof(WeeeCategory)))
                {
                    note1.NoteTonnage.Add(new NoteTonnage((WeeeCategory)value, tonnageCount++, tonnageCount++));
                }
                
                note2.NoteTonnage.Clear();
                context.Notes.Add(note1);
                context.Notes.Add(note2);

                await db.WeeeContext.SaveChangesAsync();

                var totalsAatf1 = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, (short)SystemTime.UtcNow.Year);
                var totalsAatf2 = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf2.Id, (short)SystemTime.UtcNow.Year);

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

                await db.WeeeContext.SaveChangesAsync();

                var notes = new List<Note>();

                for (var i = 0; i < 500; i++)
                {
                    var note1 = ApprovedNote(db, organisation1, aatf1);

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
                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, (short)SystemTime.UtcNow.Year);
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
        public async Task Execute_GivenCreateNoteWithStartDateOutsideComplianceYear_NotNullNoteShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var context = db.WeeeContext;
                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                context.Organisations.Add(organisation1);
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);
                context.Aatfs.Add(aatf1);
                await db.WeeeContext.SaveChangesAsync();

                // Act
                Note note = NoteCommon.CreateNote(db, organisation1, null, aatf1, startDate: new DateTime(2020, 1, 1), complianceYear: 2022);

                // Assert
                note.Should().NotBeNull();
                note.ComplianceYear.Should().Be(2022);
            }
        }

        [Fact]
        public async Task Execute_GivenCreateNoteWithStartDateAndNullComplianceYear_NotNullNoteShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var context = db.WeeeContext;
                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                context.Organisations.Add(organisation1);
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);
                context.Aatfs.Add(aatf1);
                await db.WeeeContext.SaveChangesAsync();

                // Act
                Note note = NoteCommon.CreateNote(db, organisation1, null, aatf1, startDate: new DateTime(2020, 1, 1), complianceYear: null);

                // Assert
                note.Should().NotBeNull();
                note.ComplianceYear.Should().Be(2020);
            }
        }

        [Fact]
        public async Task Execute_GivenCreateNoteWithStartDateInsideComplianceYear_OneNoteShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var context = db.WeeeContext;
                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                context.Organisations.Add(organisation1);
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);
                context.Aatfs.Add(aatf1);
                await db.WeeeContext.SaveChangesAsync();

                // Act
                Note note = NoteCommon.CreateNote(db, organisation1, null, aatf1, startDate: new DateTime(2025, 1, 1), complianceYear: 2025);

                // Assert
                note.Should().NotBeNull();
                note.ComplianceYear.Should().Be(2025);
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

        private static Note ApprovedNote(DatabaseWrapper db, Organisation organisation1, Aatf aatf1)
        {
            var note1 = NoteCommon.CreateNote(db, organisation1, null, aatf1);
            note1.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser());
            note1.UpdateStatus(NoteStatus.Approved, db.WeeeContext.GetCurrentUser());
            return note1;
        }
    }
}
