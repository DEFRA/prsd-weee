namespace EA.Weee.Ibis.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class TransactionFileTests
    {
        [Fact]
        public void Write_WithOneInvoiceAndTwoLineItems_GeneratesCorrectOutput()
        {
            // Arrange
            List<InvoiceLineItem> lineItems = new List<InvoiceLineItem>();
            lineItems.Add(new InvoiceLineItem(100.00m, "Test item 1"));
            lineItems.Add(new InvoiceLineItem(123.45m, "Test item 2"));

            Invoice invoice = new Invoice(
                "WEE0001",
                new DateTime(2015, 1, 1),
                TransactionType.Invoice,
                "WEE741804H",
                lineItems);

            TransactionFile file = new TransactionFile("WEE", 0);

            file.AddInvoice(invoice);

            // Act
            string result = file.Write();

            // Assert
            string expectedDate = DateTime.UtcNow.Date.ToString("dd-MMM-yyyy").ToUpperInvariant();
            string expectedOutput =
                "\"H\",\"0000000\",\"WEE\",\"H\",\"I\",\"0\",\"\",\"" + expectedDate + "\"" + Environment.NewLine +
                "\"D\",\"0000001\",\"WEE0001\",\"01-JAN-2015\",\"I\",\"WEE741804H\",\"\",\"GBP\",\"\",\"01-JAN-2015\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\","
                    + "\"10000\",\"\",\"H\",\"Test item 1\",\"W\",\"H\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"1\",\"Each\",\"10000\"" + Environment.NewLine +
                "\"D\",\"0000002\",\"WEE0001\",\"01-JAN-2015\",\"I\",\"WEE741804H\",\"\",\"GBP\",\"\",\"01-JAN-2015\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\","
                    + "\"12345\",\"\",\"H\",\"Test item 2\",\"W\",\"H\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"1\",\"Each\",\"12345\"" + Environment.NewLine +
                "\"T\",\"0000003\",\"4\",\"22345\",\"0\"" + Environment.NewLine;

            Assert.Equal(expectedOutput, result);
        }
    }
}
