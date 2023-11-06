﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using AutoFixture;
    using EA.Prsd.Core;
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
                null, SystemTime.UtcNow.Year, null, new List<WasteType>() { WasteType.Household }, null, null, null, int.MaxValue, 1));

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
                null, SystemTime.UtcNow.Year, null, new List<WasteType>() { WasteType.Household }, null, null, null, int.MaxValue, 1));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenEmptyAllowedStatusArgumentExceptionExpected()
        {
            // act
            var result = Record.Exception(() => new GetAatfNotesRequest(organisationId, Guid.Empty, new List<NoteStatus>(), null, SystemTime.UtcNow.Year, null, new List<WasteType>() { WasteType.Household }, null, null, null, int.MaxValue, 1));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetAatfNotesRequest_ConstructorListOfAllowedStatusIsNull_ArgumentNullExceptionExpected()
        {
            // act
            var result = Record.Exception(() => new GetAatfNotesRequest(organisationId, aatfId, null, null, SystemTime.UtcNow.Year, null, new List<WasteType>() { WasteType.Household }, null, null, null, int.MaxValue, 1));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenDraftEvidenceNoteValuesWithListOfAllowedStatuses_PropertiesShouldBeSet()
        {
            // arrange 
            var allowedStatuses = new List<NoteStatus> { NoteStatus.Draft };

            // act
            var result = new GetAatfNotesRequest(organisationId, aatfId, allowedStatuses, null, SystemTime.UtcNow.Year, null, new List<WasteType>() { WasteType.Household }, null, null, null, int.MaxValue, 1);

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

            var result = new GetAatfNotesRequest(organisationId, aatfId, allowedStatus, searchRef, SystemTime.UtcNow.Year, null, new List<WasteType>() { WasteType.Household }, null, null, null, int.MaxValue, 1);

            // assert
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.AllowedStatuses.Should().BeEquivalentTo(allowedStatus);
            result.SearchRef.Should().Be(searchRef);
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenApprovedStatusAllFiltersSet_AllowedStatusesShouldBeSet()
        {
            // act
            var allowedStatus = new List<NoteStatus>() { NoteStatus.Approved };
            var allowedWasteType = new List<WasteType>() { WasteType.Household };
            var searchRef = fixture.Create<string>();
            var recievedId = fixture.Create<Guid>();
            var noteStatus = fixture.Create<NoteStatus>();
            var startDate = fixture.Create<DateTime>();
            var endDate = fixture.Create<DateTime>();
            var selectedComplianceYear = fixture.Create<int>();

            var result = new GetAatfNotesRequest(organisationId, aatfId, allowedStatus, searchRef, selectedComplianceYear, recievedId, allowedWasteType, noteStatus, startDate, endDate, int.MaxValue, 1);

            // assert
            result.OrganisationId.Should().Be(organisationId);
            result.AatfId.Should().Be(aatfId);
            result.AllowedStatuses.Should().BeEquivalentTo(allowedStatus);
            result.SearchRef.Should().Be(searchRef);
            result.RecipientId.Should().Be(recievedId);
            result.ObligationTypeFilterList.Should().BeEquivalentTo(allowedWasteType);
            result.NoteStatusFilter.Should().Be(noteStatus);
            result.StartDateSubmitted.Should().Be(startDate);
            result.EndDateSubmitted.Should().Be(endDate);
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenZeroPageNumber_ShouldThrowArgumentOutOfRangeException()
        {
            // act
            var allowedStatus = new List<NoteStatus>() { NoteStatus.Approved };
            var allowedWasteType = new List<WasteType>() { WasteType.Household };
            var searchRef = fixture.Create<string>();
            var recievedId = fixture.Create<Guid>();
            var wasteType = fixture.Create<WasteType>();
            var noteStatus = fixture.Create<NoteStatus>();
            var startDate = fixture.Create<DateTime>();
            var endDate = fixture.Create<DateTime>();
            var selectedComplianceYear = fixture.Create<int>();

            //arrange
            var exception = Record.Exception(() => new GetAatfNotesRequest(organisationId, aatfId, allowedStatus, searchRef, selectedComplianceYear, recievedId, allowedWasteType, noteStatus, startDate, endDate, int.MaxValue, 0));

            //assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetAatfNotesRequest_Constructor_GivenZeroPageSize_ShouldThrowArgumentOutOfRangeException()
        {
            // act
            var allowedStatus = new List<NoteStatus>() { NoteStatus.Approved };
            var allowedWasteType = new List<WasteType>() { WasteType.Household };
            var searchRef = fixture.Create<string>();
            var recievedId = fixture.Create<Guid>();
            var wasteType = fixture.Create<WasteType>();
            var noteStatus = fixture.Create<NoteStatus>();
            var startDate = fixture.Create<DateTime>();
            var endDate = fixture.Create<DateTime>();
            var selectedComplianceYear = fixture.Create<int>();

            //arrange
            var exception = Record.Exception(() => new GetAatfNotesRequest(organisationId, aatfId, allowedStatus, searchRef, selectedComplianceYear, recievedId, allowedWasteType, noteStatus, startDate, endDate, 0, 1));

            //assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }
    }
}
