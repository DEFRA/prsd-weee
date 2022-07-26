namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using AutoFixture;
    using Core.DataReturns;
    using Core.Helpers;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GetEvidenceNotesForTransferRequestTests : SimpleUnitTestBase
    {
        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenEmptyGuid_ArgumentExceptionExpected()
        {
            var exception = Record.Exception(() => new GetEvidenceNotesForTransferRequest(Guid.Empty,
                new List<int>()
                {
                    WeeeCategory.ConsumerEquipment.ToInt()
                }, TestFixture.Create<int>()));

            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenEmptyCategories_ArgumentExceptionExpected()
        {
            var exception = Record.Exception(() => new GetEvidenceNotesForTransferRequest(Guid.NewGuid(), 
                new List<int>(), TestFixture.Create<int>()));

            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenNullCategories_ArgumentExceptionExpected()
        {
            var exception = Record.Exception(() => new GetEvidenceNotesForTransferRequest(Guid.NewGuid(), null, TestFixture.Create<int>()));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenValues_PropertiesShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var categories = new List<int>() { WeeeCategory.ConsumerEquipment.ToInt(), WeeeCategory.DisplayEquipment.ToInt() };
            var complianceYear = TestFixture.Create<int>();

            var request = new GetEvidenceNotesForTransferRequest(organisationId, categories, complianceYear);

            request.Categories.Should().BeEquivalentTo(categories);
            request.OrganisationId.Should().Be(organisationId);
            request.EvidenceNotes.Should().BeEmpty();
            request.ComplianceYear.Should().Be(complianceYear);
        }

        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenValuesWithEvidenceNotes_PropertiesShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var categories = new List<int>() { WeeeCategory.ConsumerEquipment.ToInt(), WeeeCategory.DisplayEquipment.ToInt() };
            var notes = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() };
            var complianceYear = TestFixture.Create<int>();

            var request = new GetEvidenceNotesForTransferRequest(organisationId, categories, complianceYear, notes);

            request.Categories.Should().BeEquivalentTo(categories);
            request.OrganisationId.Should().Be(organisationId);
            request.EvidenceNotes.Should().BeEquivalentTo(notes);
        }
    }
}
