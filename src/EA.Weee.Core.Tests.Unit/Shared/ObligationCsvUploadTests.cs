namespace EA.Weee.Core.Tests.Unit.Shared
{
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
    }
}
