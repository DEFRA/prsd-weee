namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.DataReturns;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Prsd.Core;
    using Xunit;

    public class EvidenceTonnageDataTests
    {
        private readonly Fixture fixture;

        public EvidenceTonnageDataTests()
        {
            fixture = new Fixture();
        }

        [Theory]
        [InlineData(5, null, 5)]
        [InlineData(5, 0, 5)]
        [InlineData(5, 5, 0)]
        [InlineData(5, 4, 1)]
        [InlineData(5, 6, 0)]
        public void EvidenceNoteData_EvidenceTonnageDataAvailableReceivedGet_ShouldCalculate_CorrectValues(decimal total, decimal transferred, decimal expectedAvailable)
        {
            //Arrange
            var noteData = new EvidenceTonnageData(fixture.Create<Guid>(), fixture.Create<WeeeCategory>(), total, null, transferred, null);

            //Assert
            noteData.AvailableReceived.Should().Be(expectedAvailable);
        }

        public static IEnumerable<object[]> Values()
        {
            yield return new object[] { 5m, null, 5m };
            yield return new object[] { 5m, 0m, 5m };
            yield return new object[] { 5m, 5m, 0m };
            yield return new object[] { 5m, 4m, 1m };
            yield return new object[] { 5m, 6m, 0m };
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void EvidenceNoteDataAvailableReusedGet_ShouldCalculate_CorrectValues(decimal total, decimal? transferred, decimal expectedReused)
        {
            //Arrange
            var noteData = new EvidenceTonnageData(fixture.Create<Guid>(), fixture.Create<WeeeCategory>(), null, total, null, transferred);

            //Assert
            noteData.AvailableReused.Should().Be(expectedReused);
        }
    }
}
