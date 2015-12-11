namespace EA.Weee.Domain.Lookup
{
    using System.ComponentModel.DataAnnotations;

    public enum Category
    {
        [Display(Name = "Large Household Appliances")]
        LargeHouseholdAppliances = 1,

        [Display(Name = "Small Household Appliances")]
        SmallHouseholdAppliances = 2,

        [Display(Name = "IT and Telecomms Equipment")]
        ITAndTelecommsEquipment = 3,

        [Display(Name = "Consumer Equipment")]
        ConsumerEquipment = 4,

        [Display(Name = "Lighting Equipment")]
        LightingEquipment = 5,

        [Display(Name = "Electrical and Electronic Tools")]
        ElectricalAndElectronicTools = 6,

        [Display(Name = "Toys Leisure and Sports")]
        ToysLeisureAndSports = 7,

        [Display(Name = "Medical Devices")]
        MedicalDevices = 8,

        [Display(Name = "Monitoring and Control Instruments")]
        MonitoringAndControlInstruments = 9,

        [Display(Name = "Automatic Dispensers")]
        AutomaticDispensers = 10,

        [Display(Name = "Display Equipment")]
        DisplayEquipment = 11,

        [Display(Name = "Cooling Appliances Containing Refrigerants")]
        CoolingApplicancesContainingRefrigerants = 12,

        [Display(Name = "Gas Discharge Lamps and LED light sources")]
        GasDischargeLampsAndLedLightSources = 13,

        [Display(Name = "Photovoltaic Panels")]
        PhotovoltaicPanels = 14,
    }
}
