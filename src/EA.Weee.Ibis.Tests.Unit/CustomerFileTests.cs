﻿namespace EA.Weee.Ibis.Tests.Unit
{
    using EA.Prsd.Core;
    using System;
    using Xunit;

    public class CustomerFileTests
    {
        [Fact]
        public void Write_WithOneCustomer_GeneratesCorrectOutput()
        {
            // Arrange
            Address address = new Address("1 High Street", "Flat 123", "Business Park", "Newtown", "Nr Oldtown", "Testshire", "AA1 1AA");
            Customer customer = new Customer("WEE0001", "Test customer", address);

            CustomerFile file = new CustomerFile("WEE", 0);

            file.AddCustomer(customer);

            // Act
            string result = file.Write();

            // Assert
            string expectedDate = SystemTime.UtcNow.Date.ToString("dd-MMM-yyyy").ToUpperInvariant();
            string expectedOutput =
                "\"H\",\"0000000\",\"WEE\",\"H\",\"C\",\"00000\",\"" + expectedDate + "\"" + Environment.NewLine +
                "\"D\",\"0000001\",\"WEE0001\",\"Test customer\",\"1 High Street\",\"Flat 123\",\"Business Park\",\"Newtown\",\"Nr Oldtown\",\"Testshire\",\"AA1 1AA\"" + Environment.NewLine +
                "\"T\",\"0000002\",\"0000003\"" + Environment.NewLine;

            Assert.Equal(expectedOutput, result);
        }
    }
}
