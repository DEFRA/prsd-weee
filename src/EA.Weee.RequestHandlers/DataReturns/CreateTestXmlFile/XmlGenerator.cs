namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Domain;
    using Domain.Lookup;
    using EA.Weee.Domain.DataReturns;
    using Prsd.Core;

    public class XmlGenerator : IXmlGenerator
    {
        private static readonly XNamespace ns = "http://www.environment-agency.gov.uk/WEEE/XMLSchema/SchemeReturns";

        private static readonly Dictionary<QuarterType, string> quarterTypeMapping = new Dictionary<QuarterType, string>()
        {
            { QuarterType.Q1, "Quarter 1: January - March" },
            { QuarterType.Q2, "Quarter 2: April - June" },
            { QuarterType.Q3, "Quarter 3: July - September" },
            { QuarterType.Q4, "Quarter 4: October - December" },
        };

        private static readonly Dictionary<ObligationType, string> obligationTypeMapping = new Dictionary<ObligationType, string>()
        {
            { ObligationType.B2B, "B2B" },
            { ObligationType.B2C, "B2C" },
        };

        private static readonly Dictionary<Category, string> categoryMapping = new Dictionary<Category, string>()
        {
            { Category.LargeHouseholdAppliances, "Large Household Appliances" },
            { Category.SmallHouseholdAppliances, "Small Household Appliances" },
            { Category.ITAndTelecommsEquipment, "IT and Telecomms Equipment" },
            { Category.ConsumerEquipment, "Consumer Equipment" },
            { Category.LightingEquipment, "Lighting Equipment" },
            { Category.ElectricalAndElectronicTools, "Electrical and Electronic Tools" },
            { Category.ToysLeisureAndSports, "Toys Leisure and Sports" },
            { Category.MedicalDevices, "Medical Devices" },
            { Category.MonitoringAndControlInstruments, "Monitoring and Control Instruments" },
            { Category.AutomaticDispensers, "Automatic Dispensers" },
            { Category.DisplayEquipment, "Display Equipment" },
            { Category.CoolingApplicancesContainingRefrigerants, "Cooling Appliances Containing Refrigerants" },
            { Category.GasDischargeLampsAndLedLightSources, "Gas Discharge Lamps and LED light sources" },
            { Category.PhotovoltaicPanels, "Photovoltaic Panels" },
        };

        public XDocument GenerateXml(DataReturnVersion dataReturnVersion)
        {
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            XDocument xmlDoc = new XDocument();

            xmlDoc.Declaration = new XDeclaration("1.0", "utf-8", "yes");

            XElement xmlSchemeReturn = new XElement(ns + "SchemeReturn");
            xmlDoc.Add(xmlSchemeReturn);

            PopulateSchemeReturn(dataReturnVersion, xmlSchemeReturn);

            return xmlDoc;
        }

        private void PopulateSchemeReturn(DataReturnVersion dataReturnVersion, XElement xmlSchemeReturn)
        {
            XElement xmlXsdVersion = new XElement(ns + "XSDVersion");
            xmlSchemeReturn.Add(xmlXsdVersion);
            xmlXsdVersion.Value = "3.2";

            XElement xmlApprovalNo = new XElement(ns + "ApprovalNo");
            xmlSchemeReturn.Add(xmlApprovalNo);
            xmlApprovalNo.Value = dataReturnVersion.DataReturn.Scheme.ApprovalNumber;

            XElement xmlComplianceYear = new XElement(ns + "ComplianceYear");
            xmlSchemeReturn.Add(xmlComplianceYear);
            xmlComplianceYear.Value = dataReturnVersion.DataReturn.Quarter.Year.ToString();

            XElement xmlReturnPeriod = new XElement(ns + "ReturnPeriod");
            xmlSchemeReturn.Add(xmlReturnPeriod);
            xmlReturnPeriod.Value = quarterTypeMapping[dataReturnVersion.DataReturn.Quarter.Q];

            XElement xmlCollectedFromDcf = new XElement(ns + "CollectedFromDCF");
            xmlSchemeReturn.Add(xmlCollectedFromDcf);

            foreach (ReturnItem returnItem in dataReturnVersion.ReturnItemsCollectedFromDcf)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlCollectedFromDcf.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            foreach (DeliveredToAtf deliveredToAatf in dataReturnVersion.DeliveredToAatf)
            {
                XElement xmlDeliveredToATF = new XElement(ns + "DeliveredToATF");
                xmlSchemeReturn.Add(xmlDeliveredToATF);

                PopulateDeliveredToAatf(deliveredToAatf, xmlDeliveredToATF);
            }

            foreach (DeliveredToAe deliveredToAE in dataReturnVersion.DeliveredToAe)
            {
                XElement xmlDeliveredToAE = new XElement(ns + "DeliveredToAE");
                xmlSchemeReturn.Add(xmlDeliveredToAE);

                PopulateDeliveredToAE(deliveredToAE, xmlDeliveredToAE);
            }

            XElement xmlB2cWeeeFromDistributors = new XElement(ns + "B2CWEEEFromDistributors");
            xmlSchemeReturn.Add(xmlB2cWeeeFromDistributors);

            foreach (ReturnItem returnItem in dataReturnVersion.B2cWeeeFromDistributors)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlB2cWeeeFromDistributors.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            XElement xmlB2cWeeeFromFinalHolders = new XElement(ns + "B2CWEEEFromFinalHolders");
            xmlSchemeReturn.Add(xmlB2cWeeeFromFinalHolders);

            foreach (ReturnItem returnItem in dataReturnVersion.B2cWeeeFromFinalHolders)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlB2cWeeeFromFinalHolders.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            XElement xmlProducerList = new XElement(ns + "ProducerList");
            xmlSchemeReturn.Add(xmlProducerList);

            foreach (Producer producer in dataReturnVersion.Producers)
            {
                XElement xmlProducer = new XElement(ns + "Producer");
                xmlProducerList.Add(xmlProducer);

                PopulateProducer(producer, xmlProducer);
            }
        }

        private void PopulateDeliveredToAatf(DeliveredToAtf deliveredToAatf, XElement xmlDeliveredToAtf)
        {
            XElement xmlDeliveredToFacility = new XElement(ns + "DeliveredToFacility");
            xmlDeliveredToAtf.Add(xmlDeliveredToFacility);

            XElement xmlAatfApprovalNo = new XElement(ns + "AATFApprovalNo");
            xmlDeliveredToFacility.Add(xmlAatfApprovalNo);
            xmlAatfApprovalNo.Value = deliveredToAatf.AatfApprovalNumber;

            XElement xmlFacilityName = new XElement(ns + "FacilityName");
            xmlDeliveredToFacility.Add(xmlFacilityName);
            xmlFacilityName.Value = deliveredToAatf.FacilityName;

            foreach (ReturnItem returnItem in deliveredToAatf.ReturnItems)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlDeliveredToAtf.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }
        }

        private void PopulateDeliveredToAE(DeliveredToAe deliveredToAe, XElement xmlDeliveredToAE)
        {
            XElement xmlDeliveredToOperator = new XElement(ns + "DeliveredToOperator");
            xmlDeliveredToAE.Add(xmlDeliveredToOperator);

            XElement xmlAEApprovalNo = new XElement(ns + "AEApprovalNo");
            xmlDeliveredToOperator.Add(xmlAEApprovalNo);
            xmlAEApprovalNo.Value = deliveredToAe.ApprovalNumber;

            XElement xmlOperatorName = new XElement(ns + "OperatorName");
            xmlDeliveredToOperator.Add(xmlOperatorName);
            xmlOperatorName.Value = deliveredToAe.OperatorName;

            foreach (ReturnItem returnItem in deliveredToAe.ReturnItems)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlDeliveredToAE.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }
        }

        private void PopulateProducer(Producer producer, XElement xmlProducer)
        {
            XElement xmlRegistrationNo = new XElement(ns + "RegistrationNo");
            xmlProducer.Add(xmlRegistrationNo);
            xmlRegistrationNo.Value = producer.RegisteredProducer.ProducerRegistrationNumber;

            XElement xmlProducerCompanyName = new XElement(ns + "ProducerCompanyName");
            xmlProducer.Add(xmlProducerCompanyName);
            xmlProducerCompanyName.Value = producer.RegisteredProducer.CurrentSubmission.OrganisationName;

            foreach (ReturnItem returnItem in producer.ReturnItems)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlProducer.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }
        }

        private void PopulateReturn(ReturnItem returnItem, XElement xmlReturn)
        {
            XElement xmlCategoryName = new XElement(ns + "CategoryName");
            xmlReturn.Add(xmlCategoryName);
            xmlCategoryName.Value = categoryMapping[returnItem.Category];

            XElement xmlObligationType = new XElement(ns + "ObligationType");
            xmlReturn.Add(xmlObligationType);
            xmlObligationType.Value = obligationTypeMapping[returnItem.ObligationType];

            XElement xmlTonnesReturnValue = new XElement(ns + "TonnesReturnValue");
            xmlReturn.Add(xmlTonnesReturnValue);
            xmlTonnesReturnValue.Value = returnItem.AmountInTonnes.ToString();
        }
    }
}
