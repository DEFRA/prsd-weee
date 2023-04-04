namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Scheme;
    using Core.Shared;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;

    public class GetSchemesWithObligationOrEvidenceByComplianceYearHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetSchemesWithEvidenceOrObligations : GetSchemesWithObligationOrEvidenceByComplianceYearHandlerIntegrationTestBase
        {
            private static Scheme scheme1;
            private static Scheme scheme2;
            private static Scheme scheme3;
            private static Scheme scheme4;
            private static Scheme schemeWithObligationNotMatchingComplianceYear;
            private static Scheme schemeWithEvidenceNotMatchingComplianceYear;
            private static Scheme schemeWithTransferNotMatchingComplianceYear;
            private static Scheme schemeWithTransferOrganisationNotMatchingComplianceYear;
            private static Scheme schemeWithAatfNotMatchingEvidenceNotes;
            private static Scheme schemeWithDraftEvidenceNote;
            private static Scheme schemeWithSubmittedEvidenceNote;
            private static Scheme schemeWithReturnedEvidenceNote;
            private static Scheme schemeWithRejectedEvidenceNote;
            private static Scheme schemeWithVoidEvidenceNote;

            private static Scheme schemeAsRecipientOfDraftTransferNote;
            private static Scheme schemeAsRecipientOfSubmittedTransferNote;
            private static Scheme schemeAsRecipientOfReturnedTransferNote;
            private static Scheme schemeAsRecipientOfRejectedTransferNote;
            private static Scheme schemeAsRecipientOfVoidTransferNote;

            private static Scheme schemeAsTransfererOfDraftTransferNote;
            private static Scheme schemeAsTransfererOfSubmittedTransferNote;
            private static Scheme schemeAsTransfererOfReturnedTransferNote;
            private static Scheme schemeAsTransfererOfRejectedTransferNote;
            private static Scheme schemeAsTransfererOfVoidTransferNote;

            private readonly Establish context = () =>
            {
                LocalSetup();

                var authority = Query.GetCompetentAuthorityByName("Environment Agency");
                
                // scheme should be returned as has an obligation in compliance year
                var organisationForScheme1 = OrganisationDbSetup.Init().Create();
                scheme1 = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForScheme1.Id)
                    .Create();

                var obligationUpload = ObligationUploadDbSetup.Init().Create();

                // should be returned
                var amounts1 = new List<ObligationSchemeAmount>()
                {
                    new ObligationSchemeAmount(WeeeCategory.LargeHouseholdAppliances, 1000)
                };

                ObligationSchemeDbSetup.Init()
                    .WithScheme(scheme1.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithObligationAmounts(amounts1)
                    .WithComplianceYear(2022).Create();

                // scheme should be returned as has recipient of approved evidence note
                var organisationForScheme2 = OrganisationDbSetup.Init().Create();
                scheme2 = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForScheme2.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatusUpdate(NoteStatus.Approved)
                    .WithRecipient(organisationForScheme2.Id)
                    .Create();

                // scheme should be returned as has recipient of approved transfer note
                var organisationForScheme3 = OrganisationDbSetup.Init().Create();
                scheme3 = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForScheme3.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                // scheme should be returned as has transfer organisation of approved transfer note
                var organisationForScheme4 = OrganisationDbSetup.Init().Create();
                scheme4 = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForScheme4.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithOrganisation(organisationForScheme4.Id)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                // should not be returned not matching compliance year
                var organisationForSchemeWithObligationNotMatchingComplianceYear = OrganisationDbSetup.Init().Create();
                schemeWithObligationNotMatchingComplianceYear = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeWithObligationNotMatchingComplianceYear.Id)
                    .Create();

                ObligationSchemeDbSetup.Init()
                    .WithScheme(schemeWithObligationNotMatchingComplianceYear.Id)
                    .WithObligationUpload(obligationUpload.Id)
                    .WithComplianceYear(2021).Create();

                // should not be returned not matching evidence note in year
                var organisationForSchemeWithEvidenceNotMatchingComplianceYear = OrganisationDbSetup.Init().Create();
                schemeWithEvidenceNotMatchingComplianceYear = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeWithEvidenceNotMatchingComplianceYear.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2023)
                    .WithRecipient(organisationForSchemeWithEvidenceNotMatchingComplianceYear.Id)
                    .Create();

                // should not be returned not matching transfer note in year
                var organisationForSchemeWithTransferNotMatchingComplianceYear = OrganisationDbSetup.Init().Create();
                schemeWithTransferNotMatchingComplianceYear = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeWithTransferNotMatchingComplianceYear.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2023)
                    .WithRecipient(organisationForSchemeWithTransferNotMatchingComplianceYear.Id)
                    .Create();

                // should not be returned not matching transfer organisation note in year
                var organisationForSchemeWithTransferOrganisationNotMatchingComplianceYear = OrganisationDbSetup.Init().Create();
                schemeWithTransferOrganisationNotMatchingComplianceYear = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeWithTransferOrganisationNotMatchingComplianceYear.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2023)
                    .WithOrganisation(organisationForSchemeWithTransferOrganisationNotMatchingComplianceYear.Id)
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                // should not be returned as organisation has scheme and aatf but scheme has no notes
                var organisationForSchemeWithAatfAndSchemeWhereSchemeHasNoNotes = OrganisationDbSetup.Init().Create();
                var aatf = AatfDbSetup.Init()
                    .WithOrganisation(organisationForSchemeWithAatfAndSchemeWhereSchemeHasNoNotes.Id).Create();

                schemeWithAatfNotMatchingEvidenceNotes = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeWithAatfAndSchemeWhereSchemeHasNoNotes.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithAatf(aatf.Id)
                    .WithOrganisation(organisationForSchemeWithAatfAndSchemeWhereSchemeHasNoNotes.Id)
                    .WithRecipient(organisationForScheme1.Id)
                    .Create();

                // scheme should not be returned as has recipient of draft evidence note
                var organisationForSchemeOfDraftEvidenceNote = OrganisationDbSetup.Init().Create();
                schemeWithDraftEvidenceNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeOfDraftEvidenceNote.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatusUpdate(NoteStatus.Draft)
                    .WithRecipient(organisationForSchemeOfDraftEvidenceNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of submitted evidence note
                var organisationForSchemeOfSubmittedEvidenceNote = OrganisationDbSetup.Init().Create();
                schemeWithSubmittedEvidenceNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeOfSubmittedEvidenceNote.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatusUpdate(NoteStatus.Submitted)
                    .WithRecipient(organisationForSchemeOfSubmittedEvidenceNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of returned evidence note
                var organisationForSchemeOfReturnedEvidenceNote = OrganisationDbSetup.Init().Create();
                schemeWithReturnedEvidenceNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeOfReturnedEvidenceNote.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatusUpdate(NoteStatus.Returned)
                    .WithRecipient(organisationForSchemeOfReturnedEvidenceNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of rejected evidence note
                var organisationForSchemeOfRejectedEvidenceNote = OrganisationDbSetup.Init().Create();
                schemeWithRejectedEvidenceNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeOfRejectedEvidenceNote.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatusUpdate(NoteStatus.Rejected)
                    .WithRecipient(organisationForSchemeOfRejectedEvidenceNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of void evidence note
                var organisationForSchemeOfVoidEvidenceNote = OrganisationDbSetup.Init().Create();
                schemeWithVoidEvidenceNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeOfVoidEvidenceNote.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatusUpdate(NoteStatus.Void)
                    .WithRecipient(organisationForSchemeOfVoidEvidenceNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of draft transfer note
                var organisationForSchemeAsRecipientOfDraftTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsRecipientOfDraftTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsRecipientOfDraftTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithOrganisation(organisationForScheme3.Id)
                    .WithRecipient(organisationForSchemeAsRecipientOfDraftTransferNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of submitted transfer note
                var organisationForSchemeAsRecipientOfSubmittedTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsRecipientOfSubmittedTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsRecipientOfSubmittedTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithOrganisation(organisationForScheme3.Id)
                    .WithRecipient(organisationForSchemeAsRecipientOfSubmittedTransferNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of returned transfer note
                var organisationForSchemeAsRecipientOfReturnedTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsRecipientOfReturnedTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsRecipientOfReturnedTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Returned, UserId.ToString())
                    .WithOrganisation(organisationForScheme3.Id)
                    .WithRecipient(organisationForSchemeAsRecipientOfReturnedTransferNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of rejected transfer note
                var organisationForSchemeAsRecipientOfRejectedTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsRecipientOfRejectedTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsRecipientOfRejectedTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Rejected, UserId.ToString())
                    .WithOrganisation(organisationForScheme3.Id)
                    .WithRecipient(organisationForSchemeAsRecipientOfRejectedTransferNote.Id)
                    .Create();

                // scheme should not be returned as has recipient of void transfer note
                var organisationForSchemeAsRecipientOfVoidTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsRecipientOfVoidTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsRecipientOfVoidTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithStatus(NoteStatus.Void, UserId.ToString())
                    .WithOrganisation(organisationForScheme3.Id)
                    .WithRecipient(organisationForSchemeAsRecipientOfVoidTransferNote.Id)
                    .Create();

                // scheme should not be returned as has transfer organisation of draft transfer note
                var organisationForSchemeAsTransfererOfDraftTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsTransfererOfDraftTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsTransfererOfDraftTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithOrganisation(organisationForSchemeAsTransfererOfDraftTransferNote.Id)
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                // scheme should not be returned as has transfer organisation of submitted transfer note
                var organisationForSchemeAsTransfererOfSubmittedTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsTransfererOfSubmittedTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsTransfererOfSubmittedTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithOrganisation(organisationForSchemeAsTransfererOfSubmittedTransferNote.Id)
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                // scheme should not be returned as has transfer organisation of returned transfer note
                var organisationForSchemeAsTransfererOfReturnedTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsTransfererOfReturnedTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsTransfererOfReturnedTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Returned, UserId.ToString())
                    .WithOrganisation(organisationForSchemeAsTransfererOfReturnedTransferNote.Id)
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                // scheme should not be returned as has transfer organisation of rejected transfer note
                var organisationForSchemeAsTransfererOfRejectedTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsTransfererOfRejectedTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsTransfererOfRejectedTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Rejected, UserId.ToString())
                    .WithOrganisation(organisationForSchemeAsTransfererOfRejectedTransferNote.Id)
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                // scheme should not be returned as has transfer organisation of void transfer note
                var organisationForSchemeAsTransfererOfVoidTransferNote = OrganisationDbSetup.Init().Create();
                schemeAsTransfererOfVoidTransferNote = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForSchemeAsTransfererOfVoidTransferNote.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithStatus(NoteStatus.Submitted, UserId.ToString())
                    .WithStatus(NoteStatus.Approved, UserId.ToString())
                    .WithStatus(NoteStatus.Void, UserId.ToString())
                    .WithOrganisation(organisationForSchemeAsTransfererOfVoidTransferNote.Id)
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                request = new GetSchemesWithObligationOrEvidence(2022);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedSchemeData = () =>
            {
                result.Should().NotBeNull();

                var schemeData = result.FirstOrDefault(s => s.Id.Equals(scheme1.Id));
                schemeData.Should().NotBeNull();
                schemeData.ShouldMapScheme(scheme1);

                schemeData = result.FirstOrDefault(s => s.Id.Equals(scheme2.Id));
                schemeData.Should().NotBeNull();
                schemeData.ShouldMapScheme(scheme2);

                schemeData = result.FirstOrDefault(s => s.Id.Equals(scheme3.Id));
                schemeData.Should().NotBeNull();
                schemeData.ShouldMapScheme(scheme3);

                schemeData = result.FirstOrDefault(s => s.Id.Equals(scheme4.Id));
                schemeData.Should().NotBeNull();
                schemeData.ShouldMapScheme(scheme4);

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithObligationNotMatchingComplianceYear.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithEvidenceNotMatchingComplianceYear.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithTransferNotMatchingComplianceYear.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithTransferOrganisationNotMatchingComplianceYear.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithAatfNotMatchingEvidenceNotes.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithDraftEvidenceNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithSubmittedEvidenceNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithReturnedEvidenceNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithRejectedEvidenceNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeWithVoidEvidenceNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsRecipientOfDraftTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsRecipientOfSubmittedTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsRecipientOfReturnedTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsRecipientOfRejectedTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsRecipientOfVoidTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsTransfererOfDraftTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsTransfererOfSubmittedTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsTransfererOfReturnedTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsTransfererOfRejectedTransferNote.Id);
                schemeData.Should().BeNull();

                schemeData = result.FirstOrDefault(s => s.Id == schemeAsTransfererOfVoidTransferNote.Id);
                schemeData.Should().BeNull();
            };

            private It shouldHaveReturnedOrderedSchemeData = () =>
            {
                result.Should().BeInAscendingOrder(s => s.SchemeName);
            };

            private It shouldHaveReturnedDistinctSchemeData = () =>
            {
                result.Should().OnlyHaveUniqueItems();
            };
        }

        public class GetSchemesWithObligationOrEvidenceByComplianceYearHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetSchemesWithObligationOrEvidence, List<SchemeData>> handler;
            protected static GetSchemesWithObligationOrEvidence request;
            protected static Fixture fixture;
            protected static List<SchemeData> result;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC(true)
                    .WithTestData(true)
                    .WithInternalUserAccess();

                Query.SetupUserWithRole(UserId.ToString(), "Administrator", CompetentAuthority.England);

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetSchemesWithObligationOrEvidence, List<SchemeData>>>();

                return setup;
            }
        }
    }
}
