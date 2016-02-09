﻿namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.FetchSummaryCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;
    using Domain;
    using EA.Weee.Core.Shared;
    using EA.Weee.RequestHandlers.DataReturns.FetchSummaryCsv;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Prsd.Core;
    using Xunit;

    public class FetchSummaryCsvTests
    {
        /// <summary>
        /// This test ensures that a SecurityException is thrown when a user with no internal or organisation access
        /// tries to use the handler.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNoInternalOrOrganisationAccess_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .DenyInternalOrOrganisationAccess()
                .Build();

            FetchSummaryCsvHandler handler = new FetchSummaryCsvHandler(
                authorization,
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IFetchSummaryCsvDataAccess>());

            // Act
            Func<Task<FileInfo>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.DataReturns.FetchSummaryCsv>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        /// <summary>
        /// This test ensures that the filename generated by the handler contains the scheme approval number,
        /// the compliance year from the request and the current local time. Note that the forward slashes in the scheme
        /// approval number are removed later.
        /// The expected format is: [scheme approval number]_EEE_WEEE_data_[compliance year]_[DDMMYYYY_HHMM].csv
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_Always_CreatesFileNameWithSchemeApprovalNumberComplianceYearAndCurrentTime()
        {
            // Arrange
            Domain.Scheme.Scheme scheme = new Domain.Scheme.Scheme(A.Dummy<Domain.Organisation.Organisation>());
            scheme.UpdateScheme(
                "Scheme name",
                "WEE/AB1234CD/SCH",
                A.Dummy<string>(),
                A.Dummy<Domain.Obligation.ObligationType?>(),
                A.Dummy<UKCompetentAuthority>());

            IFetchSummaryCsvDataAccess dataAccess = A.Fake<IFetchSummaryCsvDataAccess>();
            A.CallTo(() => dataAccess.FetchSchemeAsync(A<Guid>._))
                .Returns(scheme);

            FetchSummaryCsvHandler handler = new FetchSummaryCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>(),
                dataAccess);

            // Act
            Requests.DataReturns.FetchSummaryCsv request = new Requests.DataReturns.FetchSummaryCsv(
                A.Dummy<Guid>(),
                2017);

            SystemTime.Freeze(new DateTime(2016, 1, 2, 15, 22, 59), true);
            FileInfo result = await handler.HandleAsync(request);
            SystemTime.Unfreeze();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WEE/AB1234CD/SCH_EEE_WEEE_data_2017_02012016_1522.csv", result.FileName);
        }

        /// <summary>
        /// This test ensures that the CsvWriter will define columns for "Quarter",
        /// "EEE or WEEE in tonnes (t)", "Obligation type" and a column for each
        /// of the fourteen categories; i.e. "Cat 1 (t)", "Cat 2 (t)", etc.
        /// </summary>
        [Fact]
        public void CreateWriter_Always_CreatesExpectedColumns()
        {
            // Arrange
            FetchSummaryCsvHandler handler = new FetchSummaryCsvHandler(
                A.Dummy<IWeeeAuthorization>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IFetchSummaryCsvDataAccess>());

            // Act
            CsvWriter<DataReturnSummaryCsvData> csvWriter = handler.CreateWriter();

            // Assert
            Assert.NotNull(csvWriter);
            Assert.Collection(csvWriter.ColumnTitles,
                c => Assert.Equal("Quarter", c),
                c => Assert.Equal("EEE or WEEE in tonnes (t)", c),
                c => Assert.Equal("Obligation type", c),
                c => Assert.Equal("Cat 1 (t)", c),
                c => Assert.Equal("Cat 2 (t)", c),
                c => Assert.Equal("Cat 3 (t)", c),
                c => Assert.Equal("Cat 4 (t)", c),
                c => Assert.Equal("Cat 5 (t)", c),
                c => Assert.Equal("Cat 6 (t)", c),
                c => Assert.Equal("Cat 7 (t)", c),
                c => Assert.Equal("Cat 8 (t)", c),
                c => Assert.Equal("Cat 9 (t)", c),
                c => Assert.Equal("Cat 10 (t)", c),
                c => Assert.Equal("Cat 11 (t)", c),
                c => Assert.Equal("Cat 12 (t)", c),
                c => Assert.Equal("Cat 13 (t)", c),
                c => Assert.Equal("Cat 14 (t)", c));
        }
    }
}