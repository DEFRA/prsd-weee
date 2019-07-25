namespace EA.Weee.RequestHandlers.Admin.Helpers
{
    using System.Collections.Generic;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain.Lookup;

    public static class ReportHelper
    {
        public static readonly Dictionary<WeeeCategory, string> CategoryDisplayNames = new Dictionary<WeeeCategory, string>()
        {
            { WeeeCategory.LargeHouseholdAppliances, $"1. {WeeeCategory.LargeHouseholdAppliances.ToDisplayString()}" },
            { WeeeCategory.SmallHouseholdAppliances, $"2. {WeeeCategory.SmallHouseholdAppliances.ToDisplayString()}" },
            { WeeeCategory.ITAndTelecommsEquipment, $"3. {WeeeCategory.ITAndTelecommsEquipment.ToDisplayString()}" },
            { WeeeCategory.ConsumerEquipment, $"4. {WeeeCategory.ConsumerEquipment.ToDisplayString()}" },
            { WeeeCategory.LightingEquipment, $"5. {WeeeCategory.LightingEquipment.ToDisplayString()}" },
            { WeeeCategory.ElectricalAndElectronicTools, $"6. {WeeeCategory.ElectricalAndElectronicTools.ToDisplayString()}" },
            { WeeeCategory.ToysLeisureAndSports, $"7. {WeeeCategory.ToysLeisureAndSports.ToDisplayString()}" },
            { WeeeCategory.MedicalDevices, $"8. {WeeeCategory.MedicalDevices.ToDisplayString()}" },
            { WeeeCategory.MonitoringAndControlInstruments, $"9. {WeeeCategory.MonitoringAndControlInstruments.ToDisplayString()}" },
            { WeeeCategory.AutomaticDispensers, $"10. {WeeeCategory.AutomaticDispensers.ToDisplayString()}" },
            { WeeeCategory.DisplayEquipment, $"11. {WeeeCategory.DisplayEquipment.ToDisplayString()}" },
            { WeeeCategory.CoolingApplicancesContainingRefrigerants, $"12. {WeeeCategory.CoolingApplicancesContainingRefrigerants.ToDisplayString()}" },
            { WeeeCategory.GasDischargeLampsAndLedLightSources, $"13. {WeeeCategory.GasDischargeLampsAndLedLightSources.ToDisplayString()}" },
            { WeeeCategory.PhotovoltaicPanels, $"14. {WeeeCategory.PhotovoltaicPanels.ToDisplayString()}" }
        };
    }
}
