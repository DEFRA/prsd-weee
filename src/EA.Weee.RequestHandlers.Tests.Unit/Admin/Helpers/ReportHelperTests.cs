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
            Assert.Equal("1. Large Household Appliances", dictionary[WeeeCategory.LargeHouseholdAppliances]);
            Assert.Equal("2. Small Household Appliances", dictionary[WeeeCategory.SmallHouseholdAppliances]);
            Assert.Equal("3. IT and Telecomms Equipment", dictionary[WeeeCategory.ITAndTelecommsEquipment]);
            Assert.Equal("4. Consumer Equipment", dictionary[WeeeCategory.ConsumerEquipment]);
            Assert.Equal("5. Lighting Equipment", dictionary[WeeeCategory.LightingEquipment]);
            Assert.Equal("6. Electrical and Electronic Tools", dictionary[WeeeCategory.ElectricalAndElectronicTools]);
            Assert.Equal("7. Toys Leisure and Sports", dictionary[WeeeCategory.ToysLeisureAndSports]);
            Assert.Equal("8. Medical Devices", dictionary[WeeeCategory.MedicalDevices]);
            Assert.Equal("9. Monitoring and Control Instruments", dictionary[WeeeCategory.MonitoringAndControlInstruments]);
            Assert.Equal("10. Automatic Dispensers", dictionary[WeeeCategory.AutomaticDispensers]);
            Assert.Equal("11. Display Equipment", dictionary[WeeeCategory.DisplayEquipment]);
            Assert.Equal("12. Cooling Appliances Containing Refrigerants", dictionary[WeeeCategory.CoolingApplicancesContainingRefrigerants]);
            Assert.Equal("13. Gas Discharge Lamps and LED light sources", dictionary[WeeeCategory.GasDischargeLampsAndLedLightSources]);
            Assert.Equal("14. Photovoltaic Panels", dictionary[WeeeCategory.PhotovoltaicPanels]);
        }
    }
}
