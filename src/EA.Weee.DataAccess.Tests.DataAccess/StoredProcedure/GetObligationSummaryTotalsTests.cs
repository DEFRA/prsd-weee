namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class GetObligationSummaryTotalsTests
    {
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
    }
}
