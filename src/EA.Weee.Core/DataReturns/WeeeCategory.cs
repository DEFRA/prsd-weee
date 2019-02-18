namespace EA.Weee.Core.DataReturns
{
    using System.ComponentModel.DataAnnotations;

    public enum WeeeCategory
    {
        [Display(Name = "Large household appliances")]
        LargeHouseholdAppliances = 1,

        [Display(Name = "Small household appliances")]
        SmallHouseholdAppliances = 2,

        [Display(Name = "IT and telecommunications equipment")]
        ITAndTelecommsEquipment = 3,

        [Display(Name = "Consumer equipment")]
        ConsumerEquipment = 4,

        [Display(Name = "Lighting equipment")]
        LightingEquipment = 5,

        [Display(Name = "Electrical and electronic tools")]
        ElectricalAndElectronicTools = 6,

        [Display(Name = "Toys, leisure and sports equipment")]
        ToysLeisureAndSports = 7,

        [Display(Name = "Medical devices")]
        MedicalDevices = 8,

        [Display(Name = "Monitoring and control instruments")]
        MonitoringAndControlInstruments = 9,

        [Display(Name = "Automatic dispensers")]
        AutomaticDispensers = 10,

        [Display(Name = "Display equipment")]
        DisplayEquipment = 11,

        [Display(Name = "Appliances containing refrigerants")]
        CoolingApplicancesContainingRefrigerants = 12,

        [Display(Name = "Gas discharge lamps and LED light sources")]
        GasDischargeLampsAndLedLightSources = 13,

        [Display(Name = "Photovoltaic panels")]
        PhotovoltaicPanels = 14
    }
}
