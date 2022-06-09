namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Shared.CsvReading;
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Obligations;
    using Weee.Tests.Core.SpecimenBuilders;
    using Xunit;

    public class SchemeObligationsDataProcessorTests
    {
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly SchemeObligationsDataProcessor schemeObligationsDataProcessor;
        private readonly Fixture fixture;

        public SchemeObligationsDataProcessorTests()
        {
            schemeDataAccess = A.Fake<ISchemeDataAccess>();
            fixture = new Fixture();
            fixture.Customizations.Add(new StringDecimalObligationUploadGenerator());

            schemeObligationsDataProcessor = new SchemeObligationsDataProcessor(schemeDataAccess);
        }

        [Theory]
        [InlineData(2022)]
        [InlineData(2023)]
        public async Task Build_givenObligationCsvUploads_ExpectedObligationSchemesShouldBeReturned(int complianceYear)
        {
            //arrange
            var obligations = fixture.CreateMany<ObligationCsvUpload>(3).ToList();

            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.ApprovalNumber).Returns(obligations.ElementAt(0).SchemeIdentifier);
            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.ApprovalNumber).Returns(obligations.ElementAt(1).SchemeIdentifier);
            var scheme3 = A.Fake<Scheme>();
            A.CallTo(() => scheme3.ApprovalNumber).Returns(obligations.ElementAt(2).SchemeIdentifier);

            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(obligations.ElementAt(0).SchemeIdentifier)).Returns(scheme1);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(obligations.ElementAt(1).SchemeIdentifier)).Returns(scheme2);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByApprovalNumber(obligations.ElementAt(2).SchemeIdentifier)).Returns(scheme3);

            //act
            var result = await schemeObligationsDataProcessor.Build(obligations, complianceYear);

            //assert
            result.Count.Should().Be(42);
            foreach (var obligation in obligations)
            {
                var schemeObligations = result.Where(s => s.Scheme.ApprovalNumber == obligation.SchemeIdentifier);

                var schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.ITAndTelecommsEquipment);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat3));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.AutomaticDispensers);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat10));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.MedicalDevices);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat8));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.PhotovoltaicPanels);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat14));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.ConsumerEquipment);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat4));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.CoolingApplicancesContainingRefrigerants);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat12));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.DisplayEquipment);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat11));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.ElectricalAndElectronicTools);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat6));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.GasDischargeLampsAndLedLightSources);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat13));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.LargeHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat1));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.MonitoringAndControlInstruments);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat9));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.SmallHouseholdAppliances);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat2));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.ToysLeisureAndSports);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat7));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
                schemeCategoryObligation =
                    schemeObligations.First(s => s.CategoryId == WeeeCategory.LightingEquipment);
                schemeCategoryObligation.Obligation.Should().Be(decimal.Parse(obligation.Cat5));
                schemeCategoryObligation.ComplianceYear.Should().Be(complianceYear);
            }
        }
    }
}
