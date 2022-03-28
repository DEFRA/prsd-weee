namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Aatf;
    using Requests.AatfEvidence;
    using Protocol = Core.AatfEvidence.Protocol;
    using WasteType = Core.AatfEvidence.WasteType;

    public class CreateEvidenceNoteRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenICreateADraftEvidenceNote : CreateEvidenceNoteHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                var builder = LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                aatf = AatfDbSetup.Init().WithOrganisation(organisation).Create();
                scheme = SchemeDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();

                var categories = new List<TonnageValues>()
                {
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.AutomaticDispensers, 2, 1),
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.ConsumerEquipment, null, null),
                    new TonnageValues(Guid.Empty, (int)WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };

                request = new CreateEvidenceNoteRequest(organisation.Id,
                    aatf.Id,
                    scheme.Id,
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    fixture.Create<WasteType>(),
                    fixture.Create<Protocol>(),
                    categories.ToList());
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteByReference(result);
            };

            private readonly It shouldHaveCreateEvidenceNote = () =>
            {
                note.Should().NotBeNull();
            };

            private readonly It shouldHaveCreatedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
            };
        }

        public class CreateEvidenceNoteHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<CreateEvidenceNoteRequest, int> handler;
            protected static Organisation organisation;
            protected static Aatf aatf;
            protected static CreateEvidenceNoteRequest request;
            protected static Scheme scheme;
            protected static int result;
            protected static Note note;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<CreateEvidenceNoteRequest, int>>();

                return setup;
            }
        }
    }
}
