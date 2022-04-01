﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.FindMatchingOrganisations.DataAccess
{
    using Core.Helpers;
    using Domain.Organisation;
    using Domain.User;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Organisations.FindMatchingOrganisations.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.DataAccess;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class FindMatchingOrganisationsDataAccessTests
    {
        private readonly WeeeContext context;

        private readonly DbContextHelper dbContextHelper;
        private readonly OrganisationHelper orgHelper = new OrganisationHelper();

        public FindMatchingOrganisationsDataAccessTests()
        {
            context = A.Fake<WeeeContext>();

            dbContextHelper = new DbContextHelper();

            // By default, all DbSets used by these tests return an empty list of data
            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            A.CallTo(() => context.OrganisationUsers)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OrganisationUser>()));
        }

        [Theory]
        [InlineData("a", "a company name")] // First letter matches
        [InlineData("A", "a company name")] // First letter matches
        [InlineData("a", "A company name")] // First letter matches
        [InlineData("a", "THE company")] // Company starts with "THE"
        [InlineData("a", "the company")] // Company starts with "THE"
        public async void GetOrganisationsBySimpleSearchTerm_SearchIsValid_AndNoExistingOrganisationUsers_ReturnsOrganisation(string searchTerm, string organisationName)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.TradingName).Returns(organisationName);
            A.CallTo(() => organisation.OrganisationStatus).Returns(OrganisationStatus.Complete);

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsBySimpleSearchTerm(searchTerm, Guid.Empty);

            Assert.Single(result);
            Assert.Equal(organisation, result.Single());
        }

        [Theory]
        [InlineData("a", "b company")]
        [InlineData("b", "a company")]
        public async void GetOrganisationBySimpleSearchTerm_SearchIsNotValid_AndNoExistingOrganisationUsers_DoesNotReturnOrganisation(string searchTerm, string organisationName)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.TradingName).Returns(organisationName);
            A.CallTo(() => organisation.OrganisationStatus).Returns(OrganisationStatus.Complete);

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsBySimpleSearchTerm(searchTerm, Guid.Empty);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000", Core.Shared.UserStatus.Rejected, Core.Shared.OrganisationStatus.Complete)] // Matching guids & complete org, but rejected status 
        [InlineData("7CE63EB0-0D7D-46AA-9385-EA2DC6D9B2F1", "00000000-0000-0000-0000-000000000000", Core.Shared.UserStatus.Active, Core.Shared.OrganisationStatus.Complete)] // Active status & complete org, but non-matching guids
        [InlineData("7CE63EB0-0D7D-46AA-9385-EA2DC6D9B2F1", "00000000-0000-0000-0000-000000000000", Core.Shared.UserStatus.Pending, Core.Shared.OrganisationStatus.Complete)] // Pending status & complete org, but non-matching guids
        [InlineData("7CE63EB0-0D7D-46AA-9385-EA2DC6D9B2F1", "00000000-0000-0000-0000-000000000000", Core.Shared.UserStatus.Inactive, Core.Shared.OrganisationStatus.Complete)] // Inactive status & complete org, but non-matching guids
        public async void
            GetOrganisationBySimpleSearchTerm_SearchIsValid_AndNoConflictingOrganisationUserExists_ReturnsOrganisation(
            string existingUserId, string userId, Core.Shared.UserStatus userStatus, Core.Shared.OrganisationStatus organisationStatus)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.TradingName).Returns("A Company");
            A.CallTo(() => organisation.OrganisationStatus).Returns(organisationStatus.ToDomainEnumeration<OrganisationStatus>());

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var organisationUser = new OrganisationUser(new Guid(userId), new Guid(existingUserId), userStatus.ToDomainEnumeration<UserStatus>());
            A.CallTo(() => context.OrganisationUsers)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OrganisationUser>
                {
                    organisationUser
                }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsBySimpleSearchTerm("A", new Guid(userId));

            Assert.Single(result);
            Assert.Equal(organisation, result.Single());
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000", Core.Shared.UserStatus.Active, Core.Shared.OrganisationStatus.Complete)] // Matching guids, active status & complete org
        [InlineData("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000", Core.Shared.UserStatus.Pending, Core.Shared.OrganisationStatus.Complete)] // Matching guids, active status & complete org
        [InlineData("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000", Core.Shared.UserStatus.Inactive, Core.Shared.OrganisationStatus.Complete)] // Inactive status & complete org, but non-matching guids
        [InlineData("00000000-0000-0000-0000-000000000000", "AE05ADBD-F954-48FB-B7FA-57041CA8145A", Core.Shared.UserStatus.Active, Core.Shared.OrganisationStatus.Complete)] // Active status & complete org, but non-matching guids
        [InlineData("00000000-0000-0000-0000-000000000000", "AE05ADBD-F954-48FB-B7FA-57041CA8145A", Core.Shared.UserStatus.Pending, Core.Shared.OrganisationStatus.Complete)] // Pending status & complete org, but non-matching guids
        [InlineData("00000000-0000-0000-0000-000000000000", "AE05ADBD-F954-48FB-B7FA-57041CA8145A", Core.Shared.UserStatus.Inactive, Core.Shared.OrganisationStatus.Complete)] // Inactive status & complete org, but non-matching guids
        [InlineData("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000", Core.Shared.UserStatus.Rejected, Core.Shared.OrganisationStatus.Incomplete)] // Rejected status & matching guids, but incomplete org
        public async void
            GetOrganisationBySimpleSearchTerm_SearchIsValid_ButConflictingOrganisationUserExists_DoesNotReturnOrganisation(
            string existingUserId, string userId, Core.Shared.UserStatus userStatus, Core.Shared.OrganisationStatus organisationStatus)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.TradingName).Returns("A Company");
            A.CallTo(() => organisation.OrganisationStatus).Returns(organisationStatus.ToDomainEnumeration<OrganisationStatus>());

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var organisationUser = new OrganisationUser(new Guid(userId), new Guid(existingUserId), userStatus.ToDomainEnumeration<UserStatus>());
            A.CallTo(() => context.OrganisationUsers)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OrganisationUser>
                {
                    organisationUser
                }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsBySimpleSearchTerm("A", new Guid(userId));

            Assert.Empty(result);
        }

        [Fact]
        public async void GetOrganisationsByPartialSearchAsync_TradeName_PartialMatch_ReturnsResult()
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.TradingName).Returns("Test Partnership");
            A.CallTo(() => organisation.OrganisationStatus).Returns(Core.Shared.OrganisationStatus.Complete.ToDomainEnumeration<OrganisationStatus>());

            var userId = Guid.NewGuid();

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsByPartialSearchAsync("Test", userId);

            result.Count().Should().Be(1);
            result[0].TradingName.Should().Be("Test Partnership");
        }

        [Fact]
        public async void GetOrganisationsByPartialSearchAsync_Name_PartialMatch_ReturnsResult()
        {
            var organisation = orgHelper.GetOrganisationWithName("Test Organisation");
            var userId = Guid.NewGuid();

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsByPartialSearchAsync("Test", userId);

            result.Count().Should().Be(1);
            result[0].Name.Should().Be("Test Organisation");
        }

        [Fact]
        public async void GetOrganisationsByPartialSearchAsync_TradeName_NoMatch_ReturnsNoResult()
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.TradingName).Returns("Test Partnership");
            A.CallTo(() => organisation.OrganisationStatus).Returns(Core.Shared.OrganisationStatus.Complete.ToDomainEnumeration<OrganisationStatus>());

            var userId = Guid.NewGuid();

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsByPartialSearchAsync("Fail", userId);

            result.Count().Should().Be(0);
        }

        [Fact]
        public async void GetOrganisationsByPartialSearchAsync_Name_NoMatch_ReturnsNoResult()
        {
            var organisation = orgHelper.GetOrganisationWithName("Test Organisation");
            var userId = Guid.NewGuid();
            var existingUserId = Guid.NewGuid();

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            var organisationUser = new OrganisationUser(userId, organisation.Id, Core.Shared.UserStatus.Active.ToDomainEnumeration<UserStatus>());
            A.CallTo(() => context.OrganisationUsers)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OrganisationUser>
                {
                    organisationUser
                }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsByPartialSearchAsync("Fail", userId);

            result.Count().Should().Be(0);
        }

        [Fact]
        public async void GetOrganisationsByPartialSearchAsync_Match_ReturnsOnlyConpleteStatusResult()
        {
            var organisation = orgHelper.GetOrganisationWithName("Test Organisation");
            var org2 = orgHelper.GetOrganisationWithName("Test Organisation 2");
            org2.OrganisationStatus = OrganisationStatus.Incomplete;
            var userId = Guid.NewGuid();

            A.CallTo(() => context.Organisations)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation, org2 }));

            var result = await FindMatchingOrganisationsDataAccess().GetOrganisationsByPartialSearchAsync("Test", userId);

            result.Count().Should().Be(1);
            result[0].Name.Should().Be("Test Organisation");
        }

        private FindMatchingOrganisationsDataAccess FindMatchingOrganisationsDataAccess()
        {
            return new FindMatchingOrganisationsDataAccess(context);
        }
    }
}
