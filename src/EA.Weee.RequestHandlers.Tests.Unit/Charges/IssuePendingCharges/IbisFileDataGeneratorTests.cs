namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Charges;
    using EA.Weee.RequestHandlers.Charges.IssuePendingCharges;
    using FakeItEasy;
    using Ibis;
    using RequestHandlers.Charges.IssuePendingCharges.Errors;
    using Xunit;

    public class IbisFileDataGeneratorTests
    {
        /// <summary>
        /// This test ensures that calling CreateFileData will return a file data
        /// object with some content in both the CustomerFileData and TransactionFileData
        /// properties.
        /// </summary>
        [Fact]
        public async Task CreateFileData_Always_CreatesFileData()
        {
            // Arrange
            CustomerFile customerFile = new CustomerFile("WEE", (ulong)12345);
            var customerFileGeneratorResult = new IbisFileGeneratorResult<CustomerFile>(customerFile, A.Dummy<List<Exception>>());
            IIbisCustomerFileGenerator customerFileGenerator = A.Fake<IIbisCustomerFileGenerator>();
            A.CallTo(() => customerFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(customerFileGeneratorResult);

            TransactionFile transactionFile = new TransactionFile("WEE", (ulong)12345);
            var ibisFileGeneratorResult = new IbisFileGeneratorResult<TransactionFile>(transactionFile, A.Dummy<List<Exception>>());
            IIbisTransactionFileGenerator transactionFileGenerator = A.Fake<IIbisTransactionFileGenerator>();
            A.CallTo(() => transactionFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(ibisFileGeneratorResult);

            IIbisFileDataErrorTranslator errorTranslator = A.Dummy<IIbisFileDataErrorTranslator>();

            IbisFileDataGenerator generator = new IbisFileDataGenerator(
                customerFileGenerator,
                transactionFileGenerator,
                errorTranslator);

            // Act
            var result = await generator.CreateFileDataAsync(A.Dummy<ulong>(), A.Dummy<InvoiceRun>());
            var ibistFileData = result.IbisFileData;

            // Assert
            Assert.NotNull(ibistFileData);
            Assert.False(string.IsNullOrEmpty(ibistFileData.CustomerFileData));
            Assert.False(string.IsNullOrEmpty(ibistFileData.TransactionFileData));
        }

        /// <summary>
        /// This test ensures that calling CreateFileData will return a file data
        /// object with customer and transaction filenames in the correct format.
        /// Customer files should be a concatentaion of "WEECI" and the file ID
        /// padded to 5 characters. The extension should be ".dat";
        /// Transaction files should be a concatenation of "WEEHI" and the file ID
        /// padded to 5 characters. The extension should be ".dat";
        /// </summary>
        [Fact]
        public async Task CreateFileData_WithFileID_CreatesFilesWithCorrectFileNames()
        {
            // Arrange
            ulong fileID = 123;

            CustomerFile customerFile = new CustomerFile("WEE", (ulong)12345);
            var customerFileGeneratorResult = new IbisFileGeneratorResult<CustomerFile>(customerFile, A.Dummy<List<Exception>>());
            IIbisCustomerFileGenerator customerFileGenerator = A.Fake<IIbisCustomerFileGenerator>();
            A.CallTo(() => customerFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(customerFileGeneratorResult);

            TransactionFile transactionFile = new TransactionFile("WEE", (ulong)12345);
            var ibisFileGeneratorResult = new IbisFileGeneratorResult<TransactionFile>(transactionFile, A.Dummy<List<Exception>>());
            IIbisTransactionFileGenerator transactionFileGenerator = A.Fake<IIbisTransactionFileGenerator>();
            A.CallTo(() => transactionFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(ibisFileGeneratorResult);

            IIbisFileDataErrorTranslator errorTranslator = A.Dummy<IIbisFileDataErrorTranslator>();

            IbisFileDataGenerator generator = new IbisFileDataGenerator(
                customerFileGenerator,
                transactionFileGenerator,
                errorTranslator);

            // Act
            var result = await generator.CreateFileDataAsync(fileID, A.Dummy<InvoiceRun>());
            var ibistFileData = result.IbisFileData;

            // Assert
            Assert.NotNull(ibistFileData);
            Assert.Equal("WEEHC00123.dat", ibistFileData.CustomerFileName);
            Assert.Equal("WEEHI00123.dat", ibistFileData.TransactionFileName);
        }

        [Fact]
        public async Task CreateFileData_WithErrorGeneratingCustomerFile_TranslatesError()
        {
            // Arrange
            ulong fileID = 123;

            var customerFile = new CustomerFile("WEE", fileID);
            var error = new Exception();
            var customerFileGeneratorResult = new IbisFileGeneratorResult<CustomerFile>(customerFile, new List<Exception> { error });

            var customerFileGenerator = A.Fake<IIbisCustomerFileGenerator>();
            A.CallTo(() => customerFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(customerFileGeneratorResult);

            var transactionFile = new TransactionFile("WEE", (ulong)12345);
            var transactionFileGeneratorResult = new IbisFileGeneratorResult<TransactionFile>(transactionFile, A.Dummy<List<Exception>>());

            var transactionFileGenerator = A.Fake<IIbisTransactionFileGenerator>();
            A.CallTo(() => transactionFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(transactionFileGeneratorResult);

            var errorTranslator = A.Fake<IIbisFileDataErrorTranslator>();

            IbisFileDataGenerator generator = new IbisFileDataGenerator(
                customerFileGenerator,
                transactionFileGenerator,
                errorTranslator);

            // Act
            await generator.CreateFileDataAsync(fileID, A.Dummy<InvoiceRun>());

            // Assert
            A.CallTo(() => errorTranslator.MakeFriendlyErrorMessages(A<List<Exception>>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task CreateFileData_WithErrorGeneratingTransactionFile_TranslatesError()
        {
            // Arrange
            ulong fileID = 123;

            var customerFile = new CustomerFile("WEE", fileID);
            var customerFileGeneratorResult = new IbisFileGeneratorResult<CustomerFile>(customerFile, A.Dummy<List<Exception>>());

            var customerFileGenerator = A.Fake<IIbisCustomerFileGenerator>();
            A.CallTo(() => customerFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(customerFileGeneratorResult);

            var transactionFile = new TransactionFile("WEE", (ulong)12345);
            var error = new Exception();
            var transactionFileGeneratorResult = new IbisFileGeneratorResult<TransactionFile>(transactionFile, new List<Exception> { error });

            var transactionFileGenerator = A.Fake<IIbisTransactionFileGenerator>();
            A.CallTo(() => transactionFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(transactionFileGeneratorResult);

            var errorTranslator = A.Fake<IIbisFileDataErrorTranslator>();

            IbisFileDataGenerator generator = new IbisFileDataGenerator(
                customerFileGenerator,
                transactionFileGenerator,
                errorTranslator);

            // Act
            await generator.CreateFileDataAsync(fileID, A.Dummy<InvoiceRun>());

            // Assert
            A.CallTo(() => errorTranslator.MakeFriendlyErrorMessages(A<List<Exception>>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task CreateFileData_WithErrorGeneratingIbisFile_TranslatesError_AndReturnNoIbisFileData()
        {
            // Arrange
            ulong fileID = 123;

            var customerFile = new CustomerFile("WEE", fileID);
            var customerFileGeneratorResult = new IbisFileGeneratorResult<CustomerFile>(customerFile, A.Dummy<List<Exception>>());

            var customerFileGenerator = A.Fake<IIbisCustomerFileGenerator>();
            A.CallTo(() => customerFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(customerFileGeneratorResult);

            var transactionFile = new TransactionFile("WEE", (ulong)12345);
            var error = new Exception();
            var transactionFileGeneratorResult = new IbisFileGeneratorResult<TransactionFile>(transactionFile, new List<Exception> { error });

            var transactionFileGenerator = A.Fake<IIbisTransactionFileGenerator>();
            A.CallTo(() => transactionFileGenerator.CreateAsync(A<ulong>._, A<InvoiceRun>._))
                .Returns(transactionFileGeneratorResult);

            var errorTranslator = A.Fake<IIbisFileDataErrorTranslator>();
            A.CallTo(() => errorTranslator.MakeFriendlyErrorMessages(A<List<Exception>>._))
                .Returns(new List<string> { "error" });

            IbisFileDataGenerator generator = new IbisFileDataGenerator(
                customerFileGenerator,
                transactionFileGenerator,
                errorTranslator);

            // Act
            var result = await generator.CreateFileDataAsync(fileID, A.Dummy<InvoiceRun>());

            // Assert
            Assert.NotEmpty(result.Errors);
            Assert.Null(result.IbisFileData);
        }
    }
}
