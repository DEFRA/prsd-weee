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
    using Core.Admin.Obligation;
    using Domain.Obligation;
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
    using Requests.Admin.Obligations;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteStatusDomain = Domain.Evidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class GetObligationSummaryRequestHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetASchemesObligationSummary : GetObligationSummaryRequestHandlerIntegrationTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var recipientOrganisation = OrganisationDbSetup.Init().Create();
                var scheme = SchemeDbSetup.Init().WithOrganisation(recipientOrganisation.Id).Create();

                var obligationUpload = ObligationUploadDbSetup.Init().Create();
                var obligationAmounts = new List<ObligationSchemeAmount>()
                {
                    new ObligationSchemeAmount(WeeeCategory.PhotovoltaicPanels, 1000),
                    new ObligationSchemeAmount(WeeeCategory.MedicalDevices, 800),
                    new ObligationSchemeAmount(WeeeCategory.GasDischargeLampsAndLedLightSources, 0),
                    new ObligationSchemeAmount(WeeeCategory.ElectricalAndElectronicTools, null),
                    new ObligationSchemeAmount(WeeeCategory.ConsumerEquipment, 100),
                    new ObligationSchemeAmount(WeeeCategory.ToysLeisureAndSports, 1000.235M),
                    new ObligationSchemeAmount(WeeeCategory.AutomaticDispensers, 600),
                    new ObligationSchemeAmount(WeeeCategory.DisplayEquipment, 200),
                    new ObligationSchemeAmount(WeeeCategory.CoolingApplicancesContainingRefrigerants, null),
                    new ObligationSchemeAmount(WeeeCategory.SmallHouseholdAppliances, 20),
                    new ObligationSchemeAmount(WeeeCategory.LargeHouseholdAppliances, 567),
                    new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, 150.5M),
                    new ObligationSchemeAmount(WeeeCategory.LightingEquipment, null),
                    new ObligationSchemeAmount(WeeeCategory.MonitoringAndControlInstruments, 1),
                };
                ObligationSchemeDbSetup.Init().WithScheme(scheme.Id).WithObligationUpload(obligationUpload.Id).WithObligationAmounts(obligationAmounts).WithComplianceYear(2022).Create();

                //create an obligation
                var tonnages = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 100, 50),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 50, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 200, 10),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 250, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 150, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 75, 20),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 10, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 30, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 200.789M, 100),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 125, null),
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 100, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 80, 70),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 10, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 20, 0),
                };

                EvidenceNoteDbSetup.Init().WithRecipient(recipientOrganisation.Id)
                    .WithStatus(NoteStatusDomain.Submitted, UserId.ToString())
                    .WithStatus(NoteStatusDomain.Approved, UserId.ToString())
                    .WithComplianceYear(2022)
                    .WithTonnages(tonnages).Create();

                request = new GetObligationSummaryRequest(scheme.Id, 2022);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveTheExpectedData = () =>
            {
                var category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.PhotovoltaicPanels.ToInt());
                category.Obligation.Should().Be(1000);
                category.Evidence.Should().Be(100);
                category.Difference.Should().Be(900);
                category.Reuse.Should().Be(50);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.MedicalDevices.ToInt());
                category.Obligation.Should().Be(800);
                category.Evidence.Should().Be(50);
                category.Difference.Should().Be(750);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.GasDischargeLampsAndLedLightSources.ToInt());
                category.Obligation.Should().Be(0);
                category.Evidence.Should().Be(200);
                category.Difference.Should().Be(-200);
                category.Reuse.Should().Be(10);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.ElectricalAndElectronicTools.ToInt());
                category.Obligation.Should().Be(0);
                category.Evidence.Should().Be(250);
                category.Difference.Should().Be(-250);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.ConsumerEquipment.ToInt());
                category.Obligation.Should().Be(100);
                category.Evidence.Should().Be(150);
                category.Difference.Should().Be(-50);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.ToysLeisureAndSports.ToInt());
                category.Obligation.Should().Be(1000.235M);
                category.Evidence.Should().Be(75);
                category.Difference.Should().Be(925.235M);
                category.Reuse.Should().Be(20);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.AutomaticDispensers.ToInt());
                category.Obligation.Should().Be(600);
                category.Evidence.Should().Be(10);
                category.Difference.Should().Be(590);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.DisplayEquipment.ToInt());
                category.Obligation.Should().Be(200);
                category.Evidence.Should().Be(30);
                category.Difference.Should().Be(170);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.CoolingApplicancesContainingRefrigerants.ToInt());
                category.Obligation.Should().Be(0);
                category.Evidence.Should().Be(200.789M);
                category.Difference.Should().Be(-200.789M);
                category.Reuse.Should().Be(100);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.SmallHouseholdAppliances.ToInt());
                category.Obligation.Should().Be(20);
                category.Evidence.Should().Be(125);
                category.Difference.Should().Be(-105);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.LargeHouseholdAppliances.ToInt());
                category.Obligation.Should().Be(567);
                category.Evidence.Should().Be(100);
                category.Difference.Should().Be(467);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.ITAndTelecommsEquipment.ToInt());
                category.Obligation.Should().Be(150.5M);
                category.Evidence.Should().Be(80);
                category.Difference.Should().Be(70.500M);
                category.Reuse.Should().Be(70);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.LightingEquipment.ToInt());
                category.Obligation.Should().Be(0);
                category.Evidence.Should().Be(10);
                category.Difference.Should().Be(-10);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
                category =
                    result.ObligationEvidenceValues.First(r => r.CategoryId.ToInt() == WeeeCategory.MonitoringAndControlInstruments.ToInt());
                category.Obligation.Should().Be(1);
                category.Evidence.Should().Be(20);
                category.Difference.Should().Be(-19);
                category.Reuse.Should().Be(0);
                category.TransferredIn.Should().Be(0);
                category.TransferredOut.Should().Be(0);
            };
        }

        public class GetObligationSummaryRequestHandlerIntegrationTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetObligationSummaryRequest, ObligationEvidenceSummaryData> handler;
            protected static GetObligationSummaryRequest request;
            protected static ObligationEvidenceSummaryData result;

            public static void LocalSetup()
            {
                SetupTest(IocApplication.RequestHandler)
                    .WithDefaultSettings()
                    .WithInternalUserAccess(false);

                var authority = Query.GetEaCompetentAuthority();
                var role = Query.GetInternalUserRole();

                Query.SetupUserWithRole(UserId.ToString(), role.Id, authority.Id);

                handler = Container.Resolve<IRequestHandler<GetObligationSummaryRequest, ObligationEvidenceSummaryData>>();
            }
        }
    }
}
