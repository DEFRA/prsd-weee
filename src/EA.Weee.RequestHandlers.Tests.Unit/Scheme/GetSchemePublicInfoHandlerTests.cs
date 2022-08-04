namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using AutoFixture;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Scheme.GetSchemePublicInfo;
    using Requests.Scheme;
    using System;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemePublicInfoHandlerTests : SimpleUnitTestBase
    {
        [Fact]
        public async void HandleAsync_OrganisationIsNotBalancingScheme_ReturnsSchemeData()
        {
            // Arrange
            var dataAccess = A.Fake<IGetSchemePublicInfoDataAccess>();
            var organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            var scheme = A.Fake<Scheme>();
            var organisationId = Guid.NewGuid();
            var organisation = A.Fake<Organisation>();

            A.CallTo(() => organisation.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => scheme.Id).Returns(Guid.NewGuid());
            A.CallTo(() => scheme.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => scheme.SchemeName).Returns("scheme");
            A.CallTo(() => scheme.ApprovalNumber).Returns("approval");
            A.CallTo(() => scheme.SchemeStatus).Returns(SchemeStatus.Approved);
            A.CallTo(() => dataAccess.FetchSchemeByOrganisationId(organisationId)).Returns(scheme);
            A.CallTo(() => organisationDataAccess.GetById(organisationId)).Returns(organisation);

            var handler = new GetSchemePublicInfoHandler(dataAccess, organisationDataAccess);
            var request = new GetSchemePublicInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.OrganisationId.Should().Be(scheme.OrganisationId);
            result.Name.Should().Be(scheme.SchemeName);
            result.ApprovalNo.Should().Be(scheme.ApprovalNumber);
            result.SchemeId.Should().Be(scheme.Id);
            result.Status.Should().Be(Core.Shared.SchemeStatus.Approved);
            result.StatusName.Should().Be(Core.Shared.SchemeStatus.Approved.ToDisplayString());
            result.IsBalancingScheme.Should().BeFalse();
        }

        [Fact]
        public async void HandleAsync_OrganisationIsBalancingScheme_ReturnsSchemeData()
        {
            // Arrange
            var dataAccess = A.Fake<IGetSchemePublicInfoDataAccess>();
            var organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            var organisationId = Guid.NewGuid();
            var organisationName = TestFixture.Create<string>();
            var organisation = Organisation.CreateRegisteredCompany(organisationName, "1234567");
            ObjectInstantiator<Organisation>.SetProperty(o => o.Id, organisationId, organisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.ProducerBalancingScheme, A.Fake<ProducerBalancingScheme>(), organisation);

            A.CallTo(() => organisationDataAccess.GetById(organisationId)).Returns(organisation);

            var handler = new GetSchemePublicInfoHandler(dataAccess, organisationDataAccess);
            var request = new GetSchemePublicInfo(organisationId);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.OrganisationId.Should().Be(organisationId);
            result.Name.Should().Be(organisationName);
            result.IsBalancingScheme.Should().BeTrue();
        }
    }
}
