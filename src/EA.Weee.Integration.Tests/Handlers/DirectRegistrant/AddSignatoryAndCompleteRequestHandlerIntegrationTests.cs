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

            private readonly It shouldSetSubmissionStatusAndDate = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                submission.CurrentSubmission.SubmittedDate.Should().NotBeNull();
                submission.DirectProducerSubmissionStatus.Should().Be(DirectProducerSubmissionStatus.Complete);
            };

            private readonly It shouldSetAppropriateSignatoryFromRequest = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                submission.CurrentSubmission.Contact.FirstName.Should().Be(request.ContactData.FirstName);
                submission.CurrentSubmission.Contact.LastName.Should().Be(request.ContactData.LastName);
                submission.CurrentSubmission.Contact.Position.Should().Be(request.ContactData.Position);
            };

            private readonly It shouldNotOverwriteRootOrganisationDetailsFromSubmissionHistory = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                // Root organisation must remain unchanged — not synced from current year submission
                submission.DirectRegistrant.Organisation.Name.Should()
                    .NotBe(submission.CurrentSubmission.CompanyName,
                        "root Organisation.Name must not be overwritten with per-year submission data");
                submission.DirectRegistrant.Organisation.TradingName.Should()
                    .NotBe(submission.CurrentSubmission.TradingName,
                        "root Organisation.TradingName must not be overwritten with per-year submission data");
            };

            private readonly It shouldNotOverwriteRootBrandNameFromSubmissionHistory = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                // Root BrandName entity must NOT be the same DB row as history BrandName.
                // If they shared the same entity, Year 2 completion would corrupt Year 1's BrandName via OverwriteWhereNull.
                if (submission.DirectRegistrant.BrandNameId.HasValue && submission.CurrentSubmission.BrandNameId.HasValue)
                {
                    submission.DirectRegistrant.BrandNameId.Value.Should()
                        .NotBe(submission.CurrentSubmission.BrandNameId.Value,
                            "root DirectRegistrant.BrandName must not share the same entity as submission history BrandName");
                }
            };

            private readonly It shouldNotOverwriteRootContactFromSubmissionHistory = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(directProducerSubmission.Id);

                // Root contact entity must remain the original registration contact, not replaced by submission history.
                submission.DirectRegistrant.ContactId.Should().HaveValue();
                submission.CurrentSubmission.ContactId.Should().HaveValue();

                submission.DirectRegistrant.ContactId.Value.Should()
                    .NotBe(submission.CurrentSubmission.ContactId.Value,
                        "root DirectRegistrant.Contact must not be overwritten with per-year submission contact");
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
                    .WithBusinessAddress(address)
                    .WithContactAddress(address)
                    .WithContact(contact)
                    .WithBrandName("new brand")
                    .WithAuthorisedRep(authedRepSubmission)
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
