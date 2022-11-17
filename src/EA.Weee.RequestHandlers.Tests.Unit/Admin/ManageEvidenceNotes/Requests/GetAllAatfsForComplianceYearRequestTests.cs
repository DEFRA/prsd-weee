namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes.Requests
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FluentAssertions;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class GetAllAatfsForComplianceYearRequestTests : SimpleUnitTestBase
    {
        [Fact]
        public void GetAllAatfsForComplianceYearRequest_ShouldHaveSerializableAttribute()
        {
            typeof(GetAllAatfsForComplianceYearRequest).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void GetAllAatfsForComplianceYearRequest_ShouldDeriveFrom_EvidenceEntityIdDisplayNameDataBase()
        {
            typeof(GetAllAatfsForComplianceYearRequest).Should().BeDerivedFrom<EvidenceEntityIdDisplayNameDataBase>();
        }

        [Fact]
        public void GetAllAatfsForComplianceYearRequest_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();

            //act
            var result = new GetAllAatfsForComplianceYearRequest(complianceYear);

            //assert
            result.ComplianceYear.Should().Be(complianceYear);
            result.AllowedStatuses.Should().BeEquivalentTo(new List<NoteStatus>()
            {
                NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void
            });
        }
    }
}
