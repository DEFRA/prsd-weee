namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.DataHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class GetOrganisationSchemeHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, OrganisationSchemeData> schemeOrganisationMap;
        private readonly IMap<ProducerBalancingScheme, OrganisationSchemeData> producerBalancingSchemeOrganisationMap;
        private GetOrganisationSchemeHandler handler;

        public GetOrganisationSchemeHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.dataAccess = A.Fake<IGetSchemesDataAccess>();
            this.schemeOrganisationMap = A.Fake<IMap<Scheme, OrganisationSchemeData>>();
            this.producerBalancingSchemeOrganisationMap = A.Fake<IMap<ProducerBalancingScheme, OrganisationSchemeData>>();

            handler = new GetOrganisationSchemeHandler(dataAccess, schemeOrganisationMap, producerBalancingSchemeOrganisationMap, authorization);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetOrganisationSchemeHandler(dataAccess, schemeOrganisationMap, producerBalancingSchemeOrganisationMap, authorization);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetOrganisationScheme>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_GivenRequest_GetSchemesDataAccessIsCalled(bool pbs)
        {
            // arrange
            var request = new GetOrganisationScheme(pbs);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => dataAccess.GetCompleteSchemes()).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_GivenRequest_GetPBSDataAccessIsCalled(bool pbs)
        {
            // arrange
            var request = new GetOrganisationScheme(pbs);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => dataAccess.GetProducerBalancingScheme()).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenApprovedSchemesfData_SchemeOrganisationMapMustBeCalled()
        {
            var approvedSchemes = A.CollectionOfFake<Scheme>(3).Where(s => s.SchemeStatus == SchemeStatus.Approved).ToList();

            A.CallTo(() => dataAccess.GetCompleteSchemes()).ReturnsNextFromSequence(approvedSchemes);

            await handler.HandleAsync(A.Dummy<GetOrganisationScheme>());

            foreach (var scheme in approvedSchemes)
            {
                A.CallTo(() => schemeOrganisationMap.Map(scheme)).MustHaveHappenedOnceExactly();
            }
        }

        [Theory]
        [ClassData(typeof(SchemeStatusData))]
        public async Task HandleAsync_GivenNonApprovedSchemesfData_SchemesOrganisationMapShouldNotBeCalled(SchemeStatus status)
        {
            if (status == SchemeStatus.Approved)
            {
                return;
            }

            var schemes = A.CollectionOfFake<Scheme>(3).Where(s => s.SchemeStatus == status).ToList();

            A.CallTo(() => dataAccess.GetCompleteSchemes()).ReturnsNextFromSequence(schemes);

            await handler.HandleAsync(A.Dummy<GetOrganisationScheme>());

            foreach (var scheme in schemes)
            {
                A.CallTo(() => schemeOrganisationMap.Map(scheme)).MustNotHaveHappened();
            }
        }

        [Fact]
        public async Task HandleAsync_GivenPBSData_ProducerBalancingSchemeShouldBeCalled()
        {
            //arrange
            var dateTime = TestFixture.Create<DateTime>();
            var pbs = TestFixture.Create<ProducerBalancingScheme>();
            A.CallTo(() => dataAccess.GetProducerBalancingScheme()).Returns(pbs);

            //act
            await handler.HandleAsync(A.Dummy<GetOrganisationScheme>());

            //assert
            A.CallTo(() => producerBalancingSchemeOrganisationMap.Map(pbs)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenApprovedSchemesData_SchemeOrganisationDataMustBeMapped()
        {
            // arrange 
            var schemeApproved1 = A.Fake<Scheme>();
            var schemeApprovedOrganisationId1 = TestFixture.Create<Guid>();
            var schemeApprovedName1 = TestFixture.Create<string>();
            A.CallTo(() => schemeApproved1.OrganisationId).Returns(schemeApprovedOrganisationId1);
            A.CallTo(() => schemeApproved1.SchemeName).Returns(schemeApprovedName1);
            A.CallTo(() => schemeApproved1.SchemeStatus).Returns(SchemeStatus.Approved);

            var schemeApproved2 = A.Fake<Scheme>();
            var schemeApprovedOrganisationId2 = TestFixture.Create<Guid>();
            var schemeApprovedName2 = TestFixture.Create<string>();
            A.CallTo(() => schemeApproved2.OrganisationId).Returns(schemeApprovedOrganisationId2);
            A.CallTo(() => schemeApproved2.SchemeName).Returns(schemeApprovedName2);
            A.CallTo(() => schemeApproved2.SchemeStatus).Returns(SchemeStatus.Approved);

            var schemes = new List<Scheme>
            {
                 schemeApproved1,
                 schemeApproved2
            };

            A.CallTo(() => dataAccess.GetCompleteSchemes()).ReturnsNextFromSequence(schemes);

            // act
            await handler.HandleAsync(A.Dummy<GetOrganisationScheme>());

            // assert
            A.CallTo(() => schemeOrganisationMap.Map(A<Scheme>.That.Matches(s => s.SchemeName.Equals(schemeApprovedName1) &&
                                                                                 s.OrganisationId.Equals(schemeApprovedOrganisationId1) &&
                                                                                 s.SchemeStatus.Equals(SchemeStatus.Approved)))).MustHaveHappenedOnceExactly();

            A.CallTo(() => schemeOrganisationMap.Map(A<Scheme>.That.Matches(s => s.SchemeName.Equals(schemeApprovedName2) &&
                                                                     s.OrganisationId.Equals(schemeApprovedOrganisationId2) &&
                                                                     s.SchemeStatus.Equals(SchemeStatus.Approved)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenPBSData_ProducerBalancingSchemeDataMustBeMapped()
        {
            // arrange 
            var pbs = A.Fake<ProducerBalancingScheme>();
            var pbsOrganisation = new Organisation();
            var pbsOrganisationName = TestFixture.Create<string>();
            var pbsOrganisationId = TestFixture.Create<Guid>();

            A.CallTo(() => pbs.Organisation).Returns(pbsOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Id, pbsOrganisationId, pbsOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Name, pbsOrganisationName, pbsOrganisation);

            A.CallTo(() => dataAccess.GetProducerBalancingScheme()).Returns(pbs);

            // act
            await handler.HandleAsync(A.Dummy<GetOrganisationScheme>());

            // assert
            A.CallTo(() => producerBalancingSchemeOrganisationMap.Map(A<ProducerBalancingScheme>.That.Matches(s => s.Organisation.Name.Equals(pbsOrganisationName) &&
                                                                                                         s.Organisation.Id.Equals(pbsOrganisationId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenGetOrganisationSchemeIncludePBSIsFalse_VerifyMappedSchemesInResult()
        {
            // arrange 
            var schemeApproved1 = A.Fake<Scheme>();
            var schemeApprovedOrganisationId1 = TestFixture.Create<Guid>();
            var schemeApprovedName1 = "XXXXXX";
            A.CallTo(() => schemeApproved1.OrganisationId).Returns(schemeApprovedOrganisationId1);
            A.CallTo(() => schemeApproved1.SchemeName).Returns(schemeApprovedName1);
            A.CallTo(() => schemeApproved1.SchemeStatus).Returns(SchemeStatus.Approved);

            var schemeApproved2 = A.Fake<Scheme>();
            var schemeApprovedOrganisationId2 = TestFixture.Create<Guid>();
            var schemeApprovedName2 = "AAAAAAAA";
            A.CallTo(() => schemeApproved2.OrganisationId).Returns(schemeApprovedOrganisationId2);
            A.CallTo(() => schemeApproved2.SchemeName).Returns(schemeApprovedName2);
            A.CallTo(() => schemeApproved2.SchemeStatus).Returns(SchemeStatus.Approved);

            var schemes = new List<Scheme>
            {
                 schemeApproved1,
                 schemeApproved2
            };

            var organisationSchemeDataForScheme1 = GetOrganisationSchemeData(schemeApprovedName1, schemeApprovedOrganisationId1);
            var organisationSchemeDataForScheme2 = GetOrganisationSchemeData(schemeApprovedName2, schemeApprovedOrganisationId2);

            var pbs = A.Fake<ProducerBalancingScheme>();
            var pbsOrganisation = new Organisation();
            var pbsOrganisationName = TestFixture.Create<string>();
            var pbsOrganisationId = TestFixture.Create<Guid>();

            A.CallTo(() => pbs.Organisation).Returns(pbsOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Id, pbsOrganisationId, pbsOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Name, pbsOrganisationName, pbsOrganisation);

            A.CallTo(() => dataAccess.GetProducerBalancingScheme()).Returns(pbs);

            A.CallTo(() => dataAccess.GetCompleteSchemes()).ReturnsNextFromSequence(schemes);

            A.CallTo(() => schemeOrganisationMap.Map(A<Scheme>.That.Matches(s => s.SchemeName.Equals(schemeApprovedName1) &&
                                                                                 s.OrganisationId.Equals(schemeApprovedOrganisationId1) &&
                                                                                 s.SchemeStatus.Equals(SchemeStatus.Approved)))).Returns(organisationSchemeDataForScheme1);

            A.CallTo(() => schemeOrganisationMap.Map(A<Scheme>.That.Matches(s => s.SchemeName.Equals(schemeApprovedName2) &&
                                                         s.OrganisationId.Equals(schemeApprovedOrganisationId2) &&
                                                         s.SchemeStatus.Equals(SchemeStatus.Approved)))).Returns(organisationSchemeDataForScheme2);

            A.CallTo(() => producerBalancingSchemeOrganisationMap.Map(A<ProducerBalancingScheme>.That.Matches(s => s.Organisation.Name.Equals(pbsOrganisationName) &&
                                                                                                       s.Organisation.Id.Equals(pbsOrganisationId))));
            // act
            var result = await handler.HandleAsync(GetRequest(false));

            // assert
            result[1].DisplayName.Should().Be(schemeApprovedName1);
            result[1].Id.Should().Be(schemeApprovedOrganisationId1);
            result[0].DisplayName.Should().Be(schemeApprovedName2);
            result[0].Id.Should().Be(schemeApprovedOrganisationId2);
        }

        [Fact]
        public async Task HandleAsync_GivenGetOrganisationSchemeIncludePBSIsTrue_VerifyMappedSchemesAndPBSInResult()
        {
            // arrange 
            var schemeApproved1 = A.Fake<Scheme>();
            var schemeApprovedOrganisationId1 = TestFixture.Create<Guid>();
            var schemeApprovedName1 = TestFixture.Create<string>();
            A.CallTo(() => schemeApproved1.OrganisationId).Returns(schemeApprovedOrganisationId1);
            A.CallTo(() => schemeApproved1.SchemeName).Returns(schemeApprovedName1);
            A.CallTo(() => schemeApproved1.SchemeStatus).Returns(SchemeStatus.Approved);

            var schemeApproved2 = A.Fake<Scheme>();
            var schemeApprovedOrganisationId2 = TestFixture.Create<Guid>();
            var schemeApprovedName2 = TestFixture.Create<string>();
            A.CallTo(() => schemeApproved2.OrganisationId).Returns(schemeApprovedOrganisationId2);
            A.CallTo(() => schemeApproved2.SchemeName).Returns(schemeApprovedName2);
            A.CallTo(() => schemeApproved2.SchemeStatus).Returns(SchemeStatus.Approved);

            var schemes = new List<Scheme>
            {
                 schemeApproved1,
                 schemeApproved2
            };

            var organisationSchemeDataForScheme1 = GetOrganisationSchemeData(schemeApprovedName1, schemeApprovedOrganisationId1);
            var organisationSchemeDataForScheme2 = GetOrganisationSchemeData(schemeApprovedName2, schemeApprovedOrganisationId2);

            var pbs = A.Fake<ProducerBalancingScheme>();
            var pbsOrganisation = new Organisation();
            var pbsOrganisationName = TestFixture.Create<string>();
            var pbsOrganisationId = TestFixture.Create<Guid>();
            var organisationSchemeDataForPBS = GetOrganisationSchemeData(pbsOrganisationName, pbsOrganisationId);

            A.CallTo(() => pbs.Organisation).Returns(pbsOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Id, pbsOrganisationId, pbsOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Name, pbsOrganisationName, pbsOrganisation);

            A.CallTo(() => dataAccess.GetProducerBalancingScheme()).Returns(pbs);

            A.CallTo(() => dataAccess.GetCompleteSchemes()).ReturnsNextFromSequence(schemes);

            A.CallTo(() => schemeOrganisationMap.Map(A<Scheme>.That.Matches(s => s.SchemeName.Equals(schemeApprovedName1) &&
                                                                                 s.OrganisationId.Equals(schemeApprovedOrganisationId1) &&
                                                                                 s.SchemeStatus.Equals(SchemeStatus.Approved)))).Returns(organisationSchemeDataForScheme1);

            A.CallTo(() => schemeOrganisationMap.Map(A<Scheme>.That.Matches(s => s.SchemeName.Equals(schemeApprovedName2) &&
                                                         s.OrganisationId.Equals(schemeApprovedOrganisationId2) &&
                                                         s.SchemeStatus.Equals(SchemeStatus.Approved)))).Returns(organisationSchemeDataForScheme2);

            A.CallTo(() => producerBalancingSchemeOrganisationMap.Map(A<ProducerBalancingScheme>.That.Matches(s => s.Organisation.Name.Equals(pbsOrganisationName) &&
                                                                                                       s.Organisation.Id.Equals(pbsOrganisationId)))).Returns(organisationSchemeDataForPBS);
            // act
            var result = await handler.HandleAsync(GetRequest(true));

            // assert
            result.Contains(organisationSchemeDataForScheme1);
            result.Contains(organisationSchemeDataForScheme2);
            result.Contains(organisationSchemeDataForPBS);
        }

        [Fact]
        public async Task HandleAsync_GivenGetOrganisationSchemeIncludePBSIsTrue_VerifyMappedSchemesAndPBSInResultAreOrderdByName()
        {
            // arrange 
            var schemeApproved1 = A.Fake<Scheme>();
            var schemeApprovedOrganisationId1 = TestFixture.Create<Guid>();
            var schemeApprovedName1 = "AAAA";
            A.CallTo(() => schemeApproved1.OrganisationId).Returns(schemeApprovedOrganisationId1);
            A.CallTo(() => schemeApproved1.SchemeName).Returns(schemeApprovedName1);
            A.CallTo(() => schemeApproved1.SchemeStatus).Returns(SchemeStatus.Approved);

            var schemeApproved2 = A.Fake<Scheme>();
            var schemeApprovedOrganisationId2 = TestFixture.Create<Guid>();
            var schemeApprovedName2 = "XXXXXXX";
            A.CallTo(() => schemeApproved2.OrganisationId).Returns(schemeApprovedOrganisationId2);
            A.CallTo(() => schemeApproved2.SchemeName).Returns(schemeApprovedName2);
            A.CallTo(() => schemeApproved2.SchemeStatus).Returns(SchemeStatus.Approved);

            var schemes = new List<Scheme>
            {
                 schemeApproved1,
                 schemeApproved2
            };

            var organisationSchemeDataForScheme1 = GetOrganisationSchemeData(schemeApprovedName1, schemeApprovedOrganisationId1);
            var organisationSchemeDataForScheme2 = GetOrganisationSchemeData(schemeApprovedName2, schemeApprovedOrganisationId2);

            var pbs = A.Fake<ProducerBalancingScheme>();
            var pbsOrganisation = new Organisation();
            var pbsOrganisationName = "KKKKKKKK";
            var pbsOrganisationId = TestFixture.Create<Guid>();
            var organisationSchemeDataForPBS = GetOrganisationSchemeData(pbsOrganisationName, pbsOrganisationId);

            A.CallTo(() => pbs.Organisation).Returns(pbsOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Id, pbsOrganisationId, pbsOrganisation);
            ObjectInstantiator<Organisation>.SetProperty(o => o.Name, pbsOrganisationName, pbsOrganisation);

            A.CallTo(() => dataAccess.GetProducerBalancingScheme()).Returns(pbs);

            A.CallTo(() => dataAccess.GetCompleteSchemes()).ReturnsNextFromSequence(schemes);

            A.CallTo(() => schemeOrganisationMap.Map(A<Scheme>.That.Matches(s => s.SchemeName.Equals(schemeApprovedName1) &&
                                                                                 s.OrganisationId.Equals(schemeApprovedOrganisationId1) &&
                                                                                 s.SchemeStatus.Equals(SchemeStatus.Approved)))).Returns(organisationSchemeDataForScheme1);

            A.CallTo(() => schemeOrganisationMap.Map(A<Scheme>.That.Matches(s => s.SchemeName.Equals(schemeApprovedName2) &&
                                                         s.OrganisationId.Equals(schemeApprovedOrganisationId2) &&
                                                         s.SchemeStatus.Equals(SchemeStatus.Approved)))).Returns(organisationSchemeDataForScheme2);

            A.CallTo(() => producerBalancingSchemeOrganisationMap.Map(A<ProducerBalancingScheme>.That.Matches(s => s.Organisation.Name.Equals(pbsOrganisationName) &&
                                                                                                       s.Organisation.Id.Equals(pbsOrganisationId)))).Returns(organisationSchemeDataForPBS);
            // act
            var result = await handler.HandleAsync(GetRequest(true));

            // assert
            result[0].DisplayName.Should().Be(schemeApprovedName1);
            result[0].Id.Should().Be(schemeApprovedOrganisationId1);
            result[2].DisplayName.Should().Be(schemeApprovedName2);
            result[2].Id.Should().Be(schemeApprovedOrganisationId2);
            result[1].DisplayName.Should().Be(pbsOrganisationName);
            result[1].Id.Should().Be(pbsOrganisationId);
        }

        private GetOrganisationScheme GetRequest(bool includePbs)
        {
            return new GetOrganisationScheme(includePbs);
        }

        private OrganisationSchemeData GetOrganisationSchemeData(string displayName, Guid organisationId)
        {
            return new OrganisationSchemeData
            {
                 DisplayName = displayName,
                 Id = organisationId
            };
        }
    }
}