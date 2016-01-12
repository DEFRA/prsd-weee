namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;
    using EA.Prsd.Core;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Admin.Reports;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Reports;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Xunit;

    public class GetSchemeWeeeCsvHandlerTests
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
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                authorization,
                A.Dummy<CsvWriterFactory>());

            GetSchemeWeeeCsv request = new GetSchemeWeeeCsv(A.Dummy<int>(), A.Dummy<ObligationType>());

            // Act
            Func<Task<FileInfo>> testCode = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        /// <summary>
        /// This test ensures that the handler generates a file with a name in the following
        /// format: "2016_B2C_schemeWEEEE_31122016_2359.csv".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_Always_GeneratesCorrectFileName()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                authorization,
                A.Dummy<CsvWriterFactory>());

            GetSchemeWeeeCsv request = new GetSchemeWeeeCsv(2016, ObligationType.B2C);

            // Act
            SystemTime.Freeze(new DateTime(2016, 12, 31, 23, 59, 0));
            FileInfo result = await handler.HandleAsync(request);
            SystemTime.Unfreeze();

            // Assert
            Assert.Equal("2016_B2C_schemeWEEEE_31122016_2359.csv", result.FileName);
        }

        /// <summary>
        /// This test ensures that the CSV writer created for a B2C report contains
        /// the columns "Scheme Name", "Scheme approval No", "Quarter", "Category",
        /// "DCF", "Distributors", "Final holders" and "Total AATF/AE", followed by a column for each
        /// AATF and a column for each AE.
        /// </summary>
        [Fact]
        public void CreateWriter_WithB2C_CreatesExpectedColumns()
        {
            // Arrange
            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            List<string> aatfLocations = new List<string>()
            {
                "AATF1",
                "AATF2"
            };

            List<string> aaeLocations = new List<string>()
            {
                "AE1",
                "AE2"
            };

            // Act
            CsvWriter<GetSchemeWeeeCsvHandler.CsvResult> result = handler.CreateWriter(
                ObligationType.B2C,
                aatfLocations,
                aaeLocations);

            // Assert
            Assert.Collection(result.ColumnTitles,
                title => Assert.Equal("Scheme name", title),
                title => Assert.Equal("Scheme approval No", title),
                title => Assert.Equal("Quarter", title),
                title => Assert.Equal("Category", title),
                title => Assert.Equal("DCF", title),
                title => Assert.Equal("Distributors", title),
                title => Assert.Equal("Final holders", title),
                title => Assert.Equal("Total AATF/AE", title),
                title => Assert.Equal("AATF1", title),
                title => Assert.Equal("AATF2", title),
                title => Assert.Equal("AE1", title),
                title => Assert.Equal("AE2", title));
        }

        /// <summary>
        /// This test ensures that the CSV writer created for a B2C report contains
        /// the columns "Scheme Name", "Scheme approval No", "Quarter", "Category",
        /// "DCF" and "Total AATF/AE", followed by a column for each
        /// AATF and a column for each AE.
        /// </summary>
        [Fact]
        public void CreateWriter_WithB2B_CreatesExpectedColumns()
        {
            // Arrange
            List<string> aatfLocations = new List<string>()
            {
                "AATF1",
                "AATF2"
            };

            List<string> aaeLocations = new List<string>()
            {
                "AE1",
                "AE2"
            };

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            CsvWriter<GetSchemeWeeeCsvHandler.CsvResult> result = handler.CreateWriter(
                ObligationType.B2B,
                aatfLocations,
                aaeLocations);

            // Assert
            Assert.Collection(result.ColumnTitles,
                title => Assert.Equal("Scheme name", title),
                title => Assert.Equal("Scheme approval No", title),
                title => Assert.Equal("Quarter", title),
                title => Assert.Equal("Category", title),
                title => Assert.Equal("DCF", title),
                title => Assert.Equal("Total AATF/AE", title),
                title => Assert.Equal("AATF1", title),
                title => Assert.Equal("AATF2", title),
                title => Assert.Equal("AE1", title),
                title => Assert.Equal("AE2", title));
        }

        /// <summary>
        /// This test ensures that for every scheme, a CSV row will be created for each category
        /// and each quarter and that the 14 categories for each quarter will be listed
        /// together.
        /// </summary>
        [Fact]
        public void CreateResults_WithOneScheme_CreatesAResultForEachCategoryAndEachQuarter()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();
            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult());

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                A.Dummy<IEnumerable<string>>(),
                A.Dummy<IEnumerable<string>>());

            // Assert
            Assert.Equal(56, results.Count());
            Assert.Collection(results.Take(15), // Just examine the first 15 rows
                r => { Assert.Equal(1, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(2, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(3, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(4, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(5, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(6, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(7, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(8, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(9, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(10, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(11, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(12, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(13, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(14, r.Category); Assert.Equal(1, r.QuarterType); },
                r => { Assert.Equal(1, r.Category); Assert.Equal(2, r.QuarterType); });
            // etc...
        }

        /// <summary>
        /// This test ensures that the CSV result for the DCF will be blank rather than 0
        /// if no collected amounts are returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithNoCollectedAmounts_PopulatesDcfAsNull()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();
            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult());

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                A.Dummy<IEnumerable<string>>(),
                A.Dummy<IEnumerable<string>>());

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(null, result1.Dcf);
        }

        /// <summary>
        /// This test ensures that the CSV result for the DCF will be populated
        /// if a collected amount with source tpye 0 is returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithCollectedAmountWithSourceType0_PopulatesDcfWithValue()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2")
            });

            data.CollectedAmounts.Add(new SpgSchemeWeeeCsvResult.CollectedAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 1,
                WeeeCategory = 1,
                SourceType = 0,
                Tonnage = 123.456m
            });

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                A.Dummy<IEnumerable<string>>(),
                A.Dummy<IEnumerable<string>>());

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(123.456m, result1.Dcf);
        }

        /// <summary>
        /// This test ensures that the CSV result for the distributors will be blank rather than 0
        /// if no collected amounts are returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithNoCollectedAmounts_PopulatesDistributorsAsNull()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();
            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult());

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                A.Dummy<IEnumerable<string>>(),
                A.Dummy<IEnumerable<string>>());

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(null, result1.Distributors);
        }

        /// <summary>
        /// This test ensures that the CSV result for the distributors will be populated
        /// if a collected amount with source type 1 is returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithCollectedAmountWithSourceType1_PopulatesDistributorsWithValue()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2")
            });

            data.CollectedAmounts.Add(new SpgSchemeWeeeCsvResult.CollectedAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 1,
                WeeeCategory = 1,
                SourceType = 1,
                Tonnage = 123.456m
            });

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                A.Dummy<IEnumerable<string>>(),
                A.Dummy<IEnumerable<string>>());

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(123.456m, result1.Distributors);
        }

        /// <summary>
        /// This test ensures that the CSV result for the final holders will be blank rather than 0
        /// if no collected amounts are returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithNoCollectedAmounts_PopulatesFinalHoldersAsNull()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();
            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult());

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                A.Dummy<IEnumerable<string>>(),
                A.Dummy<IEnumerable<string>>());

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(null, result1.FinalHolders);
        }

        /// <summary>
        /// This test ensures that the CSV result for the final holders will be populated
        /// if a collected amount with source type 2 is returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithCollectedAmountWithSourceType2_PopulatesFinalHoldersWithValue()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2")
            });

            data.CollectedAmounts.Add(new SpgSchemeWeeeCsvResult.CollectedAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 1,
                WeeeCategory = 1,
                SourceType = 2,
                Tonnage = 123.456m
            });

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                A.Dummy<IEnumerable<string>>(),
                A.Dummy<IEnumerable<string>>());

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(123.456m, result1.FinalHolders);
        }

        /// <summary>
        /// This test ensures that the CSV result for the Total delivered to AATF/AE will be
        /// null blank than 0 when no delievered amounts are returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithNoDeliveredAmounts_PopulatesTotalAsNull()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();
            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult());

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                new List<string>() { "AATF1" },
                new List<string>() { "AE1" });

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(null, result1.TotalDelivered);
        }

        /// <summary>
        /// This test ensures that the CSV result for the delivered amount to an AATF
        /// and the total will be populated when a delivered amount is returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithDeliveredAmountForLocationType0_PopulatesAatfAmountAndTotal()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2")
            });

            data.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 1,
                WeeeCategory = 1,
                LocationType = 0,
                LocationApprovalNumber = "AATF1",
                Tonnage = 123.456m
            });

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                new List<string>() { "AATF1" },
                new List<string>() { "AE1" });

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(123.456m, result1.AatfTonnage["AATF1"]);
            Assert.Equal(123.456m, result1.TotalDelivered);
        }

        /// <summary>
        /// This test ensures that the CSV result for the delivered amount to an AE
        /// and the total will be populated when a delivered amount is returned in the data.
        /// </summary>
        [Fact]
        public void CreateResults_WithDeliveredAmountForLocationType1_PopulatesAeAmountAndTotal()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2")
            });

            data.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 1,
                WeeeCategory = 1,
                LocationType = 1,
                LocationApprovalNumber = "AE1",
                Tonnage = 123.456m
            });

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                new List<string>() { "AATF1" },
                new List<string>() { "AE1" });

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(123.456m, result1.AeTonnage["AE1"]);
            Assert.Equal(123.456m, result1.TotalDelivered);
        }

        /// <summary>
        /// This test ensures that the CSV result for the total amount is populated as
        /// the sum of delivered amounts returned in the data for the scheme, category and quarter.
        /// </summary>
        [Fact]
        public void CreateResults_WithDeliveredAmountsForVariousSchemesCategoriesAndQuarters_SumsAmountsAndPopulatesTotals()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                SchemeName = "Scheme 1"
            });

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeId = new Guid("9E1EE29C-5FFD-4137-BA55-98C0F0F7212D"),
                SchemeName = "Scheme 2"
            });

            data.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 1,
                WeeeCategory = 1,
                LocationType = 0,
                LocationApprovalNumber = "AATF1",
                Tonnage = 1
            });

            data.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 1,
                WeeeCategory = 1,
                LocationType = 1,
                LocationApprovalNumber = "AE1",
                Tonnage = 2
            });

            data.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 4,
                WeeeCategory = 1,
                LocationType = 1,
                LocationApprovalNumber = "AE1",
                Tonnage = 9999
            });

            data.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
            {
                SchemeId = new Guid("3E299215-BA37-403F-B398-EB345371F3D2"),
                QuarterType = 1,
                WeeeCategory = 14,
                LocationType = 1,
                LocationApprovalNumber = "AE1",
                Tonnage = 9999
            });

            data.DeliveredAmounts.Add(new SpgSchemeWeeeCsvResult.DeliveredAmountResult()
            {
                SchemeId = new Guid("9E1EE29C-5FFD-4137-BA55-98C0F0F7212D"),
                QuarterType = 1,
                WeeeCategory = 1,
                LocationType = 1,
                LocationApprovalNumber = "AE1",
                Tonnage = 9999
            });

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                new List<string>() { "AATF1" },
                new List<string>() { "AE1" });

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal(3, result1.TotalDelivered);
        }

        /// <summary>
        /// This test ensures that the CSV result for the total amount is populated as
        /// the sum of delivered amounts returned in the data for the scheme, category and quarter.
        /// </summary>
        [Fact]
        public void CreateResults_WithMultipleSchemes_OrdersSchemesBySchemeName()
        {
            // Arrange
            SpgSchemeWeeeCsvResult data = new SpgSchemeWeeeCsvResult();

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeName = "Scheme B"
            });

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeName = "Scheme A"
            });

            data.Schemes.Add(new SpgSchemeWeeeCsvResult.SchemeResult()
            {
                SchemeName = "Scheme C"
            });

            GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                A.Dummy<WeeeContext>(),
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>());

            // Act
            IEnumerable<GetSchemeWeeeCsvHandler.CsvResult> results = handler.CreateResults(
                data,
                A.Dummy<IEnumerable<string>>(),
                A.Dummy<IEnumerable<string>>());

            // Assert
            GetSchemeWeeeCsvHandler.CsvResult result1 = results.First();
            Assert.Equal("Scheme A", result1.SchemeName);
        }
    }
}
