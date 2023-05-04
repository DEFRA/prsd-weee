namespace EA.Weee.Integration.Tests.Handlers
{
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Autofac;
    using AutoFixture;
    using Base;
    using Builders;
    using Core.Admin;
    using Domain.Evidence;
    using Domain.Lookup;
    using EA.Prsd.Core;
    using FluentAssertions;
    using NUnit.Specifications;
    using NUnit.Specifications.Categories;
    using Prsd.Core.Autofac;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence.Reports;
    using NoteStatus = Domain.Evidence.NoteStatus;

    public class GetAatfSummaryReportHandlerIntegrationTests : IntegrationTestBase
    {
        [Component]
        public class WhenIGetAnAatfsSummaryCsvData : GetAatfSummaryReportRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create(); 
                var aatf = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                var complianceYear = fixture.Create<int>();

                //draft notes
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear).Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteWithTonnage(2.2M, 1.1M))
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear).Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnagesWithNullTonnage())
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
                    .WithTonnages(NoteWithTonnage(3.3M, 2.2M))
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnagesWithNullTonnage())
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
                    .WithTonnages(NoteWithTonnage(4.4M, 3.3M))
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
                    .WithTonnages(NoteTonnagesWithNullTonnage())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //rejected note not included
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

                //voided note not included
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
                        n.UpdateStatus(NoteStatus.Void, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //returned note not included
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

                //submitted note not correct compliance year
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear + 1)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //submitted note different aatf
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //approved note not correct compliance year
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear + 1)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //approved note different aatf
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear)
                    .With(n =>
                    {
                        n.UpdateStatus(NoteStatus.Submitted, UserId.ToString(), SystemTime.UtcNow);
                        n.UpdateStatus(NoteStatus.Approved, UserId.ToString(), SystemTime.UtcNow);
                    }).Create();

                //draft note not correct compliance year
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithAatf(aatf.Id)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear + 1)
                    .Create();

                //draft note different aatf
                EvidenceNoteDbSetup.Init()
                    .WithRecipient(SchemeDbSetup.Init().WithNewOrganisation().Create().OrganisationId)
                    .WithTonnages(NoteTonnages())
                    .WithOrganisation(organisation.Id)
                    .WithComplianceYear(complianceYear).Create();

                request = new GetAatfSummaryReportRequest(aatf.Id, complianceYear);
            };

            private static List<NoteTonnage> NoteTonnages()
            {
                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, 1, 2),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, 2, 3),
                    new NoteTonnage(WeeeCategory.MedicalDevices, 3, 4),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, 4, 5),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, 5, 6),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, 6, 7),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, 7, 8),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, 8, 9),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, 9, 10),
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, 10, 11),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, 11, 12),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, 12, 13),
                    new NoteTonnage(WeeeCategory.LightingEquipment, 13, 14),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, 14, 15)
                };
                return categories;
            }

            private static List<NoteTonnage> NoteWithTonnage(decimal received, decimal reused)
            {
                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, received, reused),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, received, reused),
                    new NoteTonnage(WeeeCategory.MedicalDevices, received, reused),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, received, reused),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, received, reused),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, received, reused),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, received, reused),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, received, reused),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, received, reused),
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, received, reused),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, received, reused),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, received, reused),
                    new NoteTonnage(WeeeCategory.LightingEquipment, received, reused),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, received, reused)
                };
                return categories;
            }

            private static List<NoteTonnage> NoteTonnagesWithNullTonnage()
            {
                var categories = new List<NoteTonnage>()
                {
                    new NoteTonnage(WeeeCategory.LargeHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.AutomaticDispensers, null, null),
                    new NoteTonnage(WeeeCategory.MedicalDevices, null, null),
                    new NoteTonnage(WeeeCategory.ConsumerEquipment, null, null),
                    new NoteTonnage(WeeeCategory.CoolingApplicancesContainingRefrigerants, null, null),
                    new NoteTonnage(WeeeCategory.DisplayEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ITAndTelecommsEquipment, null, null),
                    new NoteTonnage(WeeeCategory.ToysLeisureAndSports, null, null),
                    new NoteTonnage(WeeeCategory.SmallHouseholdAppliances, null, null),
                    new NoteTonnage(WeeeCategory.PhotovoltaicPanels, null, null),
                    new NoteTonnage(WeeeCategory.ElectricalAndElectronicTools, null, null),
                    new NoteTonnage(WeeeCategory.GasDischargeLampsAndLedLightSources, null, null),
                    new NoteTonnage(WeeeCategory.LightingEquipment, null, null),
                    new NoteTonnage(WeeeCategory.MonitoringAndControlInstruments, null, null)
                };
                return categories;
            }

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var expectedCsvData =
                    "Category,Approved evidence (tonnes),Approved reuse (tonnes),Submitted evidence (tonnes),Submitted reuse (tonnes),Draft evidence (tonnes),Draft reuse (tonnes)\r\n" +

                    "1. Large household appliances,5.400,5.300,4.300,4.200,3.200,3.100\r\n" +
                    "2. Small household appliances,13.400,13.300,12.300,12.200,11.200,11.100\r\n" +
                    "3. IT and telecommunications equipment,11.400,11.300,10.300,10.200,9.200,9.100\r\n" +
                    "4. Consumer equipment,8.400,8.300,7.300,7.200,6.200,6.100\r\n" +
                    "5. Lighting equipment,17.400,17.300,16.300,16.200,15.200,15.100\r\n" +
                    "6. Electrical and electronic tools,15.400,15.300,14.300,14.200,13.200,13.100\r\n" +
                    "\"7. Toys, leisure and sports equipment\",12.400,12.300,11.300,11.200,10.200,10.100\r\n" +
                    "8. Medical devices,7.400,7.300,6.300,6.200,5.200,5.100\r\n" +
                    "9. Monitoring and control instruments,18.400,18.300,17.300,17.200,16.200,16.100\r\n" +
                    "10. Automatic dispensers,6.400,6.300,5.300,5.200,4.200,4.100\r\n" +
                    "11. Display equipment,10.400,10.300,9.300,9.200,8.200,8.100\r\n" +
                    "12. Appliances containing refrigerants,9.400,9.300,8.300,8.200,7.200,7.100\r\n" +
                    "13. Gas discharge lamps and LED light sources,16.400,16.300,15.300,15.200,14.200,14.100\r\n" +
                    "14. Photovoltaic panels,14.400,14.300,13.300,13.200,12.200,12.100\r\n" +
                    "Total (tonnes),166.600,165.200,151.200,149.800,135.800,134.400\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{request.ComplianceYear}_Summary report");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetAnAatfsSummaryCsvDataThatHasNoTonnage : GetAatfSummaryReportRequestHandlerTestBase
        {
            private readonly Establish context = () =>
            {
                LocalSetup();

                var organisation = OrganisationDbSetup.Init().Create();
                OrganisationUserDbSetup.Init().WithUserIdAndOrganisationId(UserId, organisation.Id).Create();
                var aatf = AatfDbSetup.Init().WithOrganisation(organisation.Id).Create();
                var complianceYear = fixture.Create<int>();

                request = new GetAatfSummaryReportRequest(aatf.Id, complianceYear);
            };

            private readonly Because of = () =>
            {
                result = Task.Run(async () => await handler.HandleAsync(request)).Result;
            };

            private readonly It shouldHaveReturnedExpectedCsv = () =>
            {
                var expectedCsvData =
                    "Category,Approved evidence (tonnes),Approved reuse (tonnes),Submitted evidence (tonnes),Submitted reuse (tonnes),Draft evidence (tonnes),Draft reuse (tonnes)\r\n" +

                    "1. Large household appliances,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "2. Small household appliances,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "3. IT and telecommunications equipment,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "4. Consumer equipment,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "5. Lighting equipment,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "6. Electrical and electronic tools,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "\"7. Toys, leisure and sports equipment\",0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "8. Medical devices,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "9. Monitoring and control instruments,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "10. Automatic dispensers,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "11. Display equipment,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "12. Appliances containing refrigerants,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "13. Gas discharge lamps and LED light sources,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "14. Photovoltaic panels,0.000,0.000,0.000,0.000,0.000,0.000\r\n" +
                    "Total (tonnes),0.000,0.000,0.000,0.000,0.000,0.000\r\n";

                result.FileContent.Should().Be(expectedCsvData);
                result.FileName.Should().Contain($"{request.ComplianceYear}_Summary report");
                result.FileName.Should().EndWith(".csv");
            };
        }

        [Component]
        public class WhenIGetAnAatfsSummaryCsvDataWhereTheCurrentUserIsNotAuthorised : GetAatfSummaryReportRequestHandlerTestBase
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

                request = new GetAatfSummaryReportRequest(aatf.Id, SystemTime.UtcNow.Year);
            };

            private readonly Because of = () =>
            {
                CatchExceptionAsync(() => handler.HandleAsync(request));
            };

            private readonly It shouldHaveCaughtArgumentException = ShouldThrowException<SecurityException>;
        }

        public class GetAatfSummaryReportRequestHandlerTestBase : WeeeContextSpecification
        {
            protected static IRequestHandler<GetAatfSummaryReportRequest, CSVFileData> handler;
            protected static GetAatfSummaryReportRequest request;
            protected static CSVFileData result;
            protected static Fixture fixture;

            public static IntegrationTestSetupBuilder LocalSetup()
            {
                var setup = SetupTest(IocApplication.RequestHandler)
                    .WithIoC()
                    .WithTestData()
                    .WithExternalUserAccess();

                fixture = new Fixture();
                handler = Container.Resolve<IRequestHandler<GetAatfSummaryReportRequest, CSVFileData>>();

                return setup;
            }
        }
    }
}
