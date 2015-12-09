namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum Category
    {
        [Display(Name = "Large Household Appliances")]
        LargeHouseholdAppliances = 0,

        [Display(Name = "Small Household Appliances")]
        SmallHouseholdAppliances = 1,

        [Display(Name = "IT and Telecomms Equipment")]
        ITAndTelecommsEquipment = 2,

        [Display(Name = "Consumer Equipment")]
        ConsumerEquipment = 3,

        [Display(Name = "Lighting Equipment")]
        LightingEquipment = 4,

        [Display(Name = "Electrical and Electronic Tools")]
        ElectricalAndElectronicTools = 5,

        [Display(Name = "Toys Leisure and Sports")]
        ToysLeisureAndSports = 6,

        [Display(Name = "Medical Devices")]
        MedicalDevices = 7,

        [Display(Name = "Monitoring and Control Instruments")]
        MonitoringAndControlInstruments = 8,

        [Display(Name = "Automatic Dispensers")]
        AutomaticDispensers = 9,

        [Display(Name = "Display Equipment")]
        DisplayEquipment = 10,

        [Display(Name = "Cooling Appliances Containing Refrigerants")]
        CoolingApplicancesContainingRefrigerants = 11,

        [Display(Name = "Gas Discharge Lamps and LED light sources")]
        GasDischargeLampsAndLedLightSources = 12,

        [Display(Name = "Photovoltaic Panels")]
        PhotovoltaicPanels = 13,
    }
}
