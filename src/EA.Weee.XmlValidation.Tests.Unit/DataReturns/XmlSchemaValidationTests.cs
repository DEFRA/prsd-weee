namespace EA.Weee.XmlValidation.Tests.Unit.DataReturns
{
    using System.Xml.Schema;
    using Weee.Tests.Core.Xml;
    using Xunit;

    /// <summary>
    /// Includes tests to verify that the validation elements and constraints added to the data returns schema work as intended.
    /// </summary>
    public class XmlSchemaValidationTests
    {
        private const string schemaFile = @"DataReturns\v3schema.xsd";

        [Fact]
        public void DataReturnSchemaValidation_CollectedFromDCF_ContainsDuplicateCategoryObligationType_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-CollectedFromDCF-DuplicateCategoryObligationType.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueCategoryCollectedFromDCF", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_DeliveredToATF_ContainsDuplicateCategoryObligationType_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-DeliveredToATF-DuplicateCategoryObligationType.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueCategoryDeliveredToATF", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_DeliveredToAE_ContainsDuplicateCategoryObligationType_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-DeliveredToAE-DuplicateCategoryObligationType.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueCategoryObligationDeliveredToAE", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_B2CWEEEFromDistributors_ContainsDuplicateCategoryObligationType_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-B2CWEEEFromDistributors-DuplicateCategoryObligationType.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueCategoryObligationB2CWEEEFromDistributors", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_B2CWEEEFromFinalHolders_ContainsDuplicateCategoryObligationType_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-B2CWEEEFromFinalHolders-DuplicateCategoryObligationType.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueCategoryObligationB2CWEEEFromFinalHolders", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_ProducerList_ContainsDuplicateCategoryObligationType_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-ProducerList-DuplicateCategoryObligationType.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueCategoryObligationProducerReturn", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_DeliveredToATF_ContainsDuplicateAATFApprovalNo_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-DeliveredToATF-DuplicateAATFApprovalNo.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueAATFApprovalNo", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_DeliveredToAE_ContainsDuplicateAEApprovalNo_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-DeliveredToAE-DuplicateAEApprovalNo.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueAEApprovalNo", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_ProducerList_ContainsDuplicateRegistrationNo_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-ProducerList-DuplicateRegistrationNo.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("uniqueProducerRegistrationNo", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_B2CWEEEFromDistributors_ContainsReturnWithB2BObligationType_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-B2CWEEEFromDistributors-ReturnWithB2BObligationType.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Equal("The value of the 'http://www.environment-agency.gov.uk/WEEE/XMLSchema/SchemeReturns:ObligationType' element does not equal its fixed value.", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_B2CWEEEFromFinalHolders_ContainsReturnWithB2BObligationType_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-B2CWEEEFromFinalHolders-ReturnWithB2BObligationType.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Equal("The value of the 'http://www.environment-agency.gov.uk/WEEE/XMLSchema/SchemeReturns:ObligationType' element does not equal its fixed value.", result.Message);
        }

        [Fact]
        public void DataReturnSchemaValidation_TonnageWithMoreThan25Digits_ReturnsValidationError()
        {
            var result = new XmlSchemaHelper(schemaFile).ValidateXmlWithSingleResult(@"DataReturns\XmlFiles\v3-Tonnage-WithMoreThan25Digits.xml");

            Assert.Equal(XmlSeverityType.Error, result.Severity);
            Assert.Contains("TotalDigits constraint failed", result.Message);
        }
    }
}
