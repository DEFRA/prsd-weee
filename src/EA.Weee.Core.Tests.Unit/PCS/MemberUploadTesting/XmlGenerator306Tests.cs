using EA.Weee.Core.PCS.MemberUploadTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace EA.Weee.Core.Tests.Unit.PCS.MemberUploadTesting
{
    public class XmlGenerator306Tests
    {
        [Fact]
        public void XmlGenerator306_WithValidEmptyProducerList_ReturnsCorrectXml()
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

            XmlGenerator306 xmlGenerator = new XmlGenerator306();

            // Act
            XDocument xDocument = xmlGenerator.GenerateXml(producerList);

            string actual;
            using (MemoryStream stream = new MemoryStream())
            {
                xDocument.Save(stream);
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

            Assert.Equal(expected, actual);
        }
    }
}
