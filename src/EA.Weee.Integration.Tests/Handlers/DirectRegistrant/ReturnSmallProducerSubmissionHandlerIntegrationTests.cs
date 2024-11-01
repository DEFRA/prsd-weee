namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System;
    using System.Security;

    public class ReturnSmallProducerSubmissionHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIReturnASubmissionThatHasNoData : ReturnSmallProducerSubmissionHandlerIntegrationTestBase
        {
            private static DirectProducerSubmission directProducerSubmission;
            private static DirectProducerSubmissionHistory directProducerSubmissionHistory;

            private readonly Establish context = () =>
            {
                LocalSetup();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDirectRegistrant(directRegistrant)
                    .WithDefaultRegisteredProducer()
                    .WithStatus(DirectProducerSubmissionStatus.Complete)
                    .Create();

                directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission)
                    .WithSellingTechnique(SellingTechniqueType.DirectSellingtoEndUser)
                    .Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                request = new ReturnSmallProducerSubmission(directProducerSubmission.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldReturnTheSubmission = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(result);

                submission.Should().NotBeNull();
                submission.Id.Should().Be(directProducerSubmission.Id);
                submission.DirectProducerSubmissionStatus.Should().Be(DirectProducerSubmissionStatus.Returned);
                submission.CurrentSubmission.BrandName.Should().BeNull();
                submission.CurrentSubmission.AppropriateSignatory.Should().BeNull();
                submission.CurrentSubmission.AuthorisedRepresentative.Should().BeNull();
                submission.CurrentSubmission.BusinessAddress.Should().BeNull();
                submission.CurrentSubmission.Contact.Should().BeNull();
                submission.CurrentSubmission.ContactAddress.Should().BeNull();
                submission.CurrentSubmission.EeeOutputReturnVersion.Should().BeNull();
                submission.CurrentSubmission.SellingTechniqueType.Should()
                    .Be(SellingTechniqueType.DirectSellingtoEndUser.Value);
                submission.CurrentSubmission.ServiceOfNoticeAddress.Should().BeNull();
                submission.CurrentSubmission.SubmittedDate.Should().BeNull();
                submission.CurrentSubmission.CompanyRegistrationNumber.Should()
                    .Be(directProducerSubmissionHistory.CompanyRegistrationNumber);
                submission.CurrentSubmission.TradingName.Should()
                    .Be(directProducerSubmissionHistory.TradingName);
                submission.CurrentSubmission.CompanyName.Should()
                    .Be(directProducerSubmissionHistory.CompanyName);
            };
        }

        [Component]
        public class WhenIReturnASubmissionThatHasAllData : ReturnSmallProducerSubmissionHandlerIntegrationTestBase
        {
            private static DirectProducerSubmission directProducerSubmission;
            private static DirectProducerSubmissionHistory directProducerSubmissionHistory;

            private readonly Establish context = () =>
            {
                LocalSetup();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDirectRegistrant(directRegistrant)
                    .WithDefaultRegisteredProducer()
                    .WithStatus(DirectProducerSubmissionStatus.Complete)
                    .Create();

                var appropriateSignatory = ContactDbSetup.Init().Create();
                var authorisedRep = AuthorisedRepDbSetup.Init().Create();
                var contact = ContactDbSetup.Init().Create();
                var contactAddress = AddressDbSetup.Init()
                    .WithCountry("UK - England")
                    .Create();
                var businessAddress = AddressDbSetup.Init().Create();
                var eeeVersion = new EeeOutputReturnVersion();
                eeeVersion.AddEeeOutputAmount(
                    EeeOutputAmountDbSetup.Init().WithData(directProducerSubmission.RegisteredProducerId, Domain.Lookup.WeeeCategory.AutomaticDispensers, Domain.Obligation.ObligationType.B2C, 10m).Create());
                eeeVersion.AddEeeOutputAmount(
                    EeeOutputAmountDbSetup.Init().WithData(directProducerSubmission.RegisteredProducerId, Domain.Lookup.WeeeCategory.AutomaticDispensers, Domain.Obligation.ObligationType.B2B, 5m).Create());

                directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithBusinessAddress(businessAddress)
                    .WithDirectProducerSubmission(directProducerSubmission)
                    .WithSellingTechnique(SellingTechniqueType.DirectSellingtoEndUser)
                    .WithBrandName(Faker.Name.First())
                    .WithAppropriateSignatory(appropriateSignatory)
                    .WithAuthorisedRep(authorisedRep)
                    .WithContact(contact)
                    .WithContactAddress(contactAddress)
                    .WithEeeData(eeeVersion)
                    .Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);

                request = new ReturnSmallProducerSubmission(directProducerSubmission.Id);
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldReturnTheSubmission = () =>
            {
                var submission = Query.GetDirectProducerSubmissionById(result);

                submission.Should().NotBeNull();
                submission.Id.Should().Be(directProducerSubmission.Id);
                submission.DirectProducerSubmissionStatus.Should().Be(DirectProducerSubmissionStatus.Returned);
                submission.CurrentSubmission.BrandName.Name.Should().Be(directProducerSubmissionHistory.BrandName.Name);
                submission.CurrentSubmission.AppropriateSignatory.Should().NotBeNull();
                submission.CurrentSubmission.AppropriateSignatory.Should()
                    .BeEquivalentTo(directProducerSubmissionHistory.AppropriateSignatory);
                submission.CurrentSubmission.AuthorisedRepresentative.Should().NotBeNull();
                submission.CurrentSubmission.AuthorisedRepresentative.Should()
                    .BeEquivalentTo(directProducerSubmissionHistory.AuthorisedRepresentative);
                submission.CurrentSubmission.BusinessAddress.Should().NotBeNull();
                submission.CurrentSubmission.BusinessAddress.Should()
                    .BeEquivalentTo(directProducerSubmissionHistory.BusinessAddress);
                submission.CurrentSubmission.Contact.Should().NotBeNull();
                submission.CurrentSubmission.Contact.Should().BeEquivalentTo(directProducerSubmissionHistory.Contact);
                submission.CurrentSubmission.ContactAddress.Should().NotBeNull();
                submission.CurrentSubmission.ContactAddress.Should()
                    .BeEquivalentTo(directProducerSubmissionHistory.ContactAddress);
                submission.CurrentSubmission.EeeOutputReturnVersion.Should().NotBeNull();
                submission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts
                    .Should().Contain(x =>
                        x.RegisteredProducerId == directProducerSubmission.RegisteredProducerId &&
                        x.WeeeCategory == WeeeCategory.AutomaticDispensers &&
                        x.ObligationType == Domain.Obligation.ObligationType.B2C &&
                        x.Tonnage == 10m);
                submission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts
                    .Should().Contain(x =>
                        x.RegisteredProducerId == directProducerSubmission.RegisteredProducerId &&
                        x.WeeeCategory == WeeeCategory.AutomaticDispensers &&
                        x.ObligationType == Domain.Obligation.ObligationType.B2B &&
                        x.Tonnage == 5m);
                submission.CurrentSubmission.SellingTechniqueType.Should()
                    .Be(SellingTechniqueType.DirectSellingtoEndUser.Value);
                submission.CurrentSubmission.ServiceOfNoticeAddress.Should().BeNull();
                submission.CurrentSubmission.SubmittedDate.Should().BeNull();
                submission.CurrentSubmission.CompanyRegistrationNumber.Should()
                    .Be(directProducerSubmissionHistory.CompanyRegistrationNumber);
                submission.CurrentSubmission.TradingName.Should()
                    .Be(directProducerSubmissionHistory.TradingName);
                submission.CurrentSubmission.CompanyName.Should()
                    .Be(directProducerSubmissionHistory.CompanyName);
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : ReturnSmallProducerSubmissionHandlerIntegrationTestBase
        {
            protected static IRequestHandler<ReturnSmallProducerSubmission, Guid> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithExternalUserAccess();

                authHandler = Container.Resolve<IRequestHandler<ReturnSmallProducerSubmission, Guid>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class ReturnSmallProducerSubmissionHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<ReturnSmallProducerSubmission, Guid> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static ReturnSmallProducerSubmission request;
            protected static Guid result;
            protected static Country country;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithInternalUserAccess(true);

                Query.SetupUserWithRole(UserId.ToString(), "Standard", CompetentAuthority.England);

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .Create();

                handler = Container.Resolve<IRequestHandler<ReturnSmallProducerSubmission, Guid>>();

                fixture = new Fixture();

                country = AsyncHelper.RunSync(() => Query.GetCountryByNameAsync("UK - England"));

                return setup;
            }
        }
    }
}
