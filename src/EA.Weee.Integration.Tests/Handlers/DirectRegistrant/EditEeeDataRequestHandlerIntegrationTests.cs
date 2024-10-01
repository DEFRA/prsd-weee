namespace EA.Weee.Integration.Tests.Handlers.DirectRegistrant
{
    using Autofac;
    using AutoFixture;
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Integration.Tests.Builders;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    public class EditEeeDataRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateEeeData : EditEeeDataRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();
                
                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                eeeData = new List<Eee>
                {
                    new Eee(10m, WeeeCategory.AutomaticDispensers, ObligationType.B2B),
                    new Eee(10m, WeeeCategory.DisplayEquipment, ObligationType.B2C)
                };

                request = new EditEeeDataRequest(directRegistrant.Id, eeeData, fixture.Create<SellingTechniqueType>());
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var submission = Query.CurrentSubmissionHistoryForComplianceYear(directRegistrant.Id, SystemTime.Now.Year);

                submission.SellingTechniqueType.ToInt().Should().Be(request.SellingTechniqueType.ToInt());
                submission.EeeOutputReturnVersion.Should().NotBeNull();
                eeeData.Should().NotBeEmpty();
                submission.EeeOutputReturnVersion.EeeOutputAmounts.Count.Should().Be(eeeData.Count);

                foreach (var eee in eeeData)
                {
                    var outputAmount = submission.EeeOutputReturnVersion.EeeOutputAmounts
                        .FirstOrDefault(e => e.WeeeCategory.ToInt() == eee.Category.ToInt());

                    outputAmount.Should().NotBeNull();
                    outputAmount.ObligationType.ToInt().Should().Be(eee.ObligationType.ToInt());
                    outputAmount.Tonnage.Should().Be(eee.Tonnage);
                    outputAmount.RegisteredProducer.Should().BeEquivalentTo(submission.DirectProducerSubmission.RegisteredProducer);
                }
            };
        }

        [Component]
        public class WhenIUpdateExistingEeeData : EditEeeDataRequestHandlerIntegrationTestBase
        {
            private static EeeOutputReturnVersion outputReturnVersion;

            private readonly Establish context = () =>
            {
                LocalSetup();

                outputReturnVersion = new EeeOutputReturnVersion();

                outputReturnVersion.AddEeeOutputAmount(
                    EeeOutputAmountDbSetup.Init().WithData(directProducerSubmission.RegisteredProducerId, Domain.Lookup.WeeeCategory.AutomaticDispensers, Domain.Obligation.ObligationType.B2C, 10m).Create());
                outputReturnVersion.AddEeeOutputAmount(
                    EeeOutputAmountDbSetup.Init().WithData(directProducerSubmission.RegisteredProducerId, Domain.Lookup.WeeeCategory.AutomaticDispensers, Domain.Obligation.ObligationType.B2B, 5m).Create());

                var directProducerSubmissionHistory = DirectRegistrantSubmissionHistoryDbSetup.Init()
                    .WithSellingTechnique(Domain.Producer.Classfication.SellingTechniqueType.Both)
                    .WithEeeData(outputReturnVersion)
                    .WithDirectProducerSubmission(directProducerSubmission).Create();

                Query.UpdateCurrentProducerSubmission(directProducerSubmission.Id, directProducerSubmissionHistory.Id);
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, directRegistrant.OrganisationId).Create();

                eeeData = new List<Eee>
                {
                    new Eee(10m, WeeeCategory.AutomaticDispensers, ObligationType.B2B),
                    new Eee(20m, WeeeCategory.DisplayEquipment, ObligationType.B2C)
                };

                request = new EditEeeDataRequest(directRegistrant.Id, eeeData, fixture.Create<SellingTechniqueType>());
            };

            private readonly Because of = () =>
            {
                result = AsyncHelper.RunSync(() => handler.HandleAsync(request));
            };

            private readonly It shouldUpdateTheData = () =>
            {
                var submission = Query.CurrentSubmissionHistoryForComplianceYear(directRegistrant.Id, SystemTime.Now.Year);

                submission.SellingTechniqueType.ToInt().Should().Be(request.SellingTechniqueType.ToInt());
                submission.EeeOutputReturnVersion.Should().NotBeNull();
                eeeData.Should().NotBeEmpty();
                submission.EeeOutputReturnVersion.EeeOutputAmounts.Count.Should().Be(eeeData.Count);

                foreach (var eee in eeeData)
                {
                    var outputAmount = submission.EeeOutputReturnVersion.EeeOutputAmounts
                        .FirstOrDefault(e => e.WeeeCategory.ToInt() == eee.Category.ToInt());

                    outputAmount.Should().NotBeNull();
                    outputAmount.ObligationType.ToInt().Should().Be(eee.ObligationType.ToInt());
                    outputAmount.Tonnage.Should().Be(eee.Tonnage);
                    outputAmount.RegisteredProducer.Should().BeEquivalentTo(submission.DirectProducerSubmission.RegisteredProducer);
                }
            };
        }

        [Component]
        public class WhenUserIsNotAuthorised : EditEeeDataRequestHandlerIntegrationTestBase
        {
            protected static IRequestHandler<EditEeeDataRequest, bool> authHandler;

            private readonly Establish context = () =>
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings();

                authHandler = Container.Resolve<IRequestHandler<EditEeeDataRequest, bool>>();
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => authHandler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtSecurityException = ShouldThrowException<SecurityException>;
        }

        public class EditEeeDataRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<EditEeeDataRequest, bool> handler;
            protected static Fixture fixture;
            protected static Domain.Producer.DirectRegistrant directRegistrant;
            protected static Domain.Producer.DirectProducerSubmission directProducerSubmission;
            protected static EditEeeDataRequest request;
            protected static bool result;
            protected static List<Eee> eeeData;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                directRegistrant = DirectRegistrantDbSetup.Init()
                    .Create();

                directProducerSubmission = DirectRegistrantSubmissionDbSetup.Init()
                    .WithDefaultRegisteredProducer()
                    .WithComplianceYear(SystemTime.UtcNow.Year)
                    .WithDirectRegistrant(directRegistrant)
                    .Create();

                handler = Container.Resolve<IRequestHandler<EditEeeDataRequest, bool>>();

                fixture = new Fixture();

                return setup;
            }
        }
    }
}