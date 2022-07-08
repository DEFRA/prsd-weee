namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;

    public class GetEvidenceNoteForInternalUserRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetOneSubmittedEvidenceNoteAsAppropriateAuthority : GetEvidenceNoteForInternalUserRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null)
                };

                note = EvidenceNoteDbSetup
                        .Init()
                        .WithTonnages(categories)
                        .With(n =>
                        {
                            n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        }).Create();

                request = new GetEvidenceNoteForInternalUserRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                data = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheEvidenceNote = () =>
            {
                data.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                data.Status.Should().Be(NoteStatus.Submitted);
            };
        }

        [Component]
        public class WhenIGetOneApprovedEvidenceNoteAsAppropriateAuthority : GetEvidenceNoteForInternalUserRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null)
                };

                note = EvidenceNoteDbSetup
                .Init()
                .WithTonnages(categories)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow);
                })
                .Create();

                request = new GetEvidenceNoteForInternalUserRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                data = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheEvidenceNote = () =>
            {
                data.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                data.Status.Should().Be(NoteStatus.Approved);
            };
        }

        [Component]
        public class WhenIGetOneRejectedEvidenceNoteAsAppropriateAuthority : GetEvidenceNoteForInternalUserRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.LightingEquipment, 10, 5)
                };

                note = EvidenceNoteDbSetup
                .Init()
                .WithTonnages(categories)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow.AddHours(-1));
                    n.UpdateStatus(NoteStatusDomain.Approved, UserId.ToString(), SystemTime.UtcNow.AddMinutes(-1));
                    n.UpdateStatus(NoteStatusDomain.Rejected, UserId.ToString(), SystemTime.UtcNow);
                })
                .Create();

                request = new GetEvidenceNoteForInternalUserRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                data = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheEvidenceNote = () =>
            {
                data.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                data.Status.Should().Be(NoteStatus.Rejected);
            };
        }

        [Component]
        public class WhenIGetOneReturnedEvidenceNoteAsAppropriateAuthority : GetEvidenceNoteForInternalUserRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 0, 1),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 5, null)
                };

                note = EvidenceNoteDbSetup
                .Init()
                .WithTonnages(categories)
                .With(n =>
                {
                    n.UpdateStatus(NoteStatusDomain.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    n.UpdateStatus(NoteStatusDomain.Returned, UserId.ToString(), SystemTime.UtcNow.AddHours(1));
                })
                .Create();

                request = new GetEvidenceNoteForInternalUserRequest(note.Id);
            };

            private readonly Because of = () =>
            {
                data = Task.Run(async () => await handler.HandleAsync(request)).Result;

                note = Query.GetEvidenceNoteById(note.Id);
            };

            private readonly It shouldHaveReturnedTheEvidenceNote = () =>
            {
                data.Should().NotBeNull();
            };

            private readonly It shouldHaveReturnedTheEvidenceNoteWithExpectedPropertyValues = () =>
            {
                ShouldMapToNote();
                data.Status.Should().Be(NoteStatus.Returned);
            };
        }

        public class GetEvidenceNoteForInternalUserRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetEvidenceNoteForInternalUserRequest, EvidenceNoteData> handler;
            protected static GetEvidenceNoteForInternalUserRequest request;
            protected static EvidenceNoteData data;
            protected static Note note;
            protected static Fixture fixture;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetDb: true)
                    .WithInternalUserAccess(false);

                var authority = Query.GetEaCompetentAuthority();
                var role = Query.GetInternalUserRole();

                if (!Query.CompetentAuthorityUserExists(UserId.ToString()))
                {
                    CompetentAuthorityUserDbSetup.Init().WithUserIdAndAuthorityAndRole(UserId.ToString(), authority.Id, role.Id)
                        .Create();
                }

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetEvidenceNoteForInternalUserRequest, EvidenceNoteData>>();
            }

            protected static void ShouldMapToNote()
            {
                data.EndDate.Should().Be(note.EndDate);
                data.StartDate.Should().Be(note.StartDate);
                data.Reference.Should().Be(note.Reference);
                data.Protocol.ToInt().Should().Be(note.Protocol.ToInt());
                data.WasteType.ToInt().Should().Be(note.WasteType.ToInt());
                data.AatfData.Should().NotBeNull();
                data.AatfData.Id.Should().Be(note.Aatf.Id);
                data.RecipientSchemeData.Should().NotBeNull();
                data.RecipientSchemeData.Id.Should().Be(note.Recipient.Id);
                data.EvidenceTonnageData.Count.Should().Be(note.NoteTonnage.Count);
                data.OrganisationData.Should().NotBeNull();
                data.OrganisationData.Id.Should().Be(note.Organisation.Id);
                ((int)data.Type).Should().Be(note.NoteType.Value);
                data.RecipientId.Should().Be(note.Recipient.Id);
                data.Id.Should().Be(note.Id);
                data.ComplianceYear.Should().Be(note.ComplianceYear);
                foreach (var noteTonnage in note.NoteTonnage)
                {
                    data.EvidenceTonnageData.Should().Contain(n => n.Received.Equals(noteTonnage.Received) &&
                                                             n.Reused.Equals(noteTonnage.Reused) &&
                                                             ((int)n.CategoryId).Equals((int)noteTonnage.CategoryId));
                }
                data.RecipientOrganisationData.Should().NotBeNull();
                data.RecipientOrganisationData.Id.Should().Be(note.Recipient.OrganisationId);
            }
        }
    }
}
