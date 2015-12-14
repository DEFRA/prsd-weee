namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Domain;
    using Domain.Lookup;
    using Domain.Producer;
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

        private static readonly Dictionary<WeeeCategory, string> categoryMapping = new Dictionary<WeeeCategory, string>()
        {
            { WeeeCategory.LargeHouseholdAppliances, "Large Household Appliances" },
            { WeeeCategory.SmallHouseholdAppliances, "Small Household Appliances" },
            { WeeeCategory.ITAndTelecommsEquipment, "IT and Telecomms Equipment" },
            { WeeeCategory.ConsumerEquipment, "Consumer Equipment" },
            { WeeeCategory.LightingEquipment, "Lighting Equipment" },
            { WeeeCategory.ElectricalAndElectronicTools, "Electrical and Electronic Tools" },
            { WeeeCategory.ToysLeisureAndSports, "Toys Leisure and Sports" },
            { WeeeCategory.MedicalDevices, "Medical Devices" },
            { WeeeCategory.MonitoringAndControlInstruments, "Monitoring and Control Instruments" },
            { WeeeCategory.AutomaticDispensers, "Automatic Dispensers" },
            { WeeeCategory.DisplayEquipment, "Display Equipment" },
            { WeeeCategory.CoolingApplicancesContainingRefrigerants, "Cooling Appliances Containing Refrigerants" },
            { WeeeCategory.GasDischargeLampsAndLedLightSources, "Gas Discharge Lamps and LED light sources" },
            { WeeeCategory.PhotovoltaicPanels, "Photovoltaic Panels" },
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

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Field name deliveredToAE is valid.")]
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

            var fromDcf = dataReturnVersion.WeeeCollectedAmounts.Where(x => x.SourceType == WeeeCollectedAmountSourceType.Dcf);
            foreach (IReturnItem returnItem in fromDcf)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlCollectedFromDcf.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            var aatfDeliveredAmounts = dataReturnVersion.AatfDeliveredAmounts.GroupBy(x => x.AatfDeliveryLocation);
            foreach (var aatfDeliveredAmount in aatfDeliveredAmounts)
            {
                XElement xmlDeliveredToATF = new XElement(ns + "DeliveredToATF");
                xmlSchemeReturn.Add(xmlDeliveredToATF);

                PopulateDeliveredToAatf(aatfDeliveredAmount, xmlDeliveredToATF);
            }

            var aeDeliveredAmounts = dataReturnVersion.AeDeliveredAmounts.GroupBy(x => x.AeDeliveryLocation);
            foreach (var aeDeliveredAmount in aeDeliveredAmounts)
            {
                XElement xmlDeliveredToAE = new XElement(ns + "DeliveredToAE");
                xmlSchemeReturn.Add(xmlDeliveredToAE);

                PopulateDeliveredToAE(aeDeliveredAmount, xmlDeliveredToAE);
            }

            XElement xmlB2cWeeeFromDistributors = new XElement(ns + "B2CWEEEFromDistributors");
            xmlSchemeReturn.Add(xmlB2cWeeeFromDistributors);

            var fromDistributors = dataReturnVersion.WeeeCollectedAmounts.Where(x => x.SourceType == WeeeCollectedAmountSourceType.Distributor);
            foreach (IReturnItem returnItem in fromDistributors)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlB2cWeeeFromDistributors.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            XElement xmlB2cWeeeFromFinalHolders = new XElement(ns + "B2CWEEEFromFinalHolders");
            xmlSchemeReturn.Add(xmlB2cWeeeFromFinalHolders);

            var fromFinalHolders = dataReturnVersion.WeeeCollectedAmounts.Where(x => x.SourceType == WeeeCollectedAmountSourceType.FinalHolder);
            foreach (IReturnItem returnItem in fromFinalHolders)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlB2cWeeeFromFinalHolders.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            XElement xmlProducerList = new XElement(ns + "ProducerList");
            xmlSchemeReturn.Add(xmlProducerList);

            var eeeOutputAmountsByProducers = dataReturnVersion.EeeOutputAmounts.GroupBy(x => x.RegisteredProducer);
            foreach (var eeeOutputAmountsByProducer in eeeOutputAmountsByProducers)
            {
                XElement xmlProducer = new XElement(ns + "Producer");
                xmlProducerList.Add(xmlProducer);

                PopulateProducer(eeeOutputAmountsByProducer.Key, eeeOutputAmountsByProducer, xmlProducer);
            }
        }

        private void PopulateDeliveredToAatf(IGrouping<AatfDeliveryLocation, AatfDeliveredAmount> deliveredToAatfs, XElement xmlDeliveredToAtf)
        {
            XElement xmlDeliveredToFacility = new XElement(ns + "DeliveredToFacility");
            xmlDeliveredToAtf.Add(xmlDeliveredToFacility);

            XElement xmlAatfApprovalNo = new XElement(ns + "AATFApprovalNo");
            xmlDeliveredToFacility.Add(xmlAatfApprovalNo);
            xmlAatfApprovalNo.Value = deliveredToAatfs.Key.AatfApprovalNumber;

            XElement xmlFacilityName = new XElement(ns + "FacilityName");
            xmlDeliveredToFacility.Add(xmlFacilityName);
            xmlFacilityName.Value = deliveredToAatfs.Key.FacilityName;

            foreach (IReturnItem returnItem in deliveredToAatfs)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlDeliveredToAtf.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }
        }

        private void PopulateDeliveredToAE(IGrouping<AeDeliveryLocation, AeDeliveredAmount> deliveredToAes, XElement xmlDeliveredToAE)
        {
            XElement xmlDeliveredToOperator = new XElement(ns + "DeliveredToOperator");
            xmlDeliveredToAE.Add(xmlDeliveredToOperator);

            XElement xmlAEApprovalNo = new XElement(ns + "AEApprovalNo");
            xmlDeliveredToOperator.Add(xmlAEApprovalNo);
            xmlAEApprovalNo.Value = deliveredToAes.Key.ApprovalNumber;

            XElement xmlOperatorName = new XElement(ns + "OperatorName");
            xmlDeliveredToOperator.Add(xmlOperatorName);
            xmlOperatorName.Value = deliveredToAes.Key.OperatorName;

            foreach (IReturnItem returnItem in deliveredToAes)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlDeliveredToAE.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }
        }

        private void PopulateProducer(RegisteredProducer registeredProducer, IEnumerable<EeeOutputAmount> eeeOutputAmounts, XElement xmlProducer)
        {
            XElement xmlRegistrationNo = new XElement(ns + "RegistrationNo");
            xmlProducer.Add(xmlRegistrationNo);
            xmlRegistrationNo.Value = registeredProducer.ProducerRegistrationNumber;

            XElement xmlProducerCompanyName = new XElement(ns + "ProducerCompanyName");
            xmlProducer.Add(xmlProducerCompanyName);
            xmlProducerCompanyName.Value = registeredProducer.CurrentSubmission.OrganisationName;

            foreach (var eeeOutputAmount in eeeOutputAmounts)
            {
                XElement xmlReturn = new XElement(ns + "Return");
                xmlProducer.Add(xmlReturn);

                PopulateReturn(eeeOutputAmount, xmlReturn);
            }
        }

        private void PopulateReturn(IReturnItem returnItem, XElement xmlReturn)
        {
            XElement xmlCategoryName = new XElement(ns + "CategoryName");
            xmlReturn.Add(xmlCategoryName);
            xmlCategoryName.Value = categoryMapping[returnItem.WeeeCategory];

            XElement xmlObligationType = new XElement(ns + "ObligationType");
            xmlReturn.Add(xmlObligationType);
            xmlObligationType.Value = obligationTypeMapping[returnItem.ObligationType];

            XElement xmlTonnesReturnValue = new XElement(ns + "TonnesReturnValue");
            xmlReturn.Add(xmlTonnesReturnValue);
            xmlTonnesReturnValue.Value = returnItem.Tonnage.ToString();
        }
    }
}
