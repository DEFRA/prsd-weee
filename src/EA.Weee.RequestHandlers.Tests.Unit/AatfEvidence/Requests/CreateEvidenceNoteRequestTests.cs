namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.Aatf;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Weee.Requests.Aatf;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class CreateEvidenceNoteRequestTests
    {
        private readonly Guid organisationId;
        private readonly Guid schemeId;
        private readonly DateTime startDate;
        private readonly DateTime endDate;
        private WasteType? wasteType;
        private Protocol? protocol;
        private readonly Guid aatfId;
        private readonly IEnumerable<TonnageValues> tonnages;
        private readonly NoteStatus status;

        public CreateEvidenceNoteRequestTests()
        {
            var fixture = new Fixture();
            organisationId = fixture.Create<Guid>();
            schemeId = fixture.Create<Guid>();
            startDate = DateTime.Now.AddDays(1);
            endDate = DateTime.Now.AddDays(2);
            wasteType = fixture.Create<WasteType>();
            protocol = fixture.Create<Protocol>();
            aatfId = fixture.Create<Guid>();
            fixture.Create<string>();
            tonnages = fixture.CreateMany<TonnageValues>();
            status = fixture.Create<NoteStatus>();
        }

        [Fact]
        public void CreateEvidenceNote_Constructor_GivenEmptyOrganisationArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new CreateEvidenceNoteRequest(Guid.Empty, aatfId,
                schemeId,
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                tonnages.ToList(),
                status,
                Guid.Empty));

                result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void CreateEvidenceNote_Constructor_GivenEmptyAatfArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new CreateEvidenceNoteRequest(organisationId, Guid.Empty, 
                schemeId,
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                tonnages.ToList(),
                status,
                Guid.Empty));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void CreateEvidenceNote_Constructor_GivenEmptySchemeArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new CreateEvidenceNoteRequest(organisationId, aatfId,
                Guid.Empty, 
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                tonnages.ToList(),
                status,
                Guid.Empty));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void CreateEvidenceNote_Constructor_GivenEmptyStartDateArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new CreateEvidenceNoteRequest(organisationId, aatfId,
                schemeId,
                DateTime.MinValue,
                DateTime.Now,
                null,
                null,
                tonnages.ToList(),
                status,
                Guid.Empty));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void CreateEvidenceNote_Constructor_GivenEmptyEndDateArgumentExceptionExpected()
        {
            var result = Record.Exception(() => new CreateEvidenceNoteRequest(organisationId, aatfId,
                schemeId,
                DateTime.Now,
                DateTime.MinValue,
                null,
                null,
                tonnages.ToList(),
                status,
                Guid.Empty));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void CreateEvidenceNote_Constructor_GivenDraftEvidenceNoteValues_PropertiesShouldBeSet()
        {
            var result = CreateNoteRequest();

            ShouldBeEqualTo(result);
        }

        [Fact]
        public void CreateEvidenceNote_Constructor_GivenDraftEvidenceNoteWithNullValuesValues_PropertiesShouldBeSet()
        {
            wasteType = null;
            protocol = null;

            var result = CreateNoteRequest();

            ShouldBeEqualTo(result);
        }

        private void ShouldBeEqualTo(CreateEvidenceNoteRequest result)
        {
            result.RecipientId.Should().Be(schemeId);
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.WasteType.Should().Be(wasteType);
            result.Protocol.Should().Be(protocol);
            result.StartDate.Should().Be(startDate);
            result.EndDate.Should().Be(endDate);
            result.TonnageValues.Should().BeEquivalentTo(tonnages);
            result.Status.Should().Be(status);
        }

        public CreateEvidenceNoteRequest CreateNoteRequest()
        {
            return new CreateEvidenceNoteRequest(organisationId,
                aatfId,
                schemeId,
                startDate,
                endDate,
                wasteType,
                protocol,
                tonnages.ToList(),
                status,
                Guid.Empty);
        }
    }
}