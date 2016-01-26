namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.CreateTestXmlFile
{
    using System.Diagnostics.CodeAnalysis;
    using Domain;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Producer;
    using FakeItEasy;
    using RequestHandlers.DataReturns.CreateTestXmlFile;
    using Weee.Tests.Core.Xml;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class XmlGeneratorTests
    {
        [Fact]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Variable name aeDeliveryLocation is valid.")]
        public void GenerateXml_CreatesValidDataReturnsXmlFile()
        {
            // Arrange
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber)
                .Returns("WEE/SC0001ST/SCH");

            var dataReturn = new DataReturn(scheme, new Quarter(2016, QuarterType.Q2));
            var dataReturnVersion = new DataReturnVersion(dataReturn);

            // WEEE collected
            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2C, WeeeCategory.ElectricalAndElectronicTools, 100));
            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2B, WeeeCategory.DisplayEquipment, 200));

            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2C, WeeeCategory.ElectricalAndElectronicTools, 100));
            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2C, WeeeCategory.ITAndTelecommsEquipment, 50));

            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2C, WeeeCategory.ElectricalAndElectronicTools, 100));
            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2C, WeeeCategory.MedicalDevices, 2));

            // WEEE delivered
            var aatfDeliveryLocation1 = new AatfDeliveryLocation("WEE/AA0001AA/ATF", "TestAATF1");
            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(
                new WeeeDeliveredAmount(ObligationType.B2C, WeeeCategory.LargeHouseholdAppliances, 200, aatfDeliveryLocation1));
            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(
                new WeeeDeliveredAmount(ObligationType.B2C, WeeeCategory.PhotovoltaicPanels, 200, aatfDeliveryLocation1));

            var aatfDeliveryLocation2 = new AatfDeliveryLocation("WEE/AA0002AA/ATF", "TestAATF2");
            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(
                new WeeeDeliveredAmount(ObligationType.B2B, WeeeCategory.PhotovoltaicPanels, 200, aatfDeliveryLocation2));

            var aeDeliveryLocation1 = new AeDeliveryLocation("WEE/AA0001AA/AE", "TestAE1");
            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(
                new WeeeDeliveredAmount(ObligationType.B2C, WeeeCategory.LargeHouseholdAppliances, 200, aeDeliveryLocation1));
            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(
                new WeeeDeliveredAmount(ObligationType.B2B, WeeeCategory.LightingEquipment, 20, aeDeliveryLocation1));

            var aeDeliveryLocation2 = new AeDeliveryLocation("WEE/AA0002AA/AE", "TestAE2");
            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(
                new WeeeDeliveredAmount(ObligationType.B2B, WeeeCategory.LightingEquipment, 10, aeDeliveryLocation2));

            // EEE output
            var registeredProducer1 = CreateRegisteredProducer(scheme, 2016, "WEE/AA0001RP", "Test Organisation1");
            dataReturnVersion.EeeOutputReturnVersion.AddEeeOutputAmount(
                new EeeOutputAmount(ObligationType.B2C, WeeeCategory.LightingEquipment, 3000, registeredProducer1));
            dataReturnVersion.EeeOutputReturnVersion.AddEeeOutputAmount(
                new EeeOutputAmount(ObligationType.B2C, WeeeCategory.PhotovoltaicPanels, 100, registeredProducer1));

            var registeredProducer2 = CreateRegisteredProducer(scheme, 2016, "WEE/AA0002RP", "Test Organisation2");
            dataReturnVersion.EeeOutputReturnVersion.AddEeeOutputAmount(
                new EeeOutputAmount(ObligationType.B2C, WeeeCategory.PhotovoltaicPanels, 100, registeredProducer2));

            var xmlGenerator = new XmlGenerator();

            // Act
            var generatedXml = xmlGenerator.GenerateXml(dataReturnVersion);

            // Assert
            var xmlSchemaHelper = new XmlSchemaHelper(@"DataReturns\v3schema.xsd");
            var validationResult = xmlSchemaHelper.ValidateXml(generatedXml);

            Assert.Empty(validationResult);
        }

        private RegisteredProducer CreateRegisteredProducer(Scheme scheme, int complianceYear, string registrationNumber, string organisationName)
        {
            var registeredProducer = new RegisteredProducer(registrationNumber, complianceYear, scheme);

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.OrganisationName)
                .Returns(organisationName);
            A.CallTo(() => producerSubmission.RegisteredProducer)
                .Returns(registeredProducer);

            registeredProducer.SetCurrentSubmission(producerSubmission);

            return registeredProducer;
        }
    }
}
