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
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Scheme;
    using FluentAssertions;
    using NUnit.Specifications;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;

    public class GetSchemesWithObligationOrEvidenceByComplianceYearHandler : IntegrationTestBase
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

                // scheme should be returned as has recipient of evidence note
                var organisationForScheme2 = OrganisationDbSetup.Init().Create();
                scheme2 = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForScheme2.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithRecipient(organisationForScheme2.Id)
                    .Create();

                EvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithRecipient(organisationForScheme2.Id)
                    .Create();

                // scheme should be returned as has recipient of transfer note
                var organisationForScheme3 = OrganisationDbSetup.Init().Create();
                scheme3 = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForScheme3.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithRecipient(organisationForScheme3.Id)
                    .Create();

                // scheme should be returned as has transfer organisation of transfer note
                var organisationForScheme4 = OrganisationDbSetup.Init().Create();
                scheme4 = SchemeDbSetup.Init()
                    .WithAuthority(authority.Id)
                    .WithOrganisation(organisationForScheme4.Id)
                    .Create();

                TransferEvidenceNoteDbSetup.Init()
                    .WithComplianceYear(2022)
                    .WithOrganisation(organisationForScheme4.Id)
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
