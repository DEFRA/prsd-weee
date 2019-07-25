namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Helpers
{
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Admin.Helpers;
    using Xunit;

    public class ReportHelperTests
    {
        [Fact]
        public void CategoryDisplayNames_ReturnsExpectedDisplayNames()
        {
            // Act
            var dictionary = ReportHelper.CategoryDisplayNames;

            // Assert
            Assert.Equal(14, dictionary.Count);
            Assert.Equal("1. Large household appliances", dictionary[WeeeCategory.LargeHouseholdAppliances]);
            Assert.Equal("2. Small household appliances", dictionary[WeeeCategory.SmallHouseholdAppliances]);
            Assert.Equal("3. IT and telecommunications equipment", dictionary[WeeeCategory.ITAndTelecommsEquipment]);
            Assert.Equal("4. Consumer equipment", dictionary[WeeeCategory.ConsumerEquipment]);
            Assert.Equal("5. Lighting equipment", dictionary[WeeeCategory.LightingEquipment]);
            Assert.Equal("6. Electrical and electronic tools", dictionary[WeeeCategory.ElectricalAndElectronicTools]);
            Assert.Equal("7. Toys, leisure and sports equipment", dictionary[WeeeCategory.ToysLeisureAndSports]);
            Assert.Equal("8. Medical devices", dictionary[WeeeCategory.MedicalDevices]);
            Assert.Equal("9. Monitoring and control instruments", dictionary[WeeeCategory.MonitoringAndControlInstruments]);
            Assert.Equal("10. Automatic dispensers", dictionary[WeeeCategory.AutomaticDispensers]);
            Assert.Equal("11. Display equipment", dictionary[WeeeCategory.DisplayEquipment]);
            Assert.Equal("12. Appliances containing refrigerants", dictionary[WeeeCategory.CoolingApplicancesContainingRefrigerants]);
            Assert.Equal("13. Gas discharge lamps and LED light sources", dictionary[WeeeCategory.GasDischargeLampsAndLedLightSources]);
            Assert.Equal("14. Photovoltaic panels", dictionary[WeeeCategory.PhotovoltaicPanels]);
        }
    }
}
