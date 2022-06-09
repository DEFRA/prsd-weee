namespace EA.Weee.Core.Shared.CsvReading
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Domain.Lookup;

    public class ObligationCsvUpload
    {
        private string schemeIdentifier;

        public string SchemeIdentifier
        {
            get => schemeIdentifier.Trim();
            set => schemeIdentifier = value;
        }

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

        private static readonly Dictionary<WeeeCategory, PropertyInfo> WeeeCategoryProperties;

        static ObligationCsvUpload()
        {
            WeeeCategoryProperties = new Dictionary<WeeeCategory, PropertyInfo>();

            var obligationCsvProperties = typeof(ObligationCsvUpload).GetProperties();
            foreach (var prop in obligationCsvProperties)
            {
                var weeeCategoryAttributes = prop.GetCustomAttributes(typeof(WeeeCategoryAttribute), true);
                if (weeeCategoryAttributes.Any())
                {
                    if (weeeCategoryAttributes[0] is WeeeCategoryAttribute weeeCategoryAttribute)
                    {
                        WeeeCategoryProperties.Add(weeeCategoryAttribute.Category, prop);
                    }
                }
            }
        }

        public decimal? GetValue(WeeeCategory category)
        {
            WeeeCategoryProperties.TryGetValue(category, out var propertyInfoValue);

            if (propertyInfoValue != null)
            {
                var propertyValue = propertyInfoValue.GetValue(this).ToString();

                if (string.IsNullOrWhiteSpace(propertyValue))
                {
                    return null;
                }

                return decimal.Parse(propertyValue);
            }

            return null;
        }
    }
}
