﻿namespace EA.Weee.Integration.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using EA.Weee.Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Requests.Note;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class SetNoteStatusRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIUpdateANoteStatusToApproved : SetNoteStatusRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                scheme = SchemeDbSetup.Init().WithOrganisation(organisation.Id).Create();

                note = EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme.Id)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString());
                    })
                    .Create();

                request = new SetNoteStatus(note.Id, Core.AatfEvidence.NoteStatus.Approved);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveUpdatedTheEvidenceNoteWithStatusApproved = () =>
            {
                note.Status.Should().Be(NoteStatus.Approved);
            };

            private readonly It shouldHaveCreatedANoteStatusHistory = () =>
            {
                var history = Query.GetLatestNoteStatusHistoryForNote(note.Id);
                history.ChangedById.Should().Be(UserId.ToString());
                history.ChangedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                history.FromStatus.Should().Be(NoteStatus.Submitted);
                history.ToStatus.Should().Be(NoteStatus.Approved);
            };
        }

        public class SetNoteStatusRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<SetNoteStatus, Guid> handler;
            protected static Organisation organisation;
            protected static SetNoteStatus request;
            protected static Guid result;
            protected static Scheme scheme;  // added by Guido
            protected static Note note;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<SetNoteStatus, Guid>>();

                return setup;
            }
        }
    }
}
