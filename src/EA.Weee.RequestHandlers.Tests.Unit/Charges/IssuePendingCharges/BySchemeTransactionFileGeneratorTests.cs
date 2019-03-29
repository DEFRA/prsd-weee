namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Charges;
    using Domain.Obligation;
    using Domain.Scheme;
    using Domain.User;
    using FakeItEasy;
    using Ibis;
    using Prsd.Core;
    using RequestHandlers.Charges.IssuePendingCharges;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class BySchemeTransactionFileGeneratorTests
    {
        private readonly ITransactionReferenceGenerator transactionReferenceGenerator;
        private readonly BySchemeTransactionFileGenerator generator;

        public BySchemeTransactionFileGeneratorTests()
        {
            transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();

            generator = new BySchemeTransactionFileGenerator(transactionReferenceGenerator);
        }

        /// <summary>
        /// This test ensures that the transaction file will be generated with the specified file ID.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithFileID_CreatesFileWithCorrectFileID()
        {
            // Arrange
            UKCompetentAuthority authority = A.Dummy<UKCompetentAuthority>();

            Organisation organisation = Organisation.CreateSoleTrader("Test organisation");

            Scheme scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);

            int complianceYear = A.Dummy<int>();

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                123.45m,
                complianceYear,
                scheme,
                A.Dummy<string>());

            memberUpload.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            ulong id = 12345;

            // Act
            var result = await generator.CreateAsync(id, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.Equal((ulong)12345, transactionFile.FileID);
        }

        /// <summary>
        /// This test ensures that the transaction file will populate invoices and line items with the
        /// correct details from the member upload and scheme.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithOneMemberUpload_CreatesFileWithOneInvoiceWithOneLineItemWithCorrectDetails()
        {
            // Arrange
            UKCompetentAuthority authority = A.Dummy<UKCompetentAuthority>();

            Organisation organisation = Organisation.CreateSoleTrader("Test organisation");

            Scheme scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);

            int complianceYear = A.Dummy<int>();

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                123.45m,
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            SystemTime.Freeze(new DateTime(2015, 1, 1));
            memberUpload.Submit(A.Dummy<User>());
            SystemTime.Unfreeze();

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            SystemTime.Freeze(new DateTime(2015, 12, 31));
            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());
            SystemTime.Unfreeze();

            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Equal(1, transactionFile.Invoices.Count);

            Invoice invoice = transactionFile.Invoices[0];
            Assert.NotNull(invoice);

            Assert.Equal(TransactionType.Invoice, invoice.TransactionType);
            Assert.Equal("WEE800001H", invoice.TransactionReference);
            Assert.Equal(123.45m, invoice.TransactionTotal);
            Assert.Equal(null, invoice.TransactionHeaderNarrative);
            Assert.Equal(new DateTime(2015, 12, 31), invoice.TransactionDate);
            Assert.Equal(null, invoice.RelatedTransactionReference);
            Assert.Equal(CurrencyCode.GBP, invoice.CurrencyCode);
            Assert.Equal("WEE00000001", invoice.CustomerReference);

            Assert.NotNull(invoice.LineItems);
            Assert.Equal(1, invoice.LineItems.Count);

            InvoiceLineItem lineItem = invoice.LineItems[0];
            Assert.NotNull(lineItem);

            Assert.Equal(123.45m, lineItem.AmountExcludingVAT);
            Assert.Equal("Charge for producer registration submission made on 01 Jan 2015.", lineItem.Description);
            Assert.Equal("H", lineItem.AreaCode);
            Assert.Equal("H", lineItem.ContextCode);
            Assert.Equal("W", lineItem.IncomeStreamCode);
            Assert.Equal((ulong)1, lineItem.Quantity);
            Assert.Equal(UnitOfMeasure.Each, lineItem.UnitOfMeasure);
            Assert.Equal(null, lineItem.VatCode);
        }

        /// <summary>
        /// This test ensures that multiple member uploads for the same scheme will be grouped
        /// into a single invoice, with each member upload being represented by a line item.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithTwoMemberUploadsForTheSameScheme_CreatesFileWithOneInvoiceWithTwoLineItems()
        {
            // Arrange
            UKCompetentAuthority authority = A.Dummy<UKCompetentAuthority>();

            Organisation organisation = Organisation.CreateSoleTrader("Test organisation");

            Scheme scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);

            int complianceYear = A.Dummy<int>();

            MemberUpload memberUpload1 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                100,
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload1.Submit(A.Dummy<User>());

            MemberUpload memberUpload2 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                200,
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload2.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload1);
            memberUploads.Add(memberUpload2);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Equal(1, transactionFile.Invoices.Count);

            Invoice invoice = transactionFile.Invoices[0];
            Assert.NotNull(invoice);

            Assert.Equal(300, invoice.TransactionTotal);

            Assert.NotNull(invoice.LineItems);
            Assert.Equal(2, invoice.LineItems.Count);

            InvoiceLineItem lineItem1 = invoice.LineItems[0];
            Assert.NotNull(lineItem1);
            Assert.Equal(100, lineItem1.AmountExcludingVAT);

            InvoiceLineItem lineItem2 = invoice.LineItems[1];
            Assert.NotNull(lineItem2);
            Assert.Equal(200, lineItem2.AmountExcludingVAT);
        }

        /// <summary>
        /// This test ensures that member uploads for different schemes will not be grouped.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithTwoMemberUploadsForDifferentSchemes_CreatesFileWithTwoInvoices()
        {
            // Arrange
            UKCompetentAuthority authority = A.Dummy<UKCompetentAuthority>();
            int complianceYear = A.Dummy<int>();

            Organisation organisation1 = Organisation.CreateSoleTrader("Test organisation");

            Scheme scheme1 = new Scheme(organisation1);
            scheme1.UpdateScheme(
                "Test scheme 1",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);

            MemberUpload memberUpload1 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                100,
                complianceYear,
                scheme1,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload1.Submit(A.Dummy<User>());

            Organisation organisation2 = Organisation.CreateSoleTrader("Test organisation");

            Scheme scheme2 = new Scheme(organisation2);
            scheme2.UpdateScheme(
                "Test scheme 2",
                "WEE/BB2222BB/SCH",
                "WEE00000002",
                A.Dummy<ObligationType>(),
                authority);

            MemberUpload memberUpload2 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                200,
                complianceYear,
                scheme2,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload2.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload1);
            memberUploads.Add(memberUpload2);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Equal(2, transactionFile.Invoices.Count);

            Invoice invoice1 = transactionFile.Invoices[0];
            Assert.NotNull(invoice1);
            Assert.Equal(100, invoice1.TransactionTotal);

            Assert.NotNull(invoice1.LineItems);
            Assert.Equal(1, invoice1.LineItems.Count);

            InvoiceLineItem lineItem1 = invoice1.LineItems[0];
            Assert.NotNull(lineItem1);
            Assert.Equal(100, lineItem1.AmountExcludingVAT);

            Invoice invoice2 = transactionFile.Invoices[1];
            Assert.NotNull(invoice2);
            Assert.Equal(200, invoice2.TransactionTotal);

            Assert.NotNull(invoice2.LineItems);
            Assert.Equal(1, invoice2.LineItems.Count);

            InvoiceLineItem lineItem2 = invoice2.LineItems[0];
            Assert.NotNull(lineItem2);
            Assert.Equal(200, lineItem2.AmountExcludingVAT);
        }

        [Fact]
        public async Task CreateTransactionFile_WithExceptionThrown_ReturnsError_AndNoTransactionFile()
        {
            // Arrange
            UKCompetentAuthority authority = A.Dummy<UKCompetentAuthority>();

            Organisation organisation = Organisation.CreateSoleTrader("Test organisation");

            Scheme scheme = new Scheme(organisation);
            scheme.UpdateScheme(
                "Test scheme",
                "WEE/AA1111AA/SCH",
                "WEE00000001",
                A.Dummy<ObligationType>(),
                authority);

            int complianceYear = A.Dummy<int>();

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                123.45m,
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            memberUpload.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync())
                .Returns((string)null); // A null value will cause the Ibis object to throw an exception.

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);

            // Assert
            Assert.Null(result.IbisFile);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async void CreateTransactionFile_GivenMemberUploadContainsAnnualChargeAndSchemeHasAnnualChargeAmount_DescriptionShouldContainAnnualCharge()
        {
            var organisation = A.Fake<Organisation>();
            var authority = A.Fake<UKCompetentAuthority>();
            var scheme = A.Fake<Scheme>();

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                123.45m,
                A.Dummy<int>(),
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                true);

            SystemTime.Freeze(new DateTime(2019, 1, 1));
            memberUpload.Submit(A.Dummy<User>());
            SystemTime.Unfreeze();

            A.CallTo(() => authority.AnnualChargeAmount).Returns(12500);
            A.CallTo(() => scheme.IbisCustomerReference).Returns("WEE00000002");
            A.CallTo(() => scheme.Organisation).Returns(organisation);
            A.CallTo(() => scheme.CompetentAuthority).Returns(authority);
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            var memberUploads = new List<MemberUpload>() { memberUpload };

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var result = await generator.CreateAsync(0, invoiceRun);

            Assert.Equal(result.IbisFile.Invoices[0].LineItems[0].Description, "Charge for producer registration submission made on 01 Jan 2019 and the £12,500.00 annual charge.");
        }

        [Fact]
        public async void CreateTransactionFile_GivenMemberUploadSchemeHasZeroAnnualChargeAmount_DescriptionShouldNotContainAnnualCharge()
        {
            var organisation = A.Fake<Organisation>();
            var authority = A.Fake<UKCompetentAuthority>();
            var scheme = A.Fake<Scheme>();

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                123.45m,
                A.Dummy<int>(),
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                true);

            SystemTime.Freeze(new DateTime(2019, 1, 1));
            memberUpload.Submit(A.Dummy<User>());
            SystemTime.Unfreeze();

            A.CallTo(() => authority.AnnualChargeAmount).Returns(0);
            A.CallTo(() => scheme.IbisCustomerReference).Returns("WEE00000002");
            A.CallTo(() => scheme.Organisation).Returns(organisation);
            A.CallTo(() => scheme.CompetentAuthority).Returns(authority);
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            var memberUploads = new List<MemberUpload>() { memberUpload };

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var result = await generator.CreateAsync(0, invoiceRun);

            Assert.Equal(result.IbisFile.Invoices[0].LineItems[0].Description, "Charge for producer registration submission made on 01 Jan 2019.");
        }

        [Fact]
        public async void CreateTransactionFile_GivenMemberUploadSchemeHasNoAnnualAmount_DescriptionShouldNotContainAnnualCharge()
        {
            var organisation = A.Fake<Organisation>();
            var authority = A.Fake<UKCompetentAuthority>();
            var scheme = A.Fake<Scheme>();

            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                123.45m,
                A.Dummy<int>(),
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            SystemTime.Freeze(new DateTime(2019, 1, 1));
            memberUpload.Submit(A.Dummy<User>());
            SystemTime.Unfreeze();

            A.CallTo(() => authority.AnnualChargeAmount).Returns(0);
            A.CallTo(() => scheme.IbisCustomerReference).Returns("WEE00000002");
            A.CallTo(() => scheme.Organisation).Returns(organisation);
            A.CallTo(() => scheme.CompetentAuthority).Returns(authority);
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            var memberUploads = new List<MemberUpload>() { memberUpload };

            var invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            var result = await generator.CreateAsync(0, invoiceRun);

            Assert.Equal(result.IbisFile.Invoices[0].LineItems[0].Description, "Charge for producer registration submission made on 01 Jan 2019.");
        }
    }
}