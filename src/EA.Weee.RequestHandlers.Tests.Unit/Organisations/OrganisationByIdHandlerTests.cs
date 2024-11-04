namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using Core.Organisations;
    using DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Security;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class OrganisationByIdHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> map;
        private readonly DbContextHelper dbHelper = new DbContextHelper();
        private readonly OrganisationByIdHandler handler;
        private readonly Guid organisationId;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public OrganisationByIdHandlerTests()
        {
            map = A.Fake<IMap<Organisation, OrganisationData>>();
            context = A.Fake<WeeeContext>();
            organisationId = Guid.NewGuid();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                GetOrganisationWithId(organisationId)
            }));

            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Domain.Scheme.Scheme>()));
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>()));
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(new List<DirectRegistrant>()));

            handler = new OrganisationByIdHandler(AuthorizationBuilder.CreateUserAllowedToAccessOrganisation(),
                context,
                map,
                systemDataDataAccess);
        }

        [Fact]
        public async Task OrganisationByIdHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new OrganisationByIdHandler(authorization, context, map, systemDataDataAccess);
            var message = new GetOrganisationInfo(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task OrganisationByIdHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            var authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var handler = new OrganisationByIdHandler(authorization, context, map, systemDataDataAccess);
            var message = new GetOrganisationInfo(organisationId);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.Contains(organisationId.ToString(), exception.Message);
            Assert.Contains("COULD NOT FIND", exception.Message.ToUpperInvariant());
            Assert.Contains("ORGANISATION", exception.Message.ToUpperInvariant());
        }

        [Fact]
        public async Task OrganisationByIdHandler_HappyPath_ReturnsOrganisationFromId()
        {
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                GetOrganisationWithId(organisationId)
            }));

            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            Assert.NotNull(expectedReturnValue);
            Assert.Same(expectedReturnValue, result);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasNoScheme_OrganisationSchemeIdShouldBeNull()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Domain.Scheme.Scheme>()));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.SchemeId.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasScheme_OrganisationSchemeIdShouldBeSet()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var scheme = A.Fake<Domain.Scheme.Scheme>();
            A.CallTo(() => scheme.Id).Returns(Guid.NewGuid());
            A.CallTo(() => scheme.OrganisationId).Returns(organisation.Id);

            var schemes = new List<Domain.Scheme.Scheme> { scheme };

            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(schemes));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.SchemeId.Should().Be(scheme.Id);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasNoAatfs_OrganisationHasAatfsShouldBeFalse()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>()));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasAatfs.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasNoAes_OrganisationHasAesShouldBeFalse()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>()));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasAes.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasAes_OrganisationHasAesShouldBeTrue()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            var ae = A.Fake<Aatf>();

            A.CallTo(() => ae.FacilityType).Returns(FacilityType.Ae);
            A.CallTo(() => ae.Organisation).Returns(organisation);
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>() { ae }));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasAes.Should().BeTrue();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasAatfs_OrganisationHasAatfsShouldBeTrue()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasAatfs.Should().BeTrue();
        }

        [Fact]
        public async Task OrganisationByIdHandler_ReturnsFalseForCanEditOrganisation_WhenCurrentUserIsNotInternalAdmin()
        {
            var weeeAuthorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new OrganisationByIdHandler(weeeAuthorization, context, map, systemDataDataAccess);

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.CanEditOrganisation.Should().BeFalse();
        }

        [Fact]
        public async Task OrganisationByIdHandler_GivenMappedOrganisation_MappedOrganisationShouldBeReturned()
        {
            var message = new GetOrganisationInfo(organisationId);

            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var result = await handler.HandleAsync(message);

            result.Should().Be(expectedReturnValue);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasNoDirectRegistrant_OrganisationShouldHaveNoDirectRegistrant()
        {
            var expectedReturnValue = new OrganisationData();

            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(new List<DirectRegistrant>()));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasDirectRegistrant.Should().BeFalse();
            result.DirectRegistrantId.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasDirectRegistrant_OrganisationShouldHaveDirectRegistrant()
        {
            // Arrange
            var organisation = GetOrganisationWithId(organisationId);
            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisation.Id);

            var knownComplianceDate = new DateTime(2024, 1, 1);

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(knownComplianceDate);

            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>
            {
                new DirectProducerSubmission { ComplianceYear = knownComplianceDate.Year }
            });

            var directRegistrants = new List<DirectRegistrant> { directRegistrant };
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(directRegistrants));

            var message = new GetOrganisationInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.HasDirectRegistrant.Should().BeTrue();
            result.DirectRegistrants.Should().ContainSingle(dr => dr.DirectRegistrantId == directRegistrantId);
            result.DirectRegistrants.First().YearSubmissionStarted.Should().BeTrue();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationRepresentsNoCompanies_IsRepresentingCompanyShouldBeFalse()
        {
            // Arrange
            var organisation = GetOrganisationWithId(organisationId);
            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisation.Id);

            var directRegistrants = new List<DirectRegistrant> { directRegistrant };
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(directRegistrants));

            var message = new GetOrganisationInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.IsRepresentingCompany.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationRepresentsCompanies_IsRepresentingCompanyShouldBeTrue()
        {
            // Arrange
            var organisation = GetOrganisationWithId(organisationId);
            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisation.Id);
            A.CallTo(() => directRegistrant.AuthorisedRepresentativeId).Returns(new Guid());

            var directRegistrants = new List<DirectRegistrant> { directRegistrant };
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(directRegistrants));

            var message = new GetOrganisationInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.IsRepresentingCompany.Should().BeTrue();
        }

        [Fact]
        public async Task HandleAsync_GivenOtherOrganisationRepresentsCompanies_IsRepresentingCompanyShouldBeFalse()
        {
            // Arrange
            var organisation = GetOrganisationWithId(organisationId);
            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid()); // Different organization
            A.CallTo(() => directRegistrant.AuthorisedRepresentativeId).Returns(new Guid());

            var directRegistrants = new List<DirectRegistrant> { directRegistrant };
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(directRegistrants));

            var message = new GetOrganisationInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.IsRepresentingCompany.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_GivenDirectRegistrantWithNoSubmissions_MostRecentSubmittedYearShouldBeZero()
        {
            // Arrange
            var organisation = GetOrganisationWithId(organisationId);
            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisation.Id);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(new List<DirectProducerSubmission>());

            var directRegistrants = new List<DirectRegistrant> { directRegistrant };
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(directRegistrants));

            var message = new GetOrganisationInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.DirectRegistrants.Single().MostRecentSubmittedYear.Should().Be(0);
        }

        [Fact]
        public async Task HandleAsync_GivenDirectRegistrantWithCompletedSubmissions_ShouldReturnMostRecentYear()
        {
            // Arrange
            var organisation = GetOrganisationWithId(organisationId);
            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisation.Id);

            var submissions = new List<DirectProducerSubmission>
            {
                new DirectProducerSubmission { ComplianceYear = 2022, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete },
                new DirectProducerSubmission { ComplianceYear = 2023, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete },
                new DirectProducerSubmission { ComplianceYear = 2024, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete }
            };

            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            var directRegistrants = new List<DirectRegistrant> { directRegistrant };
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(directRegistrants));

            var message = new GetOrganisationInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.DirectRegistrants.Single().MostRecentSubmittedYear.Should().Be(2023);
        }

        [Fact]
        public async Task HandleAsync_GivenDirectRegistrantWithReturnedSubmissions_ShouldReturnMostRecentYear()
        {
            // Arrange
            var organisation = GetOrganisationWithId(organisationId);
            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisation.Id);

            var submissions = new List<DirectProducerSubmission>
            {
                new DirectProducerSubmission { ComplianceYear = 2022, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete },
                new DirectProducerSubmission { ComplianceYear = 2023, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Returned },
                new DirectProducerSubmission { ComplianceYear = 2024, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete }
            };

            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            var directRegistrants = new List<DirectRegistrant> { directRegistrant };
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(directRegistrants));

            var message = new GetOrganisationInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.DirectRegistrants.Single().MostRecentSubmittedYear.Should().Be(2023);
        }

        [Fact]
        public async Task HandleAsync_GivenDirectRegistrantWithMixedStatusSubmissions_ShouldOnlyConsiderCompleteAndReturnedYears()
        {
            // Arrange
            var organisation = GetOrganisationWithId(organisationId);
            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var directRegistrant = A.Fake<DirectRegistrant>();
            var directRegistrantId = Guid.NewGuid();

            A.CallTo(() => directRegistrant.Id).Returns(directRegistrantId);
            A.CallTo(() => directRegistrant.OrganisationId).Returns(organisation.Id);

            var submissions = new List<DirectProducerSubmission>
            {
                new DirectProducerSubmission { ComplianceYear = 2022, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete },
                                new DirectProducerSubmission { ComplianceYear = 2024, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete },
                new DirectProducerSubmission { ComplianceYear = 2021, DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Returned }
            };

            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            var directRegistrants = new List<DirectRegistrant> { directRegistrant };
            A.CallTo(() => context.DirectRegistrants).Returns(dbHelper.GetAsyncEnabledDbSet(directRegistrants));

            var message = new GetOrganisationInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            result.DirectRegistrants.Single().MostRecentSubmittedYear.Should().Be(2022);
        }

        private Organisation GetOrganisationWithId(Guid id)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(id);
            return organisation;
        }
    }
}
