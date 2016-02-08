namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using EA.Weee.Core.Shared;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using Requests.Admin;
    using Xunit;

    public class GetProducerEeeDataHistoryCsvHandlerTests
    {
        /// <summary>
        /// This test ensures that the handler throws a security exception if used by
        /// a user without access to the internal area.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            // Arrange            
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            GetProducerEeeDataHistoryCsv request = new GetProducerEeeDataHistoryCsv(A.Dummy<string>());

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNoRegistrationNumber_ThrowsArgumentException()
        {
            // Arrange            
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            GetProducerEeeDataHistoryCsv request = new GetProducerEeeDataHistoryCsv(string.Empty);

            // Act
            Func<Task<CSVFileData>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileContent()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            GetProducerEeeDataHistoryCsv request = new GetProducerEeeDataHistoryCsv("PRN");
            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        /// <summary>
        /// This test ensures that blank row is added for producer which has been removed and "LatestData" flag set to "yes" 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateResults_WithProducerRemovedAfterFirstUpload_ReturnsEeeDataHistorywithLatestAsBlankDataforProducer()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            ProducerEeeHistoryCsvData results = new ProducerEeeHistoryCsvData();
            results.ProducerReturnsHistoryData.Add(CreateEeeDataForProducer("WEE/MM0841AA", "WEE/FA0000KE/SCH", "schemeB2B", 2007, 1, new DateTime(2016, 1, 1), 100, "Yes"));
            results.ProducerRemovedFromReturnsData.Add(CreateRemovedProducerResults("WEE/FA0000KE/SCH", 2007, new DateTime(2016, 1, 2), 1));

            //Act
            IEnumerable<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult> csvResults = new List<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult>();
            csvResults = handler.CreateResults(results);

            //Assert
            Assert.NotNull(csvResults);
            Assert.Equal(2, csvResults.Count());
            Assert.Collection(csvResults,
               (r1) => Assert.Equal(new DateTime(2016, 1, 2), r1.SubmittedDate),
               (r2) => Assert.Equal(new DateTime(2016, 1, 1), r2.SubmittedDate));

            Assert.Collection(csvResults,
               (r1) => Assert.Equal("Yes", r1.LatestData),
               (r2) => Assert.Equal("No", r2.LatestData));

            Assert.Collection(csvResults,
              (r1) => Assert.Equal(null, r1.Cat1B2B),
              (r2) => Assert.Equal(100, r2.Cat1B2B));
        }
        
        /// <summary>
        /// This test ensures that blank row is added in between 2 upload 
        /// for producer which has been removed in upload2 after upload1 but added later on in the upload 3
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateResults_WithProducerRemovedAfterFirstUploadandAddedLaterOn_ReturnsEeeDataHistorywithBlankDataforProducerasSecondRow()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            //first upload
            ProducerEeeHistoryCsvData results = new ProducerEeeHistoryCsvData();
            ProducerEeeHistoryCsvData.ProducerInReturnsResult upload1 = CreateEeeDataForProducer("WEE/MM0841AA", "WEE/FA0000KE/SCH", "Test Scheme", 2007, 1, new DateTime(2016, 1, 1), 100, "No");
            results.ProducerReturnsHistoryData.Add(upload1);

            //producer removed in second upload
            ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult removedUpload = CreateRemovedProducerResults("WEE/FA0000KE/SCH", 2007, new DateTime(2016, 1, 2), 1);
            results.ProducerRemovedFromReturnsData.Add(removedUpload);

            //In this upload producer added back again
            ProducerEeeHistoryCsvData.ProducerInReturnsResult upload2 = CreateEeeDataForProducer("WEE/MM0841AA", "WEE/FA0000KE/SCH", "Test Scheme", 2007, 1, new DateTime(2016, 1, 3), 200, "Yes");            
            results.ProducerReturnsHistoryData.Add(upload2);
            
            //Act
            IEnumerable<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult> csvResults = new List<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult>();
            csvResults = handler.CreateResults(results);

            //Assert
            Assert.NotNull(csvResults);
            Assert.Equal(3, csvResults.Count());
            Assert.Collection(csvResults,
               (r1) => Assert.Equal(new DateTime(2016, 1, 3), r1.SubmittedDate),
               (r2) => Assert.Equal(new DateTime(2016, 1, 2), r2.SubmittedDate),
               (r3) => Assert.Equal(new DateTime(2016, 1, 1), r3.SubmittedDate));

            Assert.Collection(csvResults,
               (r1) => Assert.Equal("Yes", r1.LatestData),
               (r2) => Assert.Equal("No", r2.LatestData),
               (r3) => Assert.Equal("No", r3.LatestData));

            Assert.Collection(csvResults,
              (r1) => Assert.Equal(200, r1.Cat1B2B),
              (r2) => Assert.Equal(null, r2.Cat1B2B),
              (r3) => Assert.Equal(100, r3.Cat1B2B));
        }

        /// <summary>
        /// This test ensures that blank row is added in between 2 upload 
        /// for producer which has been removed in upload2 after upload1 but added later on in the upload 3 even if data is not changes
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateResults_WithProducerRemovedAfterFirstUploadandAddedLaterOn_ReturnsEeeDataHistorywithBlankDataforProducerasSecondRowEvenIfDatanotChanges()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            //first upload
            ProducerEeeHistoryCsvData results = new ProducerEeeHistoryCsvData();
            ProducerEeeHistoryCsvData.ProducerInReturnsResult upload1 = CreateEeeDataForProducer("WEE/MM0841AA", "WEE/FA0000KE/SCH", "Test Scheme", 2007, 1, new DateTime(2016, 1, 1), 100, "No");
            results.ProducerReturnsHistoryData.Add(upload1);

            //producer removed in second upload
            ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult removedUpload = CreateRemovedProducerResults("WEE/FA0000KE/SCH", 2007, new DateTime(2016, 1, 2), 1);
            results.ProducerRemovedFromReturnsData.Add(removedUpload);

            //In this upload producer added back again with same data as first upload
            ProducerEeeHistoryCsvData.ProducerInReturnsResult upload2 = CreateEeeDataForProducer("WEE/MM0841AA", "WEE/FA0000KE/SCH", "Test Scheme", 2007, 1, new DateTime(2016, 1, 3), 100, "Yes");
            results.ProducerReturnsHistoryData.Add(upload2);

            //Act
            IEnumerable<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult> csvResults = new List<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult>();
            csvResults = handler.CreateResults(results);

            //Assert
            Assert.NotNull(csvResults);
            Assert.Equal(3, csvResults.Count());
            Assert.Collection(csvResults,
               (r1) => Assert.Equal(new DateTime(2016, 1, 3), r1.SubmittedDate),
               (r2) => Assert.Equal(new DateTime(2016, 1, 2), r2.SubmittedDate),
               (r3) => Assert.Equal(new DateTime(2016, 1, 1), r3.SubmittedDate));

            Assert.Collection(csvResults,
               (r1) => Assert.Equal("Yes", r1.LatestData),
               (r2) => Assert.Equal("No", r2.LatestData),
               (r3) => Assert.Equal("No", r3.LatestData));

            Assert.Collection(csvResults,
              (r1) => Assert.Equal(100, r1.Cat1B2B),
              (r2) => Assert.Equal(null, r2.Cat1B2B),
              (r3) => Assert.Equal(100, r3.Cat1B2B));
        }

        /// <summary>
        /// This test ensures that only one blank row is added for producer which has been removed in upload2 and upload3 after upload1 but added later on added in the upload 4 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateResults_WithProducerConsecutivelyRemovedInNetUploadsbutLatesOnAdded_ReturnsEeeDataHistoryWithOneBlankDataRowforProducer()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            //first upload
            ProducerEeeHistoryCsvData results = new ProducerEeeHistoryCsvData();
            ProducerEeeHistoryCsvData.ProducerInReturnsResult upload1 = CreateEeeDataForProducer("WEE/MM0841AA", "WEE/FA0000KE/SCH", "Test Scheme", 2007, 1, new DateTime(2016, 1, 1), 100, "No");
            results.ProducerReturnsHistoryData.Add(upload1);

            //producer removed in second upload
            ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult removedUpload = CreateRemovedProducerResults("WEE/FA0000KE/SCH", 2007, new DateTime(2016, 1, 2), 1);
            results.ProducerRemovedFromReturnsData.Add(removedUpload);

            //producer removed in third upload
            ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult removedUpload1 = CreateRemovedProducerResults("WEE/FA0000KE/SCH", 2007, new DateTime(2016, 1, 3), 1);
            results.ProducerRemovedFromReturnsData.Add(removedUpload1);

            //In this upload producer added back again with same data as first upload
            ProducerEeeHistoryCsvData.ProducerInReturnsResult upload2 = CreateEeeDataForProducer("WEE/MM0841AA", "WEE/FA0000KE/SCH", "Test Scheme", 2007, 1, new DateTime(2016, 1, 4), 200, "Yes");
            results.ProducerReturnsHistoryData.Add(upload2);

            //Act
            IEnumerable<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult> csvResults = new List<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult>();
            csvResults = handler.CreateResults(results);

            //Assert
            Assert.NotNull(csvResults);
            Assert.Equal(3, csvResults.Count());
            Assert.Collection(csvResults,
               (r1) => Assert.Equal(new DateTime(2016, 1, 4), r1.SubmittedDate),
               (r2) => Assert.Equal(new DateTime(2016, 1, 2), r2.SubmittedDate),
               (r3) => Assert.Equal(new DateTime(2016, 1, 1), r3.SubmittedDate));

            Assert.Collection(csvResults,
               (r1) => Assert.Equal("Yes", r1.LatestData),
               (r2) => Assert.Equal("No", r2.LatestData),
               (r3) => Assert.Equal("No", r3.LatestData));

            Assert.Collection(csvResults,
              (r1) => Assert.Equal(200, r1.Cat1B2B),
              (r2) => Assert.Equal(null, r2.Cat1B2B),
              (r3) => Assert.Equal(100, r3.Cat1B2B));
        }

        /// <summary>
        /// This test ensures that only one blank row is added 
        /// for producer which has been removed in the subsequent uploads after the first upload
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateResults_WithProducerRemovedFromSubsequentUploads_ReturnsEeeDataHistoryWithOneBlankDataRowforProducer()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            //first upload
            ProducerEeeHistoryCsvData results = new ProducerEeeHistoryCsvData();
            ProducerEeeHistoryCsvData.ProducerInReturnsResult upload1 = CreateEeeDataForProducer("WEE/MM0841AA", "WEE/FA0000KE/SCH", "Test Scheme", 2007, 1, new DateTime(2016, 1, 1), 100, "Yes");
            results.ProducerReturnsHistoryData.Add(upload1);

            //producer removed in second upload
            ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult removedUpload = CreateRemovedProducerResults("WEE/FA0000KE/SCH", 2007, new DateTime(2016, 1, 2), 1);
            results.ProducerRemovedFromReturnsData.Add(removedUpload);

            //producer removed in third upload
            ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult removedUpload1 = CreateRemovedProducerResults("WEE/FA0000KE/SCH", 2007, new DateTime(2016, 1, 3), 1);
            results.ProducerRemovedFromReturnsData.Add(removedUpload1);
           
            //Act
            IEnumerable<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult> csvResults = new List<GetProducerEeeDataHistoryCsvHandler.EeeHistoryCsvResult>();
            csvResults = handler.CreateResults(results);
            
            //Assert
            Assert.NotNull(csvResults);
            Assert.Equal(2, csvResults.Count());
            Assert.Collection(csvResults,
               (r1) => Assert.Equal(new DateTime(2016, 1, 2), r1.SubmittedDate),
               (r2) => Assert.Equal(new DateTime(2016, 1, 1), r2.SubmittedDate));

            Assert.Collection(csvResults,
               (r1) => Assert.Equal("Yes", r1.LatestData),
               (r2) => Assert.Equal("No", r2.LatestData));

            Assert.Collection(csvResults,
              (r1) => Assert.Equal(null, r1.Cat1B2B),
              (r2) => Assert.Equal(100, r2.Cat1B2B));
        }
        private static ProducerEeeHistoryCsvData.ProducerInReturnsResult CreateEeeDataForProducer(string prn, string approvalNumber, string schemeName, int year, 
            int quarter, DateTime date, int cat1b2b, string latestData)
        {          
            ProducerEeeHistoryCsvData.ProducerInReturnsResult eeeDatahistoryResult = new ProducerEeeHistoryCsvData.ProducerInReturnsResult();
            eeeDatahistoryResult.PRN = prn;
            eeeDatahistoryResult.SchemeName = schemeName;
            eeeDatahistoryResult.ApprovalNumber = approvalNumber; 
            eeeDatahistoryResult.ComplianceYear = year;
            eeeDatahistoryResult.Quarter = quarter;
            eeeDatahistoryResult.SubmittedDate = date;
            eeeDatahistoryResult.LatestData = latestData;
            eeeDatahistoryResult.Cat1B2B = cat1b2b;
            return eeeDatahistoryResult;            
        }

        private static ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult CreateRemovedProducerResults(string approvalnumber, int year, DateTime date, int quarter)
        {
            ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult removedResult = new ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult();
            removedResult.ApprovalNumber = approvalnumber;
            removedResult.ComplianceYear = year;
            removedResult.SubmittedDate = date;
            removedResult.Quarter = quarter;
            return removedResult;
        }
    }
}
