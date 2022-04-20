namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class GetAatfEvidenceSummaryTotalsTests
    {
        private readonly Fixture fixture;

        public GetAatfEvidenceSummaryTotalsTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task Execute_GivenAatfWithDraftNote_NoDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(db, organisation1, null, aatf1);
                
                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1);

                totals.Count.Should().Be(0);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithSubmitted_NoDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(db, organisation1, null, aatf1);
                note1.UpdateStatus(NoteStatus.Submitted, db.WeeeContext.GetCurrentUser());

                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1);

                totals.Count.Should().Be(0);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithRejectedNote_NoDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(db, organisation1, null, aatf1);
                note1.UpdateStatus(NoteStatus.Rejected, db.WeeeContext.GetCurrentUser());

                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1);

                totals.Count.Should().Be(0);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithVoidNote_NoDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var context = db.WeeeContext;

                var organisation1 = ObligatedWeeeIntegrationCommon.CreateOrganisation();

                context.Organisations.Add(organisation1);

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation1);

                context.Aatfs.Add(aatf1);

                await db.WeeeContext.SaveChangesAsync();

                var note1 = NoteCommon.CreateNote(db, organisation1, null, aatf1);
                note1.UpdateStatus(NoteStatus.Void, db.WeeeContext.GetCurrentUser());

                context.Notes.Add(note1);

                await db.WeeeContext.SaveChangesAsync();

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1);

                totals.Count.Should().Be(0);
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

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1);

                totals.Count.Should().Be(14);
                foreach (var aatfEvidenceSummaryTotalsData in totals)
                {
                    aatfEvidenceSummaryTotalsData.Received.Should().BeNull();
                    aatfEvidenceSummaryTotalsData.Reused.Should().BeNull();
                }
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

                var totals = await db.EvidenceStoredProcedures.GetAatfEvidenceSummaryTotals(aatf1.Id, 1);

                totals.Count.Should().Be(14);
                foreach (var aatfEvidenceSummaryTotalsData in totals)
                {
                    aatfEvidenceSummaryTotalsData.Received.Should().BeNull();
                    aatfEvidenceSummaryTotalsData.Reused.Should().BeNull();
                }
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
