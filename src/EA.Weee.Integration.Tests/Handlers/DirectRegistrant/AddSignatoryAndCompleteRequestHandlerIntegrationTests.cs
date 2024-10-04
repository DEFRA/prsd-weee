namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Linq;
    using System.Security;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;

    public class AddSignatoryAndCompleteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateOrganisationDetails : AddSignatoryAndCompleteRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                submission.CurrentSubmission.Contact.FirstName.Should().Be(request.ContactData.FirstName);
                submission.CurrentSubmission.Contact.LastName.Should().Be(request.ContactData.LastName);
                submission.CurrentSubmission.Contact.Position.Should().Be(request.ContactData.Position);

                submission.DirectRegistrant.Organisation.BusinessAddress.Should().Be(submission.CurrentSubmission.BusinessAddress);
                submission.DirectRegistrant.Organisation.Name.Should().Be(submission.CurrentSubmission.CompanyName);
                submission.DirectRegistrant.Organisation.TradingName.Should().Be(submission.CurrentSubmission.TradingName);

                submission.DirectRegistrant.Contact.Should().Be(submission.CurrentSubmission.Contact);
                submission.DirectRegistrant.Address.Should().Be(submission.CurrentSubmission.ContactAddress);
                submission.DirectRegistrant.BrandName.Should().Be(submission.CurrentSubmission.BrandName);
                submission.DirectRegistrant.AuthorisedRepresentative.Should().Be(submission.CurrentSubmission.AuthorisedRepresentative);

                submission.CurrentSubmission.SubmittedDate.Should().NotBeNull();
                submission.DirectProducerSubmissionStatus.Should().Be(DirectProducerSubmissionStatus.Complete);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : AddSignatoryAndCompleteRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<AddSignatoryAndCompleteRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<AddSignatoryAndCompleteRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class AddSignatoryAndCompleteRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<AddSignatoryAndCompleteRequest, bool> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static AddSignatoryAndCompleteRequest request;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static bool result;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                .WithExternalUserAccess();

                var organisation = OrganisationDbSetup.Init().Create();
                var address = AddressDbSetup.Init().Create();
                var contact = ContactDbSetup.Init().Create();

                var authedRep = AuthorisedRepDbSetup.Init().Create();
                directRegistrant = DirectRegistrantDbSetup.Init()
                    .WithAddress(address.Id)
                    .WithContact(contact.Id)
                    .WithBrandName("another brand")
                    .WithAuthorisedRep(authedRep)
                    .WithOrganisation(organisation.Id)
                    .Create();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDefaultRegisteredProducer()
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithDirectRegistrant(directRegistrant)
                    .Create();

                var authedRepSubmission = AuthorisedRepDbSetup.Init().Create();

                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithBusinessAddress(address.Id)
                    .WithContactAddress(address.Id)
                    .WithContact(contact.Id)
                    .WithBrandName("new brand")
                    .WithAuthorisedRep(authedRepSubmission.Id)
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                handler = Container.Resolve<IRequestHandler<AddSignatoryAndCompleteRequest, bool>>();

                fixture = new Fixture();

                var contactData = fixture.Build<ContactData>()
                    .With(a => a.FirstName, contact.FirstName)
                    .With(a => a.LastName, contact.LastName)
                    .With(a => a.Position, contact.Position).Create();

                request = new AddSignatoryAndCompleteRequest(directRegistrant.Id, contactData);

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                return setup;
            }
        }
    }
}
