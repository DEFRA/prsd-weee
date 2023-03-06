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
    using Core.AatfEvidence;
    using Domain.Scheme;
    using EA.Prsd.Core;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Organisation;
    using FluentAssertions;
    using NUnit.Specifications;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;
    using UserStatus = Domain.User.UserStatus;

    public class GetSchemeDataForFilterRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetSchemesForLookupForAnAatf : GetSchemeDataForFilterRequestHandlerIntegrationTestBase
        {
            private static Aatf aatf;
            private static Scheme scheme1;
            private static Scheme scheme2;
            private static Scheme schemeNotIncludedNotLinkedToRequestedAatf;
            private static Scheme schemeNotIncludedAsHasDraftNote;
            private static Scheme schemeNotIncludedAsHasNoteInIncorrectYear;

            private readonly Establish context = () =>
            {
                LocalExternalSetup();

                var organisation = OrganisationDbSetup.Init().Create();
                aatf = AatfDbSetup.Init().WithComplianceYear(ComplianceYear).WithOrganisation(organisation.Id).Create();

                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id)
                    .WithStatus(UserStatus.Active)
                    .Create();

                scheme1 = SchemeDbSetup.Init().WithNewOrganisation().Create();
                scheme2 = SchemeDbSetup.Init().WithNewOrganisation().Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .WithAatf(aatf.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Returned, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .WithAatf(aatf.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .WithAatf(aatf.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme2.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Void, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .WithAatf(aatf.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme2.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Rejected, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .WithAatf(aatf.Id)
                    .Create();

                schemeNotIncludedNotLinkedToRequestedAatf = SchemeDbSetup.Init().WithNewOrganisation().Create();
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(schemeNotIncludedNotLinkedToRequestedAatf.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                schemeNotIncludedAsHasDraftNote = SchemeDbSetup.Init().WithNewOrganisation().Create();
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(schemeNotIncludedAsHasDraftNote.OrganisationId)
                    .WithComplianceYear(ComplianceYear)
                    .WithAatf(aatf.Id)
                    .Create();

                schemeNotIncludedAsHasNoteInIncorrectYear = SchemeDbSetup.Init().WithNewOrganisation().Create();
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(schemeNotIncludedAsHasNoteInIncorrectYear.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear + 1)
                    .WithAatf(aatf.Id)
                    .Create();

                var allowedStatus = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected };
                request = new GetSchemeDataForFilterRequest(RecipientOrTransfer.Recipient, aatf.Id, ComplianceYear, allowedStatus, new List<NoteType>()
                {
                    NoteType.Evidence
                });
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEntityIdDisplayNameData = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCount = () =>
            {
                result.Should().HaveCount(2);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                var schemeData = result.FirstOrDefault(e => e.Id == scheme1.OrganisationId);
                schemeData.Should().NotBeNull();
                schemeData.Id.Should().Be(scheme1.OrganisationId);
                schemeData.DisplayName.Should().Be(scheme1.SchemeName);

                schemeData = result.FirstOrDefault(e => e.Id == scheme2.OrganisationId);
                schemeData.Should().NotBeNull();
                schemeData.Id.Should().Be(scheme2.OrganisationId);
                schemeData.DisplayName.Should().Be(scheme2.SchemeName);
            };

            private readonly It shouldHaveExcludedExpectedData = () =>
            {
                result.FirstOrDefault(e => e.Id == schemeNotIncludedNotLinkedToRequestedAatf.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasDraftNote.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasNoteInIncorrectYear.OrganisationId)
                    .Should().BeNull();
            };

            private readonly It shouldHaveDistinctItems = () =>
            {
                result.Should().OnlyHaveUniqueItems();
            };

            private readonly It shouldBeOrderedCorrectly = () =>
            {
                result.Should().BeInAscendingOrder(e => e.DisplayName);
            };
        }

        [Component]
        public class WhenIGetTransferSchemesForLookupForAnInternalUser : GetSchemeDataForFilterRequestHandlerIntegrationTestBase
        {
            private static Scheme scheme1;
            private static Scheme scheme2;
            private static Scheme recipientScheme;
            private static Scheme schemeNotIncludedAsHasDraftNote;
            private static Scheme schemeNotIncludedAsHasNoteInIncorrectYear;
            private static Scheme schemeNotIncludedAsHasEvidenceNote;

            private readonly Establish context = () =>
            {
                LocalInternalSetup();

                scheme1 = SchemeDbSetup.Init().WithNewOrganisation().Create();
                scheme2 = SchemeDbSetup.Init().WithNewOrganisation().Create();
                recipientScheme = SchemeDbSetup.Init().WithNewOrganisation().Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientScheme.OrganisationId)
                    .WithOrganisation(scheme1.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientScheme.OrganisationId)
                    .WithOrganisation(scheme1.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Returned, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientScheme.OrganisationId)
                    .WithOrganisation(scheme1.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientScheme.OrganisationId)
                    .WithOrganisation(scheme2.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Void, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientScheme.OrganisationId)
                    .WithOrganisation(scheme2.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Rejected, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                schemeNotIncludedAsHasDraftNote = SchemeDbSetup.Init().WithNewOrganisation().Create();
                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientScheme.OrganisationId)
                    .WithOrganisation(schemeNotIncludedAsHasDraftNote.OrganisationId)
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                schemeNotIncludedAsHasEvidenceNote = SchemeDbSetup.Init().WithNewOrganisation().Create();
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientScheme.OrganisationId)
                    .WithOrganisation(schemeNotIncludedAsHasEvidenceNote.OrganisationId)
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                schemeNotIncludedAsHasNoteInIncorrectYear = SchemeDbSetup.Init().WithNewOrganisation().Create();
                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(recipientScheme.OrganisationId)
                    .WithOrganisation(schemeNotIncludedAsHasNoteInIncorrectYear.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear + 1)
                    .Create();

                var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
                var allowedNoteTypes = new List<NoteType> { NoteType.Transfer };

                request = new GetSchemeDataForFilterRequest(RecipientOrTransfer.Transfer, null, ComplianceYear, allowedStatuses, allowedNoteTypes);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEntityIdDisplayNameData = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCount = () =>
            {
                result.Should().HaveCount(2);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                var schemeData = result.FirstOrDefault(e => e.Id == scheme1.OrganisationId);
                schemeData.Should().NotBeNull();
                schemeData.Id.Should().Be(scheme1.OrganisationId);
                schemeData.DisplayName.Should().Be(scheme1.SchemeName);

                schemeData = result.FirstOrDefault(e => e.Id == scheme2.OrganisationId);
                schemeData.Should().NotBeNull();
                schemeData.Id.Should().Be(scheme2.OrganisationId);
                schemeData.DisplayName.Should().Be(scheme2.SchemeName);
            };

            private readonly It shouldHaveExcludedExpectedData = () =>
            {
                result.FirstOrDefault(e => e.Id == recipientScheme.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasDraftNote.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasNoteInIncorrectYear.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasEvidenceNote.OrganisationId)
                    .Should().BeNull();
            };

            private readonly It shouldHaveDistinctItems = () =>
            {
                result.Should().OnlyHaveUniqueItems();
            };

            private readonly It shouldBeOrderedCorrectly = () =>
            {
                result.Should().BeInAscendingOrder(e => e.DisplayName);
            };
        }

        [Component]
        public class WhenIGetRecipientSchemesForTransferNoteLookupForAnInternalUser : GetSchemeDataForFilterRequestHandlerIntegrationTestBase
        {
            private static Scheme scheme1;
            private static Scheme scheme2;
            private static Scheme transferScheme;
            private static Scheme schemeNotIncludedAsHasDraftNote;
            private static Scheme schemeNotIncludedAsHasNoteInIncorrectYear;

            private readonly Establish context = () =>
            {
                LocalInternalSetup();

                scheme1 = SchemeDbSetup.Init().WithNewOrganisation().Create();
                scheme2 = SchemeDbSetup.Init().WithNewOrganisation().Create();
                transferScheme = SchemeDbSetup.Init().WithNewOrganisation().Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Returned, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme2.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Void, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme2.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Rejected, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                schemeNotIncludedAsHasDraftNote = SchemeDbSetup.Init().WithNewOrganisation().Create();
                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(schemeNotIncludedAsHasDraftNote.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                schemeNotIncludedAsHasNoteInIncorrectYear = SchemeDbSetup.Init().WithNewOrganisation().Create();
                TransferEvidenceNoteDbSetup.Init()
                    .WithRecipient(schemeNotIncludedAsHasNoteInIncorrectYear.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear + 1)
                    .Create();

                var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
                var allowedNotes = new List<NoteType> { NoteType.Transfer };
                request = new GetSchemeDataForFilterRequest(RecipientOrTransfer.Recipient, null, ComplianceYear, allowedStatuses, allowedNotes);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEntityIdDisplayNameData = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCount = () =>
            {
                result.Should().HaveCount(2);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                var schemeData = result.FirstOrDefault(e => e.Id == scheme1.OrganisationId);
                schemeData.Should().NotBeNull();
                schemeData.Id.Should().Be(scheme1.OrganisationId);
                schemeData.DisplayName.Should().Be(scheme1.SchemeName);

                schemeData = result.FirstOrDefault(e => e.Id == scheme2.OrganisationId);
                schemeData.Should().NotBeNull();
                schemeData.Id.Should().Be(scheme2.OrganisationId);
                schemeData.DisplayName.Should().Be(scheme2.SchemeName);
            };

            private readonly It shouldHaveExcludedExpectedData = () =>
            {
                result.FirstOrDefault(e => e.Id == transferScheme.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasDraftNote.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasNoteInIncorrectYear.OrganisationId)
                    .Should().BeNull();
            };

            private readonly It shouldHaveDistinctItems = () =>
            {
                result.Should().OnlyHaveUniqueItems();
            };

            private readonly It shouldBeOrderedCorrectly = () =>
            {
                result.Should().BeInAscendingOrder(e => e.DisplayName);
            };
        }

        [Component]
        public class WhenIGetRecipientSchemesForEvidenceNoteLookupForAnInternalUser : GetSchemeDataForFilterRequestHandlerIntegrationTestBase
        {
            private static Scheme scheme1;
            private static Scheme scheme2;
            private static Scheme transferScheme;
            private static Scheme schemeNotIncludedAsHasDraftNote;
            private static Scheme schemeNotIncludedAsHasNoteInIncorrectYear;

            private readonly Establish context = () =>
            {
                LocalInternalSetup();

                scheme1 = SchemeDbSetup.Init().WithNewOrganisation().Create();
                scheme2 = SchemeDbSetup.Init().WithNewOrganisation().Create();
                transferScheme = SchemeDbSetup.Init().WithNewOrganisation().Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Returned, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme1.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme2.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Void, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(scheme2.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Rejected, UserId.ToString())
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                schemeNotIncludedAsHasDraftNote = SchemeDbSetup.Init().WithNewOrganisation().Create();
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(schemeNotIncludedAsHasDraftNote.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithComplianceYear(ComplianceYear)
                    .Create();

                schemeNotIncludedAsHasNoteInIncorrectYear = SchemeDbSetup.Init().WithNewOrganisation().Create();
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(schemeNotIncludedAsHasNoteInIncorrectYear.OrganisationId)
                    .WithOrganisation(transferScheme.OrganisationId)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithComplianceYear(ComplianceYear + 1)
                    .Create();

                var allowedStatuses = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
                var allowedNotes = new List<NoteType> { NoteType.Evidence };

                request = new GetSchemeDataForFilterRequest(RecipientOrTransfer.Recipient, null, ComplianceYear, allowedStatuses, allowedNotes);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldReturnListOfEntityIdDisplayNameData = () =>
            {
                result.Should().NotBeNull();
            };

            private readonly It shouldHaveExpectedResultsCount = () =>
            {
                result.Should().HaveCount(2);
            };

            private readonly It shouldHaveExpectedData = () =>
            {
                var schemeData = result.FirstOrDefault(e => e.Id == scheme1.OrganisationId);
                schemeData.Should().NotBeNull();
                schemeData.Id.Should().Be(scheme1.OrganisationId);
                schemeData.DisplayName.Should().Be(scheme1.SchemeName);

                schemeData = result.FirstOrDefault(e => e.Id == scheme2.OrganisationId);
                schemeData.Should().NotBeNull();
                schemeData.Id.Should().Be(scheme2.OrganisationId);
                schemeData.DisplayName.Should().Be(scheme2.SchemeName);
            };

            private readonly It shouldHaveExcludedExpectedData = () =>
            {
                result.FirstOrDefault(e => e.Id == transferScheme.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasDraftNote.OrganisationId)
                    .Should().BeNull();

                result.FirstOrDefault(e => e.Id == schemeNotIncludedAsHasNoteInIncorrectYear.OrganisationId)
                    .Should().BeNull();
            };

            private readonly It shouldHaveDistinctItems = () =>
            {
                result.Should().OnlyHaveUniqueItems();
            };

            private readonly It shouldBeOrderedCorrectly = () =>
            {
                result.Should().BeInAscendingOrder(e => e.DisplayName);
            };
        }

        public class GetSchemeDataForFilterRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static List<EntityIdDisplayNameData> result;
            protected static IRequestHandler<GetSchemeDataForFilterRequest, List<EntityIdDisplayNameData>> handler;
            protected static GetSchemeDataForFilterRequest request;
            protected const int ComplianceYear = 2022;

            public static void LocalExternalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetData: true)
                    .WithExternalUserAccess();

                handler = Container.Resolve<IRequestHandler<GetSchemeDataForFilterRequest, List<EntityIdDisplayNameData>>>();
            }

            public static void LocalInternalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings(resetData: true)
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Standard", CompetentAuthority.England);

                handler = Container.Resolve<IRequestHandler<GetSchemeDataForFilterRequest, List<EntityIdDisplayNameData>>>();
            }
        }
    }
}
