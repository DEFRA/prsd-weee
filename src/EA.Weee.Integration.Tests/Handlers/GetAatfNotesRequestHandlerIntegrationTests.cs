namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Builders;
    using Core.Helpers;
    using Core.Scheme;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Requests.AatfEvidence;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    public class GetAatfNotesRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAatfNotesdWithStatusDraft : GetAatfNotesRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                aatf = AatfDbSetup.Init().WithOrganisation(organisation).Create();
                scheme = SchemeDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
            };

            private readonly Because of = () =>
            {
                evidenceNoteData = Task.Run(async () => await handler.HandleAsync(new GetAatfNotesRequest(organisation.Id, aatf.Id))).Result;
            };
        }
        
        public class GetAatfNotesRequestHandlerTestBase : WeeeContextSpecification
        {
            protected static List<EvidenceNoteData> evidenceNoteData;
            protected static IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>> handler;
            protected static Organisation organisation;
            protected static Scheme scheme;
            protected static Aatf aatf;
            protected static Note note;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetDb: true)
                    .WithExternalUserAccess();

                handler = Container.Resolve<IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>>>();
            }
        }
    }
}
