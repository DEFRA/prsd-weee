namespace EA.Weee.Core.Shared.CsvReading
{
    using Domain.Lookup;

    public class ObligationCsvUpload
    {
        public string SchemeIdentifier { get; set; }

        public string SchemeName { get; set; }

        [WeeeCategory(WeeeCategory.LargeHouseholdAppliances)]
        public string Cat1 { get; set; }

        [WeeeCategory(WeeeCategory.SmallHouseholdAppliances)]
        public string Cat2 { get; set; }

        [WeeeCategory(WeeeCategory.ITAndTelecommsEquipment)]
        public string Cat3 { get; set; }

        [WeeeCategory(WeeeCategory.ConsumerEquipment)]
        public string Cat4 { get; set; }

        [WeeeCategory(WeeeCategory.LightingEquipment)]
        public string Cat5 { get; set; }

        [WeeeCategory(WeeeCategory.ElectricalAndElectronicTools)]
        public string Cat6 { get; set; }

        [WeeeCategory(WeeeCategory.ToysLeisureAndSports)]
        public string Cat7 { get; set; }

        [WeeeCategory(WeeeCategory.MedicalDevices)]
        public string Cat8 { get; set; }

        [WeeeCategory(WeeeCategory.MonitoringAndControlInstruments)]
        public string Cat9 { get; set; }

        [WeeeCategory(WeeeCategory.AutomaticDispensers)]
        public string Cat10 { get; set; }

        [WeeeCategory(WeeeCategory.DisplayEquipment)]
        public string Cat11 { get; set; }

        [WeeeCategory(WeeeCategory.CoolingApplicancesContainingRefrigerants)]
        public string Cat12 { get; set; }

        [WeeeCategory(WeeeCategory.GasDischargeLampsAndLedLightSources)]
        public string Cat13 { get; set; }

        [WeeeCategory(WeeeCategory.PhotovoltaicPanels)]
        public string Cat14 { get; set; }
        
        public ObligationCsvUpload()
        {
        }
    }
}
