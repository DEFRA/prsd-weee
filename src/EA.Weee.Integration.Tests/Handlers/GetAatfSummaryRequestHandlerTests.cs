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
    using Domain.Evidence;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using FluentAssertions;
    using NUnit.Specifications;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class GetAatfSummaryRequestHandlerTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAnAatfsSummaryData : GetAatfSummaryRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create(); 
                var aatf = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                var complianceYear = fixture.Create<int>();

                //draft note
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear).Create();

                //submitted notes
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //approved notes
                EvidenceNoteDbSetup.Init().WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //rejected notes
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Rejected, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Rejected, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Rejected, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //voided note
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Void, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //returned note
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Returned, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                request = new GetAatfSummaryRequest(aatf.Id, complianceYear);
            };

            private static List<NoteTonnage> NoteTonnages()
            {
                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 1),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 0, 0)
                };
                return categories;
            }

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedTonnages = () =>
            {
                result.EvidenceCategoryTotals.Count.Should().Be(14);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.LargeHouseholdAppliances)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.LargeHouseholdAppliances)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.SmallHouseholdAppliances)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.SmallHouseholdAppliances)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.ITAndTelecommsEquipment)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.ConsumerEquipment)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.ConsumerEquipment)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.LightingEquipment)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.LightingEquipment)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.ElectricalAndElectronicTools)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.ElectricalAndElectronicTools)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.ToysLeisureAndSports)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.ToysLeisureAndSports)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.MedicalDevices)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.MedicalDevices)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.MonitoringAndControlInstruments)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.MonitoringAndControlInstruments)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.AutomaticDispensers)).Received.Should().Be(4M);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.AutomaticDispensers)).Reused.Should().Be(2M);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.DisplayEquipment)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.DisplayEquipment)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.CoolingApplicancesContainingRefrigerants)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.CoolingApplicancesContainingRefrigerants)).Reused.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.GasDischargeLampsAndLedLightSources)).Received.Should().Be(0);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.GasDischargeLampsAndLedLightSources)).Reused.Should().Be(0);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.PhotovoltaicPanels)).Received.Should().Be(null);
                result.EvidenceCategoryTotals.First(c => c.CategoryId.Equals(Core.DataReturns.WeeeCategory.PhotovoltaicPanels)).Reused.Should().Be(null);
            };
        }

        [Component]
        public class WhenIGetAnAatfsSummaryDataWhereTheCurrentUserIsNotAuthorised : GetAatfSummaryRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var organisation = OrganisationDbSetup.Init().Create();
                var aatf = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();

                //draft note
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id).Create();

                request = new GetAatfSummaryRequest(aatf.Id, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetAatfSummaryRequestHandlerTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetAatfSummaryRequest, AatfEvidenceSummaryData> handler;
            protected static GetAatfSummaryRequest request;
            protected static AatfEvidenceSummaryData result;
            protected static List<Note> notes;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetAatfSummaryRequest, AatfEvidenceSummaryData>>();

                return setup;
            }
        }
    }
}
