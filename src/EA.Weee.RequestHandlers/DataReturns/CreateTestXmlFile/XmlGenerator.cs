namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml.Linq;
    using Domain;
    using Domain.Lookup;
    using Domain.Producer;
    using EA.Weee.Domain.DataReturns;
    using Prsd.Core;
    using Xml;
    using QuarterType = EA.Weee.Domain.DataReturns.QuarterType;

    public class XmlGenerator : IXmlGenerator
    {
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

            XElement xmlSchemeReturn = new XElement(XmlNamespace.DataReturns + "SchemeReturn");
            xmlDoc.Add(xmlSchemeReturn);

            PopulateSchemeReturn(dataReturnVersion, xmlSchemeReturn);

            return xmlDoc;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Field name deliveredToAE is valid.")]
        private void PopulateSchemeReturn(DataReturnVersion dataReturnVersion, XElement xmlSchemeReturn)
        {
            XElement xmlXsdVersion = new XElement(XmlNamespace.DataReturns + "XSDVersion");
            xmlSchemeReturn.Add(xmlXsdVersion);
            xmlXsdVersion.Value = "3.23";

            XElement xmlApprovalNo = new XElement(XmlNamespace.DataReturns + "ApprovalNo");
            xmlSchemeReturn.Add(xmlApprovalNo);
            xmlApprovalNo.Value = dataReturnVersion.DataReturn.Scheme.ApprovalNumber;

            XElement xmlComplianceYear = new XElement(XmlNamespace.DataReturns + "ComplianceYear");
            xmlSchemeReturn.Add(xmlComplianceYear);
            xmlComplianceYear.Value = dataReturnVersion.DataReturn.Quarter.Year.ToString();

            XElement xmlReturnPeriod = new XElement(XmlNamespace.DataReturns + "ReturnPeriod");
            xmlSchemeReturn.Add(xmlReturnPeriod);
            xmlReturnPeriod.Value = quarterTypeMapping[dataReturnVersion.DataReturn.Quarter.Q];

            XElement xmlCollectedFromDcf = new XElement(XmlNamespace.DataReturns + "CollectedFromDCF");
            xmlSchemeReturn.Add(xmlCollectedFromDcf);

            var fromDcf = dataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts.Where(w => w.SourceType == WeeeCollectedAmountSourceType.Dcf);
            foreach (IReturnItem returnItem in fromDcf)
            {
                XElement xmlReturn = new XElement(XmlNamespace.DataReturns + "Return");
                xmlCollectedFromDcf.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            var aatfDeliveredAmounts = dataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts
                .Where(x => x.IsAatfDeliveredAmount)
                .GroupBy(x => x.AatfDeliveryLocation);

            foreach (var aatfDeliveredAmount in aatfDeliveredAmounts)
            {
                XElement xmlDeliveredToATF = new XElement(XmlNamespace.DataReturns + "DeliveredToATF");
                xmlSchemeReturn.Add(xmlDeliveredToATF);

                PopulateDeliveredToAatf(aatfDeliveredAmount, xmlDeliveredToATF);
            }

            var aeDeliveredAmounts = dataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts
                .Where(x => x.IsAeDeliveredAmount)
                .GroupBy(x => x.AeDeliveryLocation);

            foreach (var aeDeliveredAmount in aeDeliveredAmounts)
            {
                XElement xmlDeliveredToAE = new XElement(XmlNamespace.DataReturns + "DeliveredToAE");
                xmlSchemeReturn.Add(xmlDeliveredToAE);

                PopulateDeliveredToAE(aeDeliveredAmount, xmlDeliveredToAE);
            }

            XElement xmlB2cWeeeFromDistributors = new XElement(XmlNamespace.DataReturns + "B2CWEEEFromDistributors");
            xmlSchemeReturn.Add(xmlB2cWeeeFromDistributors);

            var fromDistributors = dataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts.Where(w => w.SourceType == WeeeCollectedAmountSourceType.Distributor);

            foreach (IReturnItem returnItem in fromDistributors)
            {
                XElement xmlReturn = new XElement(XmlNamespace.DataReturns + "Return");
                xmlB2cWeeeFromDistributors.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            XElement xmlB2cWeeeFromFinalHolders = new XElement(XmlNamespace.DataReturns + "B2CWEEEFromFinalHolders");
            xmlSchemeReturn.Add(xmlB2cWeeeFromFinalHolders);

            var fromFinalHolders = dataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts.Where(w => w.SourceType == WeeeCollectedAmountSourceType.FinalHolder);
            foreach (IReturnItem returnItem in fromFinalHolders)
            {
                XElement xmlReturn = new XElement(XmlNamespace.DataReturns + "Return");
                xmlB2cWeeeFromFinalHolders.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }

            XElement xmlProducerList = new XElement(XmlNamespace.DataReturns + "ProducerList");
            xmlSchemeReturn.Add(xmlProducerList);

            var eeeOutputAmountsByProducers = dataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts.GroupBy(x => x.RegisteredProducer);
            foreach (var eeeOutputAmountsByProducer in eeeOutputAmountsByProducers)
            {
                XElement xmlProducer = new XElement(XmlNamespace.DataReturns + "Producer");
                xmlProducerList.Add(xmlProducer);

                PopulateProducer(eeeOutputAmountsByProducer.Key, eeeOutputAmountsByProducer, xmlProducer);
            }
        }

        private void PopulateDeliveredToAatf(IGrouping<AatfDeliveryLocation, WeeeDeliveredAmount> deliveredToAatfs, XElement xmlDeliveredToAtf)
        {
            XElement xmlDeliveredToFacility = new XElement(XmlNamespace.DataReturns + "DeliveredToFacility");
            xmlDeliveredToAtf.Add(xmlDeliveredToFacility);

            XElement xmlAatfApprovalNo = new XElement(XmlNamespace.DataReturns + "AATFApprovalNo");
            xmlDeliveredToFacility.Add(xmlAatfApprovalNo);
            xmlAatfApprovalNo.Value = deliveredToAatfs.Key.ApprovalNumber;

            XElement xmlFacilityName = new XElement(XmlNamespace.DataReturns + "FacilityName");
            xmlDeliveredToFacility.Add(xmlFacilityName);
            xmlFacilityName.Value = deliveredToAatfs.Key.FacilityName;

            foreach (IReturnItem returnItem in deliveredToAatfs)
            {
                XElement xmlReturn = new XElement(XmlNamespace.DataReturns + "Return");
                xmlDeliveredToAtf.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }
        }

        private void PopulateDeliveredToAE(IGrouping<AeDeliveryLocation, WeeeDeliveredAmount> deliveredToAes, XElement xmlDeliveredToAE)
        {
            XElement xmlDeliveredToOperator = new XElement(XmlNamespace.DataReturns + "DeliveredToOperator");
            xmlDeliveredToAE.Add(xmlDeliveredToOperator);

            XElement xmlAEApprovalNo = new XElement(XmlNamespace.DataReturns + "AEApprovalNo");
            xmlDeliveredToOperator.Add(xmlAEApprovalNo);
            xmlAEApprovalNo.Value = deliveredToAes.Key.ApprovalNumber;

            XElement xmlOperatorName = new XElement(XmlNamespace.DataReturns + "OperatorName");
            xmlDeliveredToOperator.Add(xmlOperatorName);
            xmlOperatorName.Value = deliveredToAes.Key.OperatorName;

            foreach (IReturnItem returnItem in deliveredToAes)
            {
                XElement xmlReturn = new XElement(XmlNamespace.DataReturns + "Return");
                xmlDeliveredToAE.Add(xmlReturn);

                PopulateReturn(returnItem, xmlReturn);
            }
        }

        private void PopulateProducer(RegisteredProducer registeredProducer, IEnumerable<EeeOutputAmount> eeeOutputAmounts, XElement xmlProducer)
        {
            XElement xmlRegistrationNo = new XElement(XmlNamespace.DataReturns + "RegistrationNo");
            xmlProducer.Add(xmlRegistrationNo);
            xmlRegistrationNo.Value = registeredProducer.ProducerRegistrationNumber;

            XElement xmlProducerCompanyName = new XElement(XmlNamespace.DataReturns + "ProducerCompanyName");
            xmlProducer.Add(xmlProducerCompanyName);
            xmlProducerCompanyName.Value = registeredProducer.CurrentSubmission.OrganisationName;

            foreach (var eeeOutputAmount in eeeOutputAmounts)
            {
                XElement xmlReturn = new XElement(XmlNamespace.DataReturns + "Return");
                xmlProducer.Add(xmlReturn);

                PopulateReturn(eeeOutputAmount, xmlReturn);
            }
        }

        private void PopulateReturn(IReturnItem returnItem, XElement xmlReturn)
        {
            XElement xmlCategoryName = new XElement(XmlNamespace.DataReturns + "CategoryName");
            xmlReturn.Add(xmlCategoryName);
            xmlCategoryName.Value = categoryMapping[returnItem.WeeeCategory];

            XElement xmlObligationType = new XElement(XmlNamespace.DataReturns + "ObligationType");
            xmlReturn.Add(xmlObligationType);
            xmlObligationType.Value = obligationTypeMapping[returnItem.ObligationType];

            XElement xmlTonnesReturnValue = new XElement(XmlNamespace.DataReturns + "TonnesReturnValue");
            xmlReturn.Add(xmlTonnesReturnValue);
            xmlTonnesReturnValue.Value = returnItem.Tonnage.ToString();
        }
    }
}
