namespace EA.Weee.RequestHandlers.Tests.DataAccess.Shared
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using RequestHandlers.Shared;
    using Requests.Shared;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetDataReturnSubmissionHistoryDataAcessTests
    {
        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsAllSubmittedSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";

                var drv2 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, false);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme1, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";

                var drv3 = modelHelper.CreateDataReturnVersion(scheme1, 2016, 2, true);
                database.Model.SaveChanges();

                var dru3 = modelHelper.CreateDataReturnUpload(scheme1, drv3);
                dru3.ComplianceYear = 2016;
                dru3.Quarter = (int?)QuarterType.Q2;
                dru3.FileName = "DataReturnUpload3.xml";

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result.Data);
                Assert.Equal(2, result.Data.Count);
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSpecifiedSchemeSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                Guid anotherOrganisationId = new Guid("d3f40672-d466-4b75-ac94-fd848879d432");
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var scheme2 = modelHelper.CreateScheme();
                scheme2.Organisation.Id = anotherOrganisationId;
                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";

                var drv2 = modelHelper.CreateDataReturnVersion(scheme2, 2015, 2, true);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme2, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";

                var drv3 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, true);
                database.Model.SaveChanges();

                var dru3 = modelHelper.CreateDataReturnUpload(scheme1, drv3);
                dru3.ComplianceYear = 2015;
                dru3.Quarter = (int?)QuarterType.Q2;
                dru3.FileName = "DataReturnUpload3.xml";

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result.Data);
                Assert.Equal(2, result.Data.Count);
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSchemeSubmissionHistoryDataOrderByDescendingSubmissionDateTime_AsDefaultSort()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                Guid anotherOrganisationId = new Guid("d3f40672-d466-4b75-ac94-fd848879d432");
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";
                dru1.DataReturnVersion.SubmittedDate = DateTime.Today;

                var drv2 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, true);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme1, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";
                dru2.DataReturnVersion.SubmittedDate = DateTime.Today.AddDays(1);

                var drv3 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, true);
                database.Model.SaveChanges();

                var dru3 = modelHelper.CreateDataReturnUpload(scheme1, drv3);
                dru3.ComplianceYear = 2015;
                dru3.Quarter = (int?)QuarterType.Q2;
                dru3.FileName = "DataReturnUpload3.xml";
                dru3.DataReturnVersion.SubmittedDate = DateTime.Today.AddDays(2);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id);

                // Assert
                Assert.NotNull(result.Data);
                Assert.Equal(3, result.Data.Count);
                Assert.Equal("DataReturnUpload3.xml", result.Data[0].FileName);
                Assert.Equal("DataReturnUpload2.xml", result.Data[1].FileName);
                Assert.Equal("DataReturnUpload1.xml", result.Data[2].FileName);
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSpecifiedSchemeAndComplianceYearSubmissionHistoryData()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                Guid anotherOrganisationId = new Guid("d3f40672-d466-4b75-ac94-fd848879d432");
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;
                database.Model.SaveChanges();

                var scheme2 = modelHelper.CreateScheme();
                scheme2.Organisation.Id = anotherOrganisationId;
                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";

                var drv2 = modelHelper.CreateDataReturnVersion(scheme2, 2015, 2, true);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme2, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";

                var drv3 = modelHelper.CreateDataReturnVersion(scheme1, 2016, 2, true);
                database.Model.SaveChanges();

                var dru3 = modelHelper.CreateDataReturnUpload(scheme1, drv3);
                dru3.ComplianceYear = 2016;
                dru3.Quarter = (int?)QuarterType.Q2;
                dru3.FileName = "DataReturnUpload3.xml";

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme1.Id, 2015);

                // Assert
                Assert.NotNull(result.Data);
                Assert.Equal(1, result.Data.Count);
                Assert.Equal("DataReturnUpload1.xml", result.Data[0].FileName);
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSubmissionHistoryData_SortedByDateDescending_WhenOrderBySubmissionDateDescendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion1 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload1 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 5);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload2 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 3);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2016, 2, true);
                var dataReturnUpload3 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion3);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 1);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id, ordering: DataReturnSubmissionsHistoryOrderBy.SubmissionDateDescending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 5), r1.SubmissionDateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 3), r2.SubmissionDateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 1), r3.SubmissionDateTime));
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSubmissionHistoryData_SortedByDateAscending_WhenOrderBySubmissionDateAscendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion1 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload1 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 5);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload2 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 3);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2016, 2, true);
                var dataReturnUpload3 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion3);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 1);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id, ordering: DataReturnSubmissionsHistoryOrderBy.SubmissionDateAscending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 1), r1.SubmissionDateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 3), r2.SubmissionDateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 5), r3.SubmissionDateTime));
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSubmissionHistoryData_SortedByYearDescendingSubmissionDateDescending_WhenOrderByComplianceYearDescendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion1 = modelHelper.CreateDataReturnVersion(scheme, 2014, 2, true);
                var dataReturnUpload1 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 5);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload2 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 3);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload3 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion3);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 1);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id, ordering: DataReturnSubmissionsHistoryOrderBy.ComplianceYearDescending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 3), r1.SubmissionDateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 1), r2.SubmissionDateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 5), r3.SubmissionDateTime));

                Assert.Collection(result.Data,
                    r1 => Assert.Equal(2015, r1.ComplianceYear),
                    r2 => Assert.Equal(2015, r2.ComplianceYear),
                    r3 => Assert.Equal(2014, r3.ComplianceYear));
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSubmissionHistoryData_SortedByYearAscendingSubmissionDateDescending_WhenOrderByComplianceYearAscendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion1 = modelHelper.CreateDataReturnVersion(scheme, 2014, 2, true);
                var dataReturnUpload1 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 5);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload2 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 1);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload3 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion3);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id, ordering: DataReturnSubmissionsHistoryOrderBy.ComplianceYearAscending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 5), r1.SubmissionDateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 3), r2.SubmissionDateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 1), r3.SubmissionDateTime));

                Assert.Collection(result.Data,
                    r1 => Assert.Equal(2014, r1.ComplianceYear),
                    r2 => Assert.Equal(2015, r2.ComplianceYear),
                    r3 => Assert.Equal(2015, r3.ComplianceYear));
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSubmissionHistoryData_SortedByYearDescendingQuarterDescendingSubmissionDateDescending_WhenOrderByQuaterDescendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion1 = modelHelper.CreateDataReturnVersion(scheme, 2014, 2, true);
                var dataReturnUpload1 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 5);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload2 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 1);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                var dataReturnUpload3 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion3);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);

                var dataReturnVersion4 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                var dataReturnUpload4 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion4);
                dataReturnVersion4.SubmittedDate = new DateTime(2015, 1, 6);

                var dataReturnVersion5 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload5 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion5);
                dataReturnVersion5.SubmittedDate = new DateTime(2015, 1, 7);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id, ordering: DataReturnSubmissionsHistoryOrderBy.QuarterDescending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 7), r1.SubmissionDateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 1), r2.SubmissionDateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 6), r3.SubmissionDateTime),
                    r4 => Assert.Equal(new DateTime(2015, 1, 3), r4.SubmissionDateTime),
                    r5 => Assert.Equal(new DateTime(2015, 1, 5), r5.SubmissionDateTime));

                Assert.Collection(result.Data,
                    r1 => Assert.Equal(2015, r1.ComplianceYear),
                    r2 => Assert.Equal(2015, r2.ComplianceYear),
                    r3 => Assert.Equal(2015, r3.ComplianceYear),
                    r4 => Assert.Equal(2015, r4.ComplianceYear),
                    r5 => Assert.Equal(2014, r5.ComplianceYear));

                Assert.Collection(result.Data,
                    r1 => Assert.Equal(QuarterType.Q2, r1.Quarter),
                    r2 => Assert.Equal(QuarterType.Q2, r2.Quarter),
                    r3 => Assert.Equal(QuarterType.Q1, r3.Quarter),
                    r4 => Assert.Equal(QuarterType.Q1, r4.Quarter),
                    r5 => Assert.Equal(QuarterType.Q2, r5.Quarter));
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSubmissionHistoryData_SortedByYearDescendingQuarterAscendingSubmissionDateDescending_WhenOrderByQuaterAscendingSpecified()
        {
            using (var database = new DatabaseWrapper())
            {
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion1 = modelHelper.CreateDataReturnVersion(scheme, 2014, 2, true);
                var dataReturnUpload1 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 5);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload2 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 1);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                var dataReturnUpload3 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion3);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);

                var dataReturnVersion4 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                var dataReturnUpload4 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion4);
                dataReturnVersion4.SubmittedDate = new DateTime(2015, 1, 6);

                var dataReturnVersion5 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload5 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion5);
                dataReturnVersion5.SubmittedDate = new DateTime(2015, 1, 7);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id, ordering: DataReturnSubmissionsHistoryOrderBy.QuarterAscending);

                // Assert
                Assert.Collection(result.Data,
                    r1 => Assert.Equal(new DateTime(2015, 1, 6), r1.SubmissionDateTime),
                    r2 => Assert.Equal(new DateTime(2015, 1, 3), r2.SubmissionDateTime),
                    r3 => Assert.Equal(new DateTime(2015, 1, 7), r3.SubmissionDateTime),
                    r4 => Assert.Equal(new DateTime(2015, 1, 1), r4.SubmissionDateTime),
                    r5 => Assert.Equal(new DateTime(2015, 1, 5), r5.SubmissionDateTime));

                Assert.Collection(result.Data,
                    r1 => Assert.Equal(2015, r1.ComplianceYear),
                    r2 => Assert.Equal(2015, r2.ComplianceYear),
                    r3 => Assert.Equal(2015, r3.ComplianceYear),
                    r4 => Assert.Equal(2015, r4.ComplianceYear),
                    r5 => Assert.Equal(2014, r5.ComplianceYear));

                Assert.Collection(result.Data,
                    r1 => Assert.Equal(QuarterType.Q1, r1.Quarter),
                    r2 => Assert.Equal(QuarterType.Q1, r2.Quarter),
                    r3 => Assert.Equal(QuarterType.Q2, r3.Quarter),
                    r4 => Assert.Equal(QuarterType.Q2, r4.Quarter),
                    r5 => Assert.Equal(QuarterType.Q2, r5.Quarter));
            }
        }

        [Fact]
        public async void GetDataReturnSubmissionHistory_ReturnsSubmissionDataCount()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var dataReturnVersion1 = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);
                var dataReturnUpload1 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 5);

                var dataReturnVersion2 = modelHelper.CreateDataReturnVersion(scheme, 2015, 2, true);
                var dataReturnUpload2 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 3);

                var dataReturnVersion3 = modelHelper.CreateDataReturnVersion(scheme, 2016, 2, true);
                var dataReturnUpload3 = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion3);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 1);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id);

                // Assert
                Assert.Equal(3, result.ResultCount);
            }
        }

        [Fact]
        public async Task GetDataReturnSubmissionHistory_ResultDoesNotIncludeSummaryData_WhenIncludeSummaryDataParameterFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2015;

                var producer = modelHelper.CreateProducerAsCompany(memberUpload, "abc");

                var dataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);

                var b2bEeeOutputAmount = modelHelper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2B", 1, 10);
                var b2cEeeOutputAmount = modelHelper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 10);

                var b2bWeeeCollected = modelHelper.CreateWeeeCollectedAmount(dataReturnVersion, "B2B", 1, 10);
                var b2cWeeeCollected = modelHelper.CreateWeeeCollectedAmount(dataReturnVersion, "B2C", 1, 10);

                var location = modelHelper.CreateAatfDeliveryLocation("AAA", "A Site");
                var b2bWeeeDelivered = modelHelper.CreateWeeeDeliveredAmount(dataReturnVersion, location, "B2B", 1, 10);
                var b2cWeeeDelivered = modelHelper.CreateWeeeDeliveredAmount(dataReturnVersion, location, "B2C", 1, 10);

                var dataReturnUpload = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id, includeSummaryData: false);

                // Assert
                Assert.Single(result.Data);

                var data = result.Data.Single();

                Assert.Null(data.EeeOutputB2b);
                Assert.Null(data.EeeOutputB2c);
                Assert.Null(data.WeeeCollectedB2b);
                Assert.Null(data.WeeeCollectedB2c);
                Assert.Null(data.WeeeDeliveredB2b);
                Assert.Null(data.WeeeDeliveredB2c);
            }
        }

        [Fact]
        public async Task GetDataReturnSubmissionHistory_ResultIncludesSummaryData_WhenIncludeSummaryDataParameterTrue()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                GetDataReturnSubmissionsHistoryResultsDataAccess dataAccess = new GetDataReturnSubmissionsHistoryResultsDataAccess(database.WeeeContext);

                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();

                var memberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2015;

                var producer = modelHelper.CreateProducerAsCompany(memberUpload, "abc");

                var dataReturnVersion = modelHelper.CreateDataReturnVersion(scheme, 2015, 1, true);

                var b2bEeeOutputAmount = modelHelper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2B", 1, 10);
                var b2cEeeOutputAmount = modelHelper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 10);

                var b2bWeeeCollected = modelHelper.CreateWeeeCollectedAmount(dataReturnVersion, "B2B", 1, 10);
                var b2cWeeeCollected = modelHelper.CreateWeeeCollectedAmount(dataReturnVersion, "B2C", 1, 10);

                var location = modelHelper.CreateAatfDeliveryLocation("AAA", "A Site");
                var b2bWeeeDelivered = modelHelper.CreateWeeeDeliveredAmount(dataReturnVersion, location, "B2B", 1, 10);
                var b2cWeeeDelivered = modelHelper.CreateWeeeDeliveredAmount(dataReturnVersion, location, "B2C", 1, 10);

                var dataReturnUpload = modelHelper.CreateDataReturnUpload(scheme, dataReturnVersion);

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetDataReturnSubmissionsHistory(scheme.Id, includeSummaryData: true);

                // Assert
                Assert.Single(result.Data);

                var data = result.Data.Single();

                Assert.NotNull(data.EeeOutputB2b);
                Assert.NotNull(data.EeeOutputB2c);
                Assert.NotNull(data.WeeeCollectedB2b);
                Assert.NotNull(data.WeeeCollectedB2c);
                Assert.NotNull(data.WeeeDeliveredB2b);
                Assert.NotNull(data.WeeeDeliveredB2c);
            }
        }
    }
}
