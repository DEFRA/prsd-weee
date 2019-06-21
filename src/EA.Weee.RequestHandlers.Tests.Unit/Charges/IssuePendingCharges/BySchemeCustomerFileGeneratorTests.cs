namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Charges;
    using Domain.Obligation;
    using Domain.Organisation;
    using Domain.Scheme;
    using Domain.User;
    using FakeItEasy;
    using Ibis;
    using RequestHandlers.Charges.IssuePendingCharges;
    using Xunit;
    using Address = Domain.Organisation.Address;
    using Organisation = Domain.Organisation.Organisation;

    public class BySchemeCustomerFileGeneratorTests
    {
        /// <summary>
        /// This test ensures that the customer file will be generated with the specified file ID.
        /// </summary>
        [Fact]
        public async Task CreateCustomerFile_WithFileID_CreatesFileWithCorrectFileID()
        {
            // Arrange
            var authority = A.Dummy<UKCompetentAuthority>();

            var address = new Address(
                "1 High Street",
                null,
                "Some town",
                "Some county",
                "Post code",
                new Country(Guid.NewGuid(), "UK - England"),
                "01234 567890",
                "someone@domain.com");

            var contact = new Contact("John", "Smith", "Manager");

            var organisation = Organisation.CreateSoleTrader("Test organisation");
            var scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);

            scheme.AddOrUpdateMainContactPerson(contact);
            scheme.AddOrUpdateAddress(address);

            var complianceYear = A.Dummy<int>();

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>());

            memberUpload.Submit(A.Dummy<User>());

            var memberUploads = new List<MemberUpload> { memberUpload };

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var generator = new BySchemeCustomerFileGenerator();
            ulong id = 12345;

            // Act
            var result = await generator.CreateAsync(id, invoiceRun);
            var customerFile = result.IbisFile;

            // Assert
            Assert.Equal((ulong)12345, customerFile.FileID);
        }

        /// <summary>
        /// This test ensures that creating a customer file from one member upload will
        /// result in one customer being added to the file; with the correct details of
        /// the scheme and their organisation's address.
        /// </summary>
        [Fact]
        public async Task CreateCustomerFile_WithOneMemberUpload_CreatesOneCustomer()
        {
            // Arrange
            var authority = A.Dummy<UKCompetentAuthority>();

            var address = new Address(
                "1 High Street",
                null,
                "Some town",
                "Some county",
                "Post code",
                new Country(Guid.NewGuid(), "UK - England"),
                "01234 567890",
                "someone@domain.com");

            var contact = new Contact("John", "Smith", "Manager");

            var organisation = Organisation.CreateSoleTrader("Test organisation");
            
            var scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);
            scheme.AddOrUpdateAddress(address);
            scheme.AddOrUpdateMainContactPerson(contact);

            var complianceYear = A.Dummy<int>();

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload.Submit(A.Dummy<User>());

            var memberUploads = new List<MemberUpload> { memberUpload };

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var generator = new BySchemeCustomerFileGenerator();

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            var customerFile = result.IbisFile;

            // Assert
            Assert.NotNull(customerFile);
            Assert.Equal(1, customerFile.Customers.Count);

            var customer = customerFile.Customers[0];

            Assert.NotNull(customer);
            Assert.Equal("WEE00000001", customer.CustomerReference);
            Assert.Equal("Test organisation", customer.Name);

            Assert.NotNull(customer.Address);
            Assert.Equal("John Smith", customer.Address.AddressLine1);
            Assert.Equal("1 High Street", customer.Address.AddressLine2);
            Assert.Equal(null, customer.Address.AddressLine3);
            Assert.Equal(null, customer.Address.AddressLine4);
            Assert.Equal("Some town", customer.Address.AddressLine5);
            Assert.Equal("Some county", customer.Address.AddressLine6);
            Assert.Equal("Post code", customer.Address.PostCode);
        }

        /// <summary>
        /// This test ensures that creating a customer file from two member uploads
        /// for the same scheme will result in one customer being added to the file.
        /// </summary>
        [Fact]
        public async Task CreateCustomerFile_WithTwoMemberUploadsForTheSameScheme_CreatesOneCustomer()
        {
            // Arrange
            var authority = A.Dummy<UKCompetentAuthority>();

            var address = new Address(
                "1 High Street",
                null,
                "Some town",
                "Some county",
                "Post code",
                new Country(Guid.NewGuid(), "UK - England"),
                "01234 567890",
                "someone@domain.com");

            var organisation = Organisation.CreateSoleTrader("Test organisation");

            var scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);
            scheme.AddOrUpdateAddress(address);
            scheme.AddOrUpdateMainContactPerson(A.Fake<Contact>());

            var complianceYear = A.Dummy<int>();

            var memberUpload1 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload1.Submit(A.Dummy<User>());

            var memberUpload2 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload2.Submit(A.Dummy<User>());

            var memberUploads = new List<MemberUpload> { memberUpload1, memberUpload2 };

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var generator = new BySchemeCustomerFileGenerator();

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            var customerFile = result.IbisFile;

            // Assert
            Assert.NotNull(customerFile);
            Assert.Equal(1, customerFile.Customers.Count);

            var customer = customerFile.Customers[0];
            Assert.NotNull(customer);
        }

        /// <summary>
        /// This test ensures that creating a customer file from two member uploads
        /// for different schemes will result in two customers being added to the file.
        /// </summary>
        [Fact]
        public async Task CreateCustomerFile_WithTwoMemberUploadsForDifferentSchemes_CreatesTwoCustomers()
        {
            // Arrange
            var authority = A.Dummy<UKCompetentAuthority>();

            var address1 = new Address(
                "1 High Street",
                null,
                "Some town",
                "Some county",
                "Post code",
                new Country(Guid.NewGuid(), "UK - England"),
                "01234 567890",
                "someone@domain.com");

            var organisation1 = Organisation.CreateSoleTrader("Test organisation 1");
            
            var scheme1 = new Scheme(organisation1);
            scheme1.UpdateScheme(
                "Test scheme 2",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);
            scheme1.AddOrUpdateMainContactPerson(A.Fake<Contact>());
            scheme1.AddOrUpdateAddress(address1);

            var complianceYear1 = A.Dummy<int>();

            var memberUpload1 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear1,
                scheme1,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload1.Submit(A.Dummy<User>());

            var address2 = new Address(
                "2 High Street",
                null,
                "Some town",
                "Some county",
                "Post code",
                new Country(Guid.NewGuid(), "UK - England"),
                "01234 567890",
                "someone@domain.com");

            var organisation2 = Organisation.CreateSoleTrader("Test organisation 2");
            
            var scheme2 = new Scheme(organisation2);
            scheme2.UpdateScheme(
                "Test scheme 2",
                "WEE/BB2222BB/SCH",
                "WEE00000002",
                A.Dummy<ObligationType>(),
                authority);
            scheme2.AddOrUpdateAddress(address2);
            scheme2.AddOrUpdateMainContactPerson(A.Fake<Contact>());

            var complianceYear2 = A.Dummy<int>();

            var memberUpload2 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear2,
                scheme2,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload2.Submit(A.Dummy<User>());

            var memberUploads = new List<MemberUpload> { memberUpload1, memberUpload2 };

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var generator = new BySchemeCustomerFileGenerator();

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            var customerFile = result.IbisFile;

            // Assert
            Assert.NotNull(customerFile);
            Assert.Equal(2, customerFile.Customers.Count);

            var customer1 = customerFile.Customers[0];
            Assert.NotNull(customer1);
            Assert.Equal("WEE00000001", customer1.CustomerReference);

            var customer2 = customerFile.Customers[1];
            Assert.NotNull(customer2);
            Assert.Equal("WEE00000002", customer2.CustomerReference);
        }

        [Fact]
        public async Task CreateCustomerFile_WithExceptionThrown_ReturnsError_AndNoCustomerFile()
        {
            // Arrange
            var authority = A.Dummy<UKCompetentAuthority>();

            var organisation = Organisation.CreateSoleTrader("Test organisation");
            
            var scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);

            var complianceYear = A.Dummy<int>();

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload.Submit(A.Dummy<User>());

            var memberUploads = new List<MemberUpload> { memberUpload };

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var generator = new BySchemeCustomerFileGenerator();

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);

            // Assert
            Assert.Null(result.IbisFile);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task CreateCustomerFile_WithNonUkAddress_ConcatenatesPostCodeAndCountry()
        {
            // Arrange
            var authority = A.Dummy<UKCompetentAuthority>();

            var address = new Address(
                "1 High Street",
                null,
                "Some town",
                "Some county",
                "AABB",
                new Country(Guid.NewGuid(), "Netherlands"),
                "01234 567890",
                "someone@domain.com");

            var contact = new Contact("John", "Smith", "Manager");

            var organisation = Organisation.CreateSoleTrader("Test organisation");
            
            var scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);
            scheme.AddOrUpdateMainContactPerson(contact);
            scheme.AddOrUpdateAddress(address);

            var complianceYear = A.Dummy<int>();

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload.Submit(A.Dummy<User>());

            var memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var generator = new BySchemeCustomerFileGenerator();

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            var customerFile = result.IbisFile;

            // Assert
            Assert.NotNull(customerFile);
            Assert.Equal(1, customerFile.Customers.Count);

            var customer = customerFile.Customers[0];
            Assert.Equal("AABB  Netherlands", customer.Address.PostCode);
        }

        [Theory]
        [InlineData("AABB", "UK - England", "AABB")]
        [InlineData("AABB", "Netherlands", "AABB  Netherlands")]
        [InlineData("  AABB  ", "Netherlands", "AABB  Netherlands")]
        [InlineData(null, "UK - England", null)]
        [InlineData(null, "Netherlands", null)]
        public void GetIbisPostCode_WithNonUkAddress_ConcatenatesPostCodeAndCountry(string postCode, string countryName, string expectedResult)
        {
            var address = new Address(
                "1 High Street",
                null,
                "Some town",
                "Some county",
                postCode,
                new Country(Guid.NewGuid(), countryName),
                "01234 567890",
                "someone@domain.com");

            var generator = new BySchemeCustomerFileGenerator();

            // Act
            var result = generator.GetIbisPostCode(address);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
