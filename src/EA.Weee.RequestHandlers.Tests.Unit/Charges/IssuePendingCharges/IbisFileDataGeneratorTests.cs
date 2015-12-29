namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Charges;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Charges.IssuePendingCharges;
    using FakeItEasy;
    using Ibis;
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
            IIbisCustomerFileGenerator customerFileGenerator = A.Fake<IIbisCustomerFileGenerator>();
            A.CallTo(() => customerFileGenerator.CreateAsync(A<ulong>._, A<IReadOnlyList<MemberUpload>>._))
                .Returns(customerFile);

            TransactionFile transactionFile = new TransactionFile("WEE", (ulong)12345);
            IIbisTransactionFileGenerator transactionFileGenerator = A.Fake<IIbisTransactionFileGenerator>();
            A.CallTo(() => transactionFileGenerator.CreateAsync(A<ulong>._, A<IReadOnlyList<MemberUpload>>._))
                .Returns(transactionFile);

            IbisFileDataGenerator generator = new IbisFileDataGenerator(
                customerFileGenerator,
                transactionFileGenerator);

            // Act
            IbisFileData result = await generator.CreateFileDataAsync(A.Dummy<ulong>(), A.Dummy<List<MemberUpload>>());

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.CustomerFileData));
            Assert.False(string.IsNullOrEmpty(result.TransactionFileData));
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
            IIbisCustomerFileGenerator customerFileGenerator = A.Fake<IIbisCustomerFileGenerator>();
            A.CallTo(() => customerFileGenerator.CreateAsync(A<ulong>._, A<IReadOnlyList<MemberUpload>>._))
                .Returns(customerFile);

            TransactionFile transactionFile = new TransactionFile("WEE", (ulong)12345);
            IIbisTransactionFileGenerator transactionFileGenerator = A.Fake<IIbisTransactionFileGenerator>();
            A.CallTo(() => transactionFileGenerator.CreateAsync(A<ulong>._, A<IReadOnlyList<MemberUpload>>._))
                .Returns(transactionFile);

            IbisFileDataGenerator generator = new IbisFileDataGenerator(
                customerFileGenerator,
                transactionFileGenerator);

            // Act
            IbisFileData result = await generator.CreateFileDataAsync(fileID, A.Dummy<List<MemberUpload>>());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WEEHC00123.dat", result.CustomerFileName);
            Assert.Equal("WEEHI00123.dat", result.TransactionFileName);
        }
    }
}
