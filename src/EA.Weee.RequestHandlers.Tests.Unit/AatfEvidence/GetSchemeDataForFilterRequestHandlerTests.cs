﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
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
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Domain.Scheme;
    using Xunit;

    public class GetSchemeDataForFilterRequestHandlerTests : SimpleUnitTestBase
    {
        private GetSchemeDataForFilterRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly GetSchemeDataForFilterRequest request;

        public GetSchemeDataForFilterRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            noteDataAccess = A.Fake<IEvidenceDataAccess>();

            request = new GetSchemeDataForFilterRequest(TestFixture.Create<RecipientOrTransfer>(), TestFixture.Create<Guid?>(),
                TestFixture.Create<int>(), TestFixture.CreateMany<NoteStatus>().ToList(), TestFixture.CreateMany<NoteType>().ToList());

            handler = new GetSchemeDataForFilterRequestHandler(weeeAuthorization, noteDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetSchemeDataForFilterRequestHandler(authorization, noteDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyAatfAccess().Build();
            handler = new GetSchemeDataForFilterRequestHandler(authorization, noteDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithOrganisationId_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureAatfHasOrganisationAccess(request.AatfId.Value))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithNoOrganisationId_ShouldCheckInternalAccess()
        {
            //arrange
            var request = new GetSchemeDataForFilterRequest(TestFixture.Create<RecipientOrTransfer>(), 
                null, 
                TestFixture.Create<int>(),
                TestFixture.CreateMany<NoteStatus>().ToList(),
                TestFixture.CreateMany<NoteType>().ToList());

            //act

            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithOrganisationId_ShouldCheckExternalAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithRecipient_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var request = new GetSchemeDataForFilterRequest(RecipientOrTransfer.Recipient, TestFixture.Create<Guid?>(),
                TestFixture.Create<int>(),
                TestFixture.CreateMany<NoteStatus>().ToList(),
                TestFixture.CreateMany<NoteType>().ToList());

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => noteDataAccess.GetRecipientOrganisations(request.AatfId,
                request.ComplianceYear, 
                A<List<Domain.Evidence.NoteStatus>>.That.IsSameSequenceAs(
                    request.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList()),
                    A<List<Domain.Evidence.NoteType>>.That.IsSameSequenceAs(
                        request.AllowedNoteTypes.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteType>()).ToList()))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithTransfer_EvidenceDataAccessShouldBeCalledOnce()
        {
            //arrange
            var request = new GetSchemeDataForFilterRequest(RecipientOrTransfer.Transfer, 
                TestFixture.Create<Guid?>(),
                TestFixture.Create<int>(),
                TestFixture.CreateMany<NoteStatus>().ToList(),
                TestFixture.CreateMany<NoteType>().ToList());

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => noteDataAccess.GetTransferOrganisations(request.ComplianceYear,
                A<List<Domain.Evidence.NoteStatus>>.That.IsSameSequenceAs(
                    request.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList()),
                A<List<Domain.Evidence.NoteType>>.That.IsSameSequenceAs(
                    request.AllowedNoteTypes.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteType>()).ToList()))).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => noteDataAccess.GetRecipientOrganisations(A<Guid>._, 
                    A<int>._, 
                    A<List<Domain.Evidence.NoteStatus>>._,
                    A<List<Domain.Evidence.NoteType>>._))
                .Returns(new List<Organisation>() { organisation1WithScheme, organisationPbs, organisation2WithScheme });

            var request = new GetSchemeDataForFilterRequest(RecipientOrTransfer.Recipient, 
                TestFixture.Create<Guid?>(),
                TestFixture.Create<int>(), 
                TestFixture.CreateMany<NoteStatus>().ToList(),
                TestFixture.CreateMany<NoteType>().ToList());

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

        [Fact]
        public async Task HandleAsync_GivenTransferOrganisations_OrganisationDataShouldBeReturned()
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

            A.CallTo(() => noteDataAccess.GetTransferOrganisations(A<int>._, 
                    A<List<Domain.Evidence.NoteStatus>>._,
                    A<List<Domain.Evidence.NoteType>>._))
                .Returns(new List<Organisation>() { organisation1WithScheme, organisationPbs, organisation2WithScheme });

            var request = new GetSchemeDataForFilterRequest(RecipientOrTransfer.Transfer, 
                TestFixture.Create<Guid?>(),
                TestFixture.Create<int>(), 
                TestFixture.CreateMany<NoteStatus>().ToList(),
                TestFixture.CreateMany<NoteType>().ToList());

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
