namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Charges;
    using Domain.Scheme;
    using FakeItEasy;
    using Ibis;
    using Prsd.Core;
    using RequestHandlers.Charges.IssuePendingCharges;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class BySubmissionTransactionFileGeneratorTests
    {
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

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads);

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            BySubmissionTransactionFileGenerator generator = new BySubmissionTransactionFileGenerator(transactionReferenceGenerator);

            ulong id = 12345;

            // Act
            TransactionFile transactionFile = await generator.CreateAsync(id, invoiceRun);

            // Assert
            Assert.Equal((ulong)12345, transactionFile.FileID);
        }

        /// <summary>
        /// This test ensures that creating a transaction file from one member upload will
        /// result in one invoice with one line item being added to the file; with the
        /// correct details taken from the member upload. It also ensures that the
        /// details are not taken from any producer submissions within the member upload.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithOneMemberUpload_CreatesOneInvoiceWithOneLineItem()
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

            SystemTime.Freeze(new DateTime(2015, 1, 1));
            memberUpload.Submit(A.Dummy<User>());
            SystemTime.Unfreeze();

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            BySubmissionTransactionFileGenerator generator = new BySubmissionTransactionFileGenerator(transactionReferenceGenerator);

            SystemTime.Freeze(new DateTime(2015, 12, 31));
            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads);
            SystemTime.Unfreeze();

            // Act
            TransactionFile transactionFile = await generator.CreateAsync(0, invoiceRun);

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
        /// This test ensures that creating a transaction file from two member uploads 
        /// will result in two invoices being added to the file.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithTwoMemberUploads_CreatesTwoInvoices()
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
                A.Dummy<string>());

            memberUpload1.Submit(A.Dummy<User>());

            MemberUpload memberUpload2 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                200,
                complianceYear,
                scheme,
                A.Dummy<string>());

            memberUpload2.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload1);
            memberUploads.Add(memberUpload2);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads);

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            BySubmissionTransactionFileGenerator generator = new BySubmissionTransactionFileGenerator(transactionReferenceGenerator);

            // Act
            TransactionFile transactionFile = await generator.CreateAsync(0, invoiceRun);

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Equal(2, transactionFile.Invoices.Count);

            Invoice invoice1 = transactionFile.Invoices[0];
            Assert.NotNull(invoice1);
            Assert.Equal(100m, invoice1.TransactionTotal);

            Invoice invoice2 = transactionFile.Invoices[1];
            Assert.NotNull(invoice2);
            Assert.Equal(200m, invoice2.TransactionTotal);
        }
    }
}
