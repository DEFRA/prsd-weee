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
            result.Count.Should().Be(3);
            foreach (var obligation in obligations)
            {
                var schemeObligation = result.First(s => s.Scheme.ApprovalNumber == obligation.SchemeIdentifier);
                schemeObligation.ComplianceYear.Should().Be(complianceYear);

                schemeObligation.ObligationSchemeAmounts.Count.Should().Be(14);
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ITAndTelecommsEquipment)
                        .Obligation.Should().Be(decimal.Parse(obligation.Cat3));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.AutomaticDispensers)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat10));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MedicalDevices)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat8));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.PhotovoltaicPanels)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat14));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ConsumerEquipment)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat4));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.CoolingApplicancesContainingRefrigerants)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat12));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.DisplayEquipment)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat11));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ElectricalAndElectronicTools)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat6));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.GasDischargeLampsAndLedLightSources)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat13));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LargeHouseholdAppliances)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat1));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.MonitoringAndControlInstruments)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat9));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.SmallHouseholdAppliances)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat2));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.ToysLeisureAndSports)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat7));
                schemeObligation.ObligationSchemeAmounts.First(s => s.CategoryId == WeeeCategory.LightingEquipment)
                    .Obligation.Should().Be(decimal.Parse(obligation.Cat5));
            }
        }
    }
}
