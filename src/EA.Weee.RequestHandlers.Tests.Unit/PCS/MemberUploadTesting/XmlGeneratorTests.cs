namespace EA.Weee.RequestHandlers.Tests.Unit.PCS.MemberUploadTesting
{
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using EA.Weee.RequestHandlers.PCS.MemberUploadTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xunit;

    public class XmlGeneratorTests
    {
        [Fact]
        [Trait("Area", "PCS Member Upload Testing")]
        public void XmlGenerator_WithValidEmptyProducerList_ReturnsCorrectXml()
        {
            // Arrange
            ProducerList producerList = new ProducerList();
            producerList.ApprovalNumber = "Approval Number";
            producerList.ComplianceYear = 2015;
            producerList.SchemaVersion = SchemaVersion.Version_3_06;
            producerList.SchemeBusiness.Company = new SchemeCompany();
            producerList.SchemeBusiness.Company.CompanyName = "Company Name";
            producerList.SchemeBusiness.Company.CompanyNumber = "Company Number";
            producerList.TradingName = "Trading Name";

            XmlGenerator xmlGenerator = new XmlGenerator();

            // Act
            XDocument xmlDocument = xmlGenerator.GenerateXml(producerList);

            string actual;
            using (MemoryStream stream = new MemoryStream())
            {
                xmlDocument.Save(stream);
                stream.Position = 0;
                using (TextReader tr = new StreamReader(stream))
                {
                    actual = tr.ReadToEnd();
                }
            }

            // Assert
            string expected =
@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<scheme xmlns=""http://www.environment-agency.gov.uk/WEEE/XMLSchema"">
  <XSDVersion>3.06</XSDVersion>
  <approvalNo>Approval Number</approvalNo>
  <complianceYear>2015</complianceYear>
  <tradingName>Trading Name</tradingName>
  <schemeBusiness>
    <company>
      <companyName>Company Name</companyName>
      <companyNumber>Company Number</companyNumber>
    </company>
  </schemeBusiness>
  <producerList />
</scheme>";

            Xunit.Assert.Equal(expected, actual);
        }
    }
}
