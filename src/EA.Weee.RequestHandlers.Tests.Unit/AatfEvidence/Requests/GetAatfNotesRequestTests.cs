﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GetAatfNotesRequestTests
    {
        private readonly Guid organisationId;
        private readonly Guid aatfId;
        private readonly Fixture fixture;

        public GetAatfNotesRequestTests()
        {
            fixture = new Fixture();
            organisationId = fixture.Create<Guid>();
            aatfId = fixture.Create<Guid>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenEmptyOrganisationArgumentExceptionExpected()
        {
            // act
            var result = Record.Exception(() => new GetAatfNotesRequest(Guid.Empty, 
                aatfId, 
                fixture.CreateMany<NoteStatus>().ToList(), 
                null));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenEmptyAatfArgumentExceptionExpected()
        {
            // act
            var result = Record.Exception(() => new GetAatfNotesRequest(organisationId, 
                Guid.Empty, 
                fixture.CreateMany<NoteStatus>().ToList(), 
                null));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenEmptyAllowedStatusArgumentExceptionExpected()
        {
            // act
            var result = Record.Exception(() => new GetAatfNotesRequest(organisationId, Guid.Empty, new List<NoteStatus>(), null));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAatfNotesRequest_ConstructorListOfAllowedStatusIsNull_ArgumentNullExceptionExpected()
        {
            // act
            var result = Record.Exception(() => new GetAatfNotesRequest(organisationId, aatfId, null, null));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenDraftEvidenceNoteValuesWithListOfAllowedStatuses_PropertiesShouldBeSet()
        {
            // arrange 
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Draft };

            // act
            var result = new GetAatfNotesRequest(organisationId, aatfId, allowedStatuses, null);

            // assert
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.AllowedStatuses.Should().BeEquivalentTo(allowedStatuses);
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenDraftEvidenceNoteValues_AllowedStatusesShouldBeSet()
        {
            // act
            var allowedStatus = new List<NoteStatus>() { NoteStatus.Approved };
            var searchRef = fixture.Create<string>();

            var result = new GetAatfNotesRequest(organisationId, aatfId, allowedStatus, searchRef);

            // assert
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.AllowedStatuses.Should().BeEquivalentTo(allowedStatus);
            result.SearchRef.Should().Be(searchRef);
        }
    }
}
