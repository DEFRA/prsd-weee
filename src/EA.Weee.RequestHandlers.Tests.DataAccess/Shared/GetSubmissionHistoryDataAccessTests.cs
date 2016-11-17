namespace EA.Weee.RequestHandlers.Tests.DataAccess.Shared
{
    using RequestHandlers.Shared;
    using Requests.Shared;
    using System;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetSubmissionHistoryDataAccessTests
    {
        [Fact]
        public async void GetSubmissionHistory_WitValidSchemeId_ReturnsAllYearsSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var user1 = modelHelper.CreateUser("Test");
                database.Model.SaveChanges();

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.SubmittedDate = new DateTime(2015, 1, 1);

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2015;
                mu2.IsSubmitted = true;
                mu2.SubmittedDate = new DateTime(2015, 1, 2);

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.SubmittedDate = new DateTime(2015, 1, 3);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result.Data);
                Assert.Equal(3, result.Data.Count);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_WitValidSchemeIdAndYear2015_ReturnsOnly2015YearSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var user1 = modelHelper.CreateUser("Test");
                database.Model.SaveChanges();

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.SubmittedDate = new DateTime(2015, 1, 1);

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2015;
                mu2.IsSubmitted = true;
                mu2.SubmittedDate = new DateTime(2015, 1, 1);

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.SubmittedDate = new DateTime(2015, 1, 1);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id, 2015);

                // Assert
                Assert.NotNull(result.Data);
                Assert.Equal(2, result.Data.Count);
                Assert.Collection(result.Data,
                                  r1 => Assert.Equal(r1.Year, 2015),
                                  r2 => Assert.Equal(r2.Year, 2015));
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsSubmissionHistoryData_SortedBySubmissionDateDescending_AsDefaultSort()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.SubmittedDate = new DateTime(2015, 1, 1);

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2018;
                mu2.IsSubmitted = true;
                mu2.SubmittedDate = new DateTime(2015, 1, 2);

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.SubmittedDate = new DateTime(2015, 1, 3);

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 3), r1.DateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 2), r2.DateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 1), r3.DateTime));
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsSubmissionHistoryData_SortedByDateDescending_WhenOrderBySubmissionDateDescendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.SubmittedDate = new DateTime(2015, 1, 1);

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2018;
                mu2.IsSubmitted = true;
                mu2.SubmittedDate = new DateTime(2015, 1, 2);

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.SubmittedDate = new DateTime(2015, 1, 3);

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id, ordering: SubmissionsHistoryOrderBy.SubmissionDateDescending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 3), r1.DateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 2), r2.DateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 1), r3.DateTime));
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsSubmissionHistoryData_SortedByDateAscending_WhenOrderBySubmissionDateAscendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.SubmittedDate = new DateTime(2015, 1, 1);

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2018;
                mu2.IsSubmitted = true;
                mu2.SubmittedDate = new DateTime(2015, 1, 2);

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.SubmittedDate = new DateTime(2015, 1, 3);

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id, ordering: SubmissionsHistoryOrderBy.SubmissionDateAscending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 1), r1.DateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 2), r2.DateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 3), r3.DateTime));
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsSubmissionHistoryData_SortedByYearDescendingSubmissionDateDescending_WhenOrderByComplianceYearDescendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.SubmittedDate = new DateTime(2015, 1, 10);

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2018;
                mu2.IsSubmitted = true;
                mu2.SubmittedDate = new DateTime(2015, 1, 1);

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2015;
                mu3.IsSubmitted = true;
                mu3.SubmittedDate = new DateTime(2015, 1, 3);

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id, ordering: SubmissionsHistoryOrderBy.ComplianceYearDescending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 1), r1.DateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 10), r2.DateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 3), r3.DateTime));

                Assert.Collection(result.Data,
                    r1 => Assert.Equal(2018, r1.Year),
                    r2 => Assert.Equal(2015, r2.Year),
                    r3 => Assert.Equal(2015, r3.Year));
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsSubmissionHistoryData_SortedByYearAscendingSubmissionDateDescending_WhenOrderByComplianceYearAscendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.SubmittedDate = new DateTime(2015, 1, 10);

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2018;
                mu2.IsSubmitted = true;
                mu2.SubmittedDate = new DateTime(2015, 1, 5);

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2015;
                mu3.IsSubmitted = true;
                mu3.SubmittedDate = new DateTime(2015, 1, 3);

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id, ordering: SubmissionsHistoryOrderBy.ComplianceYearAscending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 10), r1.DateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 3), r2.DateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 5), r3.DateTime));

                Assert.Collection(result.Data,
                    r1 => Assert.Equal(2015, r1.Year),
                    r2 => Assert.Equal(2015, r2.Year),
                    r3 => Assert.Equal(2018, r3.Year));
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsSubmissionHistoryDataCount()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();

                var mu1 = modelHelper.CreateMemberUpload(scheme1);
                mu1.ComplianceYear = 2015;
                mu1.IsSubmitted = true;
                mu1.SubmittedDate = new DateTime(2015, 1, 1);

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2018;
                mu2.IsSubmitted = true;
                mu2.SubmittedDate = new DateTime(2015, 1, 2);

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.SubmittedDate = new DateTime(2015, 1, 3);

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.Equal(3, result.ResultCount);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_DoesNotReturnSummaryData_WhenIncludeSummaryDataFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                modelHelper.CreateProducerAsCompany(memberUpload, "1234");

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme.Id, includeSummaryData: false);

                // Assert
                Assert.Single(result.Data);
                Assert.Null(result.Data[0].NumberOfChangedProducers);
                Assert.Null(result.Data[0].NumberOfNewProducers);
                Assert.Null(result.Data[0].ProducersChanged);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsProducerAsNew_WhenNoExistingProducerSubmissionForSameYearAndScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme.Id, includeSummaryData: true);

                // Assert
                Assert.Single(result.Data);
                Assert.Equal(0, result.Data[0].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[0].NumberOfNewProducers);
                Assert.True(result.Data[0].ProducersChanged);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsProducerAsNew_WhenExistingProducerUploadButNoSubmissionForSameYearAndScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload1 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = false;

                modelHelper.CreateProducerAsCompany(memberUpload1, "1111");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme.Id, includeSummaryData: true);

                // Assert
                Assert.Single(result.Data);
                Assert.Equal(0, result.Data[0].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[0].NumberOfNewProducers);
                Assert.True(result.Data[0].ProducersChanged);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsProducerAsNew_ProducerRegisteredForDifferentSchemeWithinSameYear_ButFirstSubmissionForCurrentScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload, "1111", "B2B");

                var scheme2 = modelHelper.CreateScheme();

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111", "B2C");

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme2.Id, includeSummaryData: true);

                // Assert
                Assert.Single(result.Data);
                Assert.Equal(0, result.Data[0].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[0].NumberOfNewProducers);
                Assert.True(result.Data[0].ProducersChanged);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsProducerAsNew_WhenRegisteredForSameSchemeInAnotherYear_ButNoExistingProducerSubmissionForCurrentYear()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2017;

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme.Id, includeSummaryData: true);

                // Assert
                Assert.Equal(2, result.Data.Count);

                Assert.Equal(0, result.Data[0].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[0].NumberOfNewProducers);
                Assert.True(result.Data[0].ProducersChanged);

                Assert.Equal(0, result.Data[1].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[1].NumberOfNewProducers);
                Assert.True(result.Data[1].ProducersChanged);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsProducerAsChanged_WhenSubmissionForSameYearAndScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                modelHelper.CreateProducerAsCompany(memberUpload, "1111");

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 2, 2);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                var memberUpload3 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload3, "1111");
                modelHelper.CreateProducerAsCompany(memberUpload3, "2222");

                var memberUpload4 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.SubmittedDate = new DateTime(2016, 4, 4);

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme.Id, ordering: SubmissionsHistoryOrderBy.SubmissionDateAscending, includeSummaryData: true);

                // Assert
                Assert.Equal(4, result.Data.Count);

                Assert.Equal(0, result.Data[0].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[0].NumberOfNewProducers);
                Assert.True(result.Data[0].ProducersChanged);

                Assert.Equal(1, result.Data[1].NumberOfChangedProducers);
                Assert.Equal(0, result.Data[1].NumberOfNewProducers);
                Assert.True(result.Data[1].ProducersChanged);

                Assert.Equal(1, result.Data[2].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[2].NumberOfNewProducers);
                Assert.True(result.Data[2].ProducersChanged);

                Assert.Equal(0, result.Data[3].NumberOfChangedProducers);
                Assert.Equal(0, result.Data[3].NumberOfNewProducers);
                Assert.False(result.Data[3].ProducersChanged);
            }
        }

        [Fact]
        public async void GetSubmissionHistory_ReturnsProducerAsNew_WhenRemovedPriorToSubmission()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.SubmittedDate = new DateTime(2016, 1, 1);

                var producer = modelHelper.CreateProducerAsCompany(memberUpload, "1111");
                producer.RegisteredProducer.Removed = true;
                producer.RegisteredProducer.RemovedDate = new DateTime(2016, 1, 2);

                var memberUpload2 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.SubmittedDate = new DateTime(2016, 2, 2);

                modelHelper.CreateProducerAsCompany(memberUpload2, "1111");

                var memberUpload3 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.SubmittedDate = new DateTime(2016, 3, 3);

                modelHelper.CreateProducerAsCompany(memberUpload3, "1111");

                var memberUpload4 = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload4.ComplianceYear = 2016;
                memberUpload4.SubmittedDate = new DateTime(2016, 4, 4);

                database.Model.SaveChanges();

                GetSubmissionsHistoryResultsDataAccess dataAccess = new GetSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetSubmissionsHistory(scheme.Id, ordering: SubmissionsHistoryOrderBy.SubmissionDateAscending, includeSummaryData: true);

                // Assert
                Assert.Equal(4, result.Data.Count);

                Assert.Equal(0, result.Data[0].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[0].NumberOfNewProducers);
                Assert.True(result.Data[0].ProducersChanged);

                Assert.Equal(0, result.Data[1].NumberOfChangedProducers);
                Assert.Equal(1, result.Data[1].NumberOfNewProducers);
                Assert.True(result.Data[1].ProducersChanged);

                Assert.Equal(1, result.Data[2].NumberOfChangedProducers);
                Assert.Equal(0, result.Data[2].NumberOfNewProducers);
                Assert.True(result.Data[2].ProducersChanged);

                Assert.Equal(0, result.Data[3].NumberOfChangedProducers);
                Assert.Equal(0, result.Data[3].NumberOfNewProducers);
                Assert.False(result.Data[3].ProducersChanged);
            }
        }
    }
}
