﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.IssuePendingCharges
{
    using Domain;
    using Domain.Charges;
    using Domain.Obligation;
    using Domain.Producer;
    using Domain.Producer.Classfication;
    using Domain.Producer.Classification;
    using Domain.Scheme;
    using Domain.User;
    using FakeItEasy;
    using Ibis;
    using Prsd.Core;
    using RequestHandlers.Charges.IssuePendingCharges;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class ByChargeValueTransactionFileGeneratorTests
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

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            ByChargeValueTransactionFileGenerator generator = new ByChargeValueTransactionFileGenerator(transactionReferenceGenerator);

            ulong id = 12345;

            // Act
            var result = await generator.CreateAsync(id, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.Equal((ulong)12345, transactionFile.FileID);
        }

        /// <summary>
        /// This test ensures that creating a transaction file from one member upload with
        /// a single producer submission, where the submission has no charge, will result
        /// in no invoices being added to the file.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_ProducerSubmissionWithNoCharge_NotIncludedInTransactionFile()
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
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>());

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/11AAAA11", complianceYear, scheme);

            ProducerSubmission producerSubmission = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                0,
                A.Dummy<StatusType>());

            memberUpload.ProducerSubmissions.Add(producerSubmission);

            memberUpload.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            ByChargeValueTransactionFileGenerator generator = new ByChargeValueTransactionFileGenerator(
                A.Dummy<ITransactionReferenceGenerator>());

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Equal(0, transactionFile.Invoices.Count);
        }

        /// <summary>
        /// This test ensures that creating a transaction file from one member upload with
        /// a single producer submission, where the producer is marked as not being invoiced,
        /// will result in no invoices being added to the file.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_ProducerSubmissionMarkedAsNotInvoiced_NotIncludedInTransactionFile()
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
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>());

            var producerSubmission = A.Fake<ProducerSubmission>();
            A.CallTo(() => producerSubmission.ChargeThisUpdate)
                .Returns(123.45m);
            A.CallTo(() => producerSubmission.Invoiced)
                .Returns(false);

            memberUpload.ProducerSubmissions.Add(producerSubmission);
            memberUpload.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            ByChargeValueTransactionFileGenerator generator = new ByChargeValueTransactionFileGenerator(
                A.Dummy<ITransactionReferenceGenerator>());

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Equal(0, transactionFile.Invoices.Count);
        }

        /// <summary>
        /// This test ensures that creating a transaction file from one member upload with
        /// a single producer submission will result in one invoice with one line item being
        /// added to the file; with the correct details of the producer submission.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithOneProducerSubmission_CreatesOneInvoiceWithOneLineItem()
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
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>());

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/11AAAA11", complianceYear, scheme);

            ProducerSubmission producerSubmission = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                123.45m,
                A.Dummy<StatusType>());

            memberUpload.ProducerSubmissions.Add(producerSubmission);

            memberUpload.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            SystemTime.Freeze(new DateTime(2015, 12, 31));
            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());
            SystemTime.Unfreeze();

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            ByChargeValueTransactionFileGenerator generator = new ByChargeValueTransactionFileGenerator(transactionReferenceGenerator);

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Single(transactionFile.Invoices);

            Invoice invoice = transactionFile.Invoices[0];
            Assert.NotNull(invoice);

            Assert.Equal(TransactionType.Invoice, invoice.TransactionType);
            Assert.Equal("WEE800001H", invoice.TransactionReference);
            Assert.Equal(123.45m, invoice.TransactionTotal);
            Assert.Null(invoice.TransactionHeaderNarrative);
            Assert.Equal(new DateTime(2015, 12, 31), invoice.TransactionDate);
            Assert.Null(invoice.RelatedTransactionReference);
            Assert.Equal(CurrencyCode.GBP, invoice.CurrencyCode);
            Assert.Equal("WEE00000001", invoice.CustomerReference);

            Assert.NotNull(invoice.LineItems);
            Assert.Single(invoice.LineItems);

            InvoiceLineItem lineItem = invoice.LineItems[0];
            Assert.NotNull(lineItem);

            Assert.Equal(123.45m, lineItem.AmountExcludingVAT);
            Assert.Equal("1 producer registration charge at £123.45.", lineItem.Description);
            Assert.Equal("H", lineItem.AreaCode);
            Assert.Equal("H", lineItem.ContextCode);
            Assert.Equal("W", lineItem.IncomeStreamCode);
            Assert.Equal((ulong)1, lineItem.Quantity);
            Assert.Equal(UnitOfMeasure.Each, lineItem.UnitOfMeasure);
            Assert.Null(lineItem.VatCode);
        }

        /// <summary>
        /// This test ensures that creating a transaction file from one member upload with
        /// two producer submissions with different charges will result in one invoice with
        /// two line items being added to the file.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithTwoProducerSubmissionsWithDifferentCharges_CreatesOneInvoiceWithTwoLineItems()
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
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>());

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/11AAAA11", complianceYear, scheme);

            ProducerSubmission producerSubmission1 = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                100m,
                A.Dummy<StatusType>());

            memberUpload.ProducerSubmissions.Add(producerSubmission1);

            ProducerSubmission producerSubmission2 = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                200m,
                A.Dummy<StatusType>());

            memberUpload.ProducerSubmissions.Add(producerSubmission2);

            memberUpload.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            ByChargeValueTransactionFileGenerator generator = new ByChargeValueTransactionFileGenerator(transactionReferenceGenerator);

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Single(transactionFile.Invoices);

            Invoice invoice = transactionFile.Invoices[0];
            Assert.NotNull(invoice);
            Assert.Equal(300m, invoice.TransactionTotal);

            Assert.NotNull(invoice.LineItems);
            Assert.Equal(2, invoice.LineItems.Count);

            InvoiceLineItem lineItem1 = invoice.LineItems[0];
            Assert.NotNull(lineItem1);
            Assert.Equal(100m, lineItem1.AmountExcludingVAT);
            Assert.Equal("1 producer registration charge at £100.00.", lineItem1.Description);

            InvoiceLineItem lineItem2 = invoice.LineItems[1];
            Assert.NotNull(lineItem2);
            Assert.Equal(200m, lineItem2.AmountExcludingVAT);
            Assert.Equal("1 producer registration charge at £200.00.", lineItem2.Description);
        }

        /// <summary>
        /// This test ensures that creating a transaction file from one member upload with
        /// two producer submissions with the same charge will result in one invoice with
        /// one line item being added to the file.
        /// </summary>
        [Fact]
        public async Task CreateTransactionFile_WithTwoProducerSubmissionsWithSameCharge_CreatesOneInvoiceWithOneLineItem()
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
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>());

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/11AAAA11", complianceYear, scheme);

            ProducerSubmission producerSubmission1 = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                100m,
                A.Dummy<StatusType>());

            memberUpload.ProducerSubmissions.Add(producerSubmission1);

            ProducerSubmission producerSubmission2 = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                100m,
                A.Dummy<StatusType>());

            memberUpload.ProducerSubmissions.Add(producerSubmission2);

            memberUpload.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            ByChargeValueTransactionFileGenerator generator = new ByChargeValueTransactionFileGenerator(transactionReferenceGenerator);

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Single(transactionFile.Invoices);

            Invoice invoice = transactionFile.Invoices[0];
            Assert.NotNull(invoice);
            Assert.Equal(200m, invoice.TransactionTotal);

            Assert.NotNull(invoice.LineItems);
            Assert.Single(invoice.LineItems);

            InvoiceLineItem lineItem = invoice.LineItems[0];
            Assert.NotNull(lineItem);
            Assert.Equal(200m, lineItem.AmountExcludingVAT);
            Assert.Equal("2 producer registration charges at £100.00.", lineItem.Description);
        }

        /// <summary>
        /// This test ensures that creating a transaction file from two member uploads 
        /// with one producer submission each will result in two invoices being added to the file.
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
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            RegisteredProducer registeredProducer1 = new RegisteredProducer("WEE/11AAAA11", complianceYear, scheme);

            ProducerSubmission producerSubmission1 = new ProducerSubmission(
                registeredProducer1,
                memberUpload1,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                100m,
                A.Dummy<StatusType>());

            memberUpload1.ProducerSubmissions.Add(producerSubmission1);

            memberUpload1.Submit(A.Dummy<User>());

            MemberUpload memberUpload2 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            RegisteredProducer registeredProducer2 = new RegisteredProducer("WEE/22BBBB22", complianceYear, scheme);

            ProducerSubmission producerSubmission2 = new ProducerSubmission(
                registeredProducer2,
                memberUpload2,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                100m,
                A.Dummy<StatusType>());

            memberUpload2.ProducerSubmissions.Add(producerSubmission2);

            memberUpload2.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload1);
            memberUploads.Add(memberUpload2);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns("WEE800001H");

            ByChargeValueTransactionFileGenerator generator = new ByChargeValueTransactionFileGenerator(transactionReferenceGenerator);

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);
            TransactionFile transactionFile = result.IbisFile;

            // Assert
            Assert.NotNull(transactionFile);
            Assert.Equal(2, transactionFile.Invoices.Count);

            Invoice invoice1 = transactionFile.Invoices[0];
            Assert.NotNull(invoice1);
            Assert.Equal(100m, invoice1.TransactionTotal);

            Invoice invoice2 = transactionFile.Invoices[1];
            Assert.NotNull(invoice2);
            Assert.Equal(100m, invoice2.TransactionTotal);
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
                A.Dummy<decimal>(),
                complianceYear,
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>(),
                false);

            RegisteredProducer registeredProducer = new RegisteredProducer("WEE/11AAAA11", complianceYear, scheme);

            ProducerSubmission producerSubmission = new ProducerSubmission(
                registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                A.Dummy<EEEPlacedOnMarketBandType>(),
                A.Dummy<SellingTechniqueType>(),
                A.Dummy<ObligationType>(),
                A.Dummy<AnnualTurnOverBandType>(),
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<Domain.Lookup.ChargeBandAmount>(),
                100M,
                A.Dummy<StatusType>());

            memberUpload.ProducerSubmissions.Add(producerSubmission);

            memberUpload.Submit(A.Dummy<User>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads, A.Dummy<User>());

            ITransactionReferenceGenerator transactionReferenceGenerator = A.Fake<ITransactionReferenceGenerator>();
            A.CallTo(() => transactionReferenceGenerator.GetNextTransactionReferenceAsync()).Returns((string)null); // A null value will cause the Ibis object to throw an exception.

            ByChargeValueTransactionFileGenerator generator = new ByChargeValueTransactionFileGenerator(
                transactionReferenceGenerator);

            // Act
            var result = await generator.CreateAsync(0, invoiceRun);

            // Assert
            Assert.Null(result.IbisFile);
            Assert.NotEmpty(result.Errors);
        }
    }
}
