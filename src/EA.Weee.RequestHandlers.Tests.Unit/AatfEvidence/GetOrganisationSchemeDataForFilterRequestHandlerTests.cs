namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using AutoFixture;
    using DataAccess.DataAccess;
    using Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfEvidence;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using Xunit;

    public class GetOrganisationSchemeDataForFilterRequestHandlerTests : SimpleUnitTestBase
    {
        private GetOrganisationSchemeDataForFilterRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly GetOrganisationSchemeDataForFilterRequest request;

        public GetOrganisationSchemeDataForFilterRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            noteDataAccess = A.Fake<IEvidenceDataAccess>();

            request = new GetOrganisationSchemeDataForFilterRequest(TestFixture.Create<Guid>(),
                TestFixture.Create<int>());

            handler = new GetOrganisationSchemeDataForFilterRequestHandler(weeeAuthorization, noteDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetOrganisationSchemeDataForFilterRequestHandler(authorization, noteDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            handler = new GetOrganisationSchemeDataForFilterRequestHandler(authorization, noteDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckExternalAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_EvidenceDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => noteDataAccess.GetRecipientOrganisations(request.OrganisationId,
                request.ComplianceYear)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRecipientOrganisations_OrganisationDataShouldBeReturned()
        {
            //arrange
            var organisationPbs = Organisation.CreateRegisteredCompany(TestFixture.Create<string>(), "1234567");
            ObjectInstantiator<Organisation>.SetProperty(o => o.ProducerBalancingScheme, new ProducerBalancingScheme(), organisationPbs);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Id, TestFixture.Create<Guid>(), organisationPbs);

            var organisation1WithScheme = A.Fake<Organisation>();
            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => organisation1WithScheme.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => organisation1WithScheme.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => scheme1.SchemeName).Returns(TestFixture.Create<string>());
            A.CallTo(() => organisation1WithScheme.Schemes).Returns(new List<Scheme>() { scheme1 });
            
            var organisation2WithScheme = A.Fake<Organisation>();
            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => organisation2WithScheme.Id).Returns(TestFixture.Create<Guid>());
            A.CallTo(() => organisation2WithScheme.ProducerBalancingScheme).Returns(null);
            A.CallTo(() => scheme2.SchemeName).Returns(TestFixture.Create<string>());
            A.CallTo(() => organisation2WithScheme.Schemes).Returns(new List<Scheme>() { scheme2 });

            A.CallTo(() => noteDataAccess.GetRecipientOrganisations(A<Guid>._, A<int>._))
                .Returns(new List<Organisation>() { organisation1WithScheme, organisationPbs, organisation2WithScheme });

            // act
            var result = await handler.HandleAsync(request);

            //assert
            result.Count.Should().Be(3);
            result.Should().BeInAscendingOrder(r => r.DisplayName);
            result.Should().Contain(r => r.DisplayName.Equals(scheme1.SchemeName));
            result.Should().Contain(r => r.DisplayName.Equals(scheme2.SchemeName));
            result.Should().Contain(r => r.DisplayName.Equals(organisationPbs.OrganisationName));
            result.Should().Contain(r => r.Id == organisationPbs.Id);
            result.Should().Contain(r => r.Id == organisation1WithScheme.Id);
            result.Should().Contain(r => r.Id == organisation2WithScheme.Id);
        }
    }
}
