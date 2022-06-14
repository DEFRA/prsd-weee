namespace EA.Weee.Core.Tests.Unit.Shared
{
    using System;
    using System.Linq;
    using Core.Helpers;
    using Core.Shared.CsvReading;
    using Domain.Lookup;
    using FluentAssertions;
    using Xunit;

    public class ObligationCsvUploadTests
    {
        [Theory]
        [InlineData("Cat1", WeeeCategory.LargeHouseholdAppliances)]
        [InlineData("Cat2", WeeeCategory.SmallHouseholdAppliances)]
        [InlineData("Cat3", WeeeCategory.ITAndTelecommsEquipment)]
        [InlineData("Cat4", WeeeCategory.ConsumerEquipment)]
        [InlineData("Cat5", WeeeCategory.LightingEquipment)]
        [InlineData("Cat6", WeeeCategory.ElectricalAndElectronicTools)]
        [InlineData("Cat7", WeeeCategory.ToysLeisureAndSports)]
        [InlineData("Cat8", WeeeCategory.MedicalDevices)]
        [InlineData("Cat9", WeeeCategory.MonitoringAndControlInstruments)]
        [InlineData("Cat10", WeeeCategory.AutomaticDispensers)]
        [InlineData("Cat11", WeeeCategory.DisplayEquipment)]
        [InlineData("Cat12", WeeeCategory.CoolingApplicancesContainingRefrigerants)]
        [InlineData("Cat13", WeeeCategory.GasDischargeLampsAndLedLightSources)]
        [InlineData("Cat14", WeeeCategory.PhotovoltaicPanels)]
        public void ObligationCsvUpload_CategoryPropertiesShouldBeDecoratedWithCategoryAttribute(string property, WeeeCategory category)
        {
            typeof(ObligationCsvUpload).GetProperty(property).Should()
                .BeDecoratedWith<WeeeCategoryAttribute>(w => w.Category == category);
        }

        [Theory]
        [InlineData(" scheme")]
        [InlineData("scheme ")]
        [InlineData(" scheme ")]
        [InlineData("  scheme  ")]
        public void SchemeIdentifier_GivenWhiteSpaceAtStartOrEndOfSchemeIdentifier_ShouldBeTrimmed(string schemeIdentifier)
        {
            //arrange
            var obligationCsvUpload = new ObligationCsvUpload()
            {
                SchemeIdentifier = schemeIdentifier
            };

            //act
            var result = obligationCsvUpload.SchemeIdentifier;

            //assert
            result.Should().Be(schemeIdentifier.Trim());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GetValue_GivenEmptyValue_NullShouldBeReturned(string value)
        {
            //arrange
            var obligationCsvUpload = new ObligationCsvUpload()
            {
                Cat1 = value
            };

            //act
            var result = obligationCsvUpload.GetValue(WeeeCategory.LargeHouseholdAppliances);

            //assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("5.55")]
        public void GetValue_GivenValue_ValueShouldBeReturned(string value)
        {
            //arrange
            var obligationCsvUpload = new ObligationCsvUpload()
            {
                Cat1 = value
            };

            //act
            var result = obligationCsvUpload.GetValue(WeeeCategory.LargeHouseholdAppliances);

            //assert
            result.Should().Be(decimal.Parse(value));
        }

        [Fact]
        public void GetValue_GivenNoValuesSpecified_NullShouldBeReturned()
        {
            //arrange
            var obligationCsvUpload = new ObligationCsvUpload();

            //act
            var weeeCategories = Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().ToList();
            foreach (var weeeCategory in weeeCategories)        
            {
                var result = obligationCsvUpload.GetValue(weeeCategory);

                //assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public void GetValue_GivenValuesSpecified_NullShouldBeReturned()
        {
            //arrange
            var obligationCsvUpload = new ObligationCsvUpload()
            {
                Cat1 = "1",
                Cat2 = "2",
                Cat3 = "3",
                Cat4 = "4",
                Cat5 = "5",
                Cat6 = "6",
                Cat7 = "7",
                Cat8 = "8",
                Cat9 = "9",
                Cat10 = "10",
                Cat11 = "11",
                Cat12 = "12",
                Cat13 = "13",
                Cat14 = "14"
            };

            //act
            var weeeCategories = Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>().ToList();
            foreach (var weeeCategory in weeeCategories)
            {
                var result = obligationCsvUpload.GetValue(weeeCategory);

                //assert
                result.Should().Be(decimal.Parse(((int)weeeCategory).ToString()));
            }
        }
    }
}
