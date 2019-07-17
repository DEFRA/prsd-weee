namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System.Collections.Generic;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain.Lookup;

    public static class ReportHelper
    {
        public static readonly Dictionary<WeeeCategory, string> CategoryDisplayNames = new Dictionary<WeeeCategory, string>()
        {
            { WeeeCategory.LargeHouseholdAppliances, $"1. {GetCsvDisplayName(WeeeCategory.LargeHouseholdAppliances)}" },
            { WeeeCategory.SmallHouseholdAppliances, $"2. {GetCsvDisplayName(WeeeCategory.SmallHouseholdAppliances)}" },
            { WeeeCategory.ITAndTelecommsEquipment, $"3. {GetCsvDisplayName(WeeeCategory.ITAndTelecommsEquipment)}" },
            { WeeeCategory.ConsumerEquipment, $"4. {GetCsvDisplayName(WeeeCategory.ConsumerEquipment)}" },
            { WeeeCategory.LightingEquipment, $"5. {GetCsvDisplayName(WeeeCategory.LightingEquipment)}" },
            { WeeeCategory.ElectricalAndElectronicTools, $"6. {GetCsvDisplayName(WeeeCategory.ElectricalAndElectronicTools)}" },
            { WeeeCategory.ToysLeisureAndSports, $"7. {GetCsvDisplayName(WeeeCategory.ToysLeisureAndSports)}" },
            { WeeeCategory.MedicalDevices, $"8. {GetCsvDisplayName(WeeeCategory.MedicalDevices)}" },
            { WeeeCategory.MonitoringAndControlInstruments, $"9. {GetCsvDisplayName(WeeeCategory.MonitoringAndControlInstruments)}" },
            { WeeeCategory.AutomaticDispensers, $"10. {GetCsvDisplayName(WeeeCategory.AutomaticDispensers)}" },
            { WeeeCategory.DisplayEquipment, $"11. {GetCsvDisplayName(WeeeCategory.DisplayEquipment)}" },
            { WeeeCategory.CoolingApplicancesContainingRefrigerants, $"12. {GetCsvDisplayName(WeeeCategory.CoolingApplicancesContainingRefrigerants)}" },
            { WeeeCategory.GasDischargeLampsAndLedLightSources, $"13. {GetCsvDisplayName(WeeeCategory.GasDischargeLampsAndLedLightSources)}" },
            { WeeeCategory.PhotovoltaicPanels, $"14. {GetCsvDisplayName(WeeeCategory.PhotovoltaicPanels)}" }
        };

        private static string GetCsvDisplayName(WeeeCategory category)
        {
            return category.ToDisplayString().Replace(",", string.Empty);
        }
    }
}
