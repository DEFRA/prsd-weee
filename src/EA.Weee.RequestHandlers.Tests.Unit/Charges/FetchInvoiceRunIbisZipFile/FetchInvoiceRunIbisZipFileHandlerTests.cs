namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchInvoiceRunIbisZipFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain;
    using Domain.Charges;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core;
    using RequestHandlers.Charges;
    using RequestHandlers.Charges.FetchPendingCharges;
    using RequestHandlers.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class FetchInvoiceRunIbisZipFileHandlerTests
    {
        /// <summary>
        /// This test ensures that a SecurityException is thrown if the handler is used by a user without access
        /// to the internal area.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            FetchInvoiceRunIbisZipFileHandler handler = new FetchInvoiceRunIbisZipFileHandler(
                authorization,
                A.Dummy<ICommonDataAccess>());

            // Act
            Func<Task<FileInfo>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.Charges.FetchInvoiceRunIbisZipFile>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        /// <summary>
        /// This test ensures that an InvalidOperationException is thrown when the ZIP file representing 1B1S file data
        /// is fetched for an invoice run without 1B1S file data.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithInvoiceRunWithout1B1SFileData_ThrowsInvalidOperationException()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            UKCompetentAuthority authority = A.Dummy<UKCompetentAuthority>();

            Scheme scheme = A.Dummy<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(authority);

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>());

            memberUpload.Submit();

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads);

            ICommonDataAccess dataAccess = A.Fake<ICommonDataAccess>();
            A.CallTo(() => dataAccess.FetchInvoiceRunAsync(A<Guid>._)).Returns(invoiceRun);

            FetchInvoiceRunIbisZipFileHandler handler = new FetchInvoiceRunIbisZipFileHandler(
                authorization,
                dataAccess);

            // Act
            Func<Task<FileInfo>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.Charges.FetchInvoiceRunIbisZipFile>());

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(testCode);
        }

        /// <summary>
        /// This test ensures that the file info returned by the handler has a file name with
        /// the format "WEEE invoice files [1B1S File ID] [Invoice run created date].zip",
        /// where the file ID is padded to 5 digits and the date is formatted as yyyy-MM-dd.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAysnc_HappyPath_ReturnsFileInfoWithCorrectFileName()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            UKCompetentAuthority authority = new UKCompetentAuthority(
                A.Dummy<Guid>(),
                "Environment Agency",
                "EA",
                A.Dummy<Country>());

            Scheme scheme = A.Dummy<Scheme>();
            A.CallTo(() => scheme.CompetentAuthority).Returns(authority);

            MemberUpload memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                scheme,
                A.Dummy<string>(),
                A.Dummy<string>());

            memberUpload.Submit();

            List<MemberUpload> memberUploads = new List<MemberUpload>();
            memberUploads.Add(memberUpload);

            SystemTime.Freeze(new DateTime(2015, 12, 31));
            InvoiceRun invoiceRun = new InvoiceRun(authority, memberUploads);
            SystemTime.Unfreeze();

            ulong fileID = 123;

            IbisFileData ibisFileData = new IbisFileData(fileID, "Customer File.dat", "data", "Transaction File.dat", "data");
            invoiceRun.SetIbisFileData(ibisFileData);

            ICommonDataAccess dataAccess = A.Fake<ICommonDataAccess>();
            A.CallTo(() => dataAccess.FetchInvoiceRunAsync(A<Guid>._)).Returns(invoiceRun);

            FetchInvoiceRunIbisZipFileHandler handler = new FetchInvoiceRunIbisZipFileHandler(
                authorization,
                dataAccess);

            // Act
            FileInfo fileInfo = await handler.HandleAsync(A.Dummy<Requests.Charges.FetchInvoiceRunIbisZipFile>());

            // Assert
            Assert.NotNull(fileInfo);
            Assert.Equal("WEEE invoice files 00123 2015-12-31.zip", fileInfo.FileName);
        }
    }
}
