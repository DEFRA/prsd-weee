namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using System;
    using System.Collections.Generic;
    using Core.DataReturns;
    using FluentAssertions;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class GetEvidenceNotesForTransferRequestTests
    {
        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenEmptyGuid_ArgumentExceptionExpected()
        {
            var exception = Record.Exception(() => new GetEvidenceNotesForTransferRequest(Guid.Empty,
                new List<WeeeCategory>()
                {
                    WeeeCategory.ConsumerEquipment
                }));

            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenEmptyCategories_ArgumentExceptionExpected()
        {
            var exception = Record.Exception(() => new GetEvidenceNotesForTransferRequest(Guid.NewGuid(), 
                new List<WeeeCategory>()));

            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenNullCategories_ArgumentExceptionExpected()
        {
            var exception = Record.Exception(() => new GetEvidenceNotesForTransferRequest(Guid.NewGuid(), null));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetEvidenceNotesForTransferRequest_GivenValues_PropertiesShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var categories = new List<WeeeCategory>() { WeeeCategory.ConsumerEquipment, WeeeCategory.DisplayEquipment };

            var request = new GetEvidenceNotesForTransferRequest(organisationId, categories);

            request.Categories.Should().BeEquivalentTo(categories);
            request.OrganisationId.Should().Be(organisationId);
        }
    }
}
