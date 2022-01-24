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
            var dictionary = ReportHelper.GetCategoryDisplayNames();

            // Assert
            Assert.Equal(14, dictionary.Count);
            Assert.Equal("01. Large household appliances", dictionary[(int)WeeeCategory.LargeHouseholdAppliances]);
            Assert.Equal("02. Small household appliances", dictionary[(int)WeeeCategory.SmallHouseholdAppliances]);
            Assert.Equal("03. IT and telecommunications equipment", dictionary[(int)WeeeCategory.ITAndTelecommsEquipment]);
            Assert.Equal("04. Consumer equipment", dictionary[(int)WeeeCategory.ConsumerEquipment]);
            Assert.Equal("05. Lighting equipment", dictionary[(int)WeeeCategory.LightingEquipment]);
            Assert.Equal("06. Electrical and electronic tools", dictionary[(int)WeeeCategory.ElectricalAndElectronicTools]);
            Assert.Equal("07. Toys, leisure and sports equipment", dictionary[(int)WeeeCategory.ToysLeisureAndSports]);
            Assert.Equal("08. Medical devices", dictionary[(int)WeeeCategory.MedicalDevices]);
            Assert.Equal("09. Monitoring and control instruments", dictionary[(int)WeeeCategory.MonitoringAndControlInstruments]);
            Assert.Equal("10. Automatic dispensers", dictionary[(int)WeeeCategory.AutomaticDispensers]);
            Assert.Equal("11. Display equipment", dictionary[(int)WeeeCategory.DisplayEquipment]);
            Assert.Equal("12. Appliances containing refrigerants", dictionary[(int)WeeeCategory.CoolingApplicancesContainingRefrigerants]);
            Assert.Equal("13. Gas discharge lamps and LED light sources", dictionary[(int)WeeeCategory.GasDischargeLampsAndLedLightSources]);
            Assert.Equal("14. Photovoltaic panels", dictionary[(int)WeeeCategory.PhotovoltaicPanels]);
        }
    }
}
