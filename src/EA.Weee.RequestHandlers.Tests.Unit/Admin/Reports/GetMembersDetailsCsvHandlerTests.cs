namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using FakeItEasy;
    using RequestHandlers.Admin.Reports;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class GetMembersDetailsCsvHandlerTests
    {
        [Fact]
        public async Task GetMembersDetailsCSVHandler_NotInternalUser_ThrowsSecurityException()
        {
            // Arrange
            var complianceYear = 2016;

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriter = A.Fake<ICsvWriter<MembersDetailsCsvData>>();

            var handler = new GetMembersDetailsCsvHandler(authorization, context, csvWriter);
            var request = new GetMemberDetailsCsv(complianceYear);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetMembersDetailsCSVHandler_NoComplianceYear_ThrowsArgumentException()
        {
            // Arrange
            var complianceYear = 0;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriter = A.Fake<ICsvWriter<MembersDetailsCsvData>>();

            var handler = new GetMembersDetailsCsvHandler(authorization, context, csvWriter);
            var request = new GetMemberDetailsCsv(complianceYear);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
        
        [Fact]
        public async Task GetMembersDetailsCSVHandler_ComplianceYear_ReturnsFileContent()
        {
            // Arrange
            var complianceYear = 2016;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriter = new CsvWriter<MembersDetailsCsvData>(A.Dummy<IExcelSanitizer>());

            var handler = new GetMembersDetailsCsvHandler(authorization, context, csvWriter);
            var request = new GetMemberDetailsCsv(complianceYear);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetMembersDetailsCSVHandler_WithoutBrandNames_DoesNotIncludeBrandNamesColumn()
        {
            // Arrange
            var complianceYear = 2016;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriter = A.Fake<ICsvWriter<MembersDetailsCsvData>>();

            var handler = new GetMembersDetailsCsvHandler(authorization, context, csvWriter);
            var request = new GetMemberDetailsCsv(complianceYear, includeBrandNames: false);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => csvWriter.DefineColumn("Brand names", A<Func<MembersDetailsCsvData, object>>._, A<bool>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetMembersDetailsCSVHandler_WithBrandNames_IncludesBrandNamesColumn()
        {
            // Arrange
            var complianceYear = 2016;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriter = A.Fake<ICsvWriter<MembersDetailsCsvData>>();

            var handler = new GetMembersDetailsCsvHandler(authorization, context, csvWriter);
            var request = new GetMemberDetailsCsv(complianceYear, includeBrandNames: true);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => csvWriter.DefineColumn("Brand names", A<Func<MembersDetailsCsvData, object>>._, A<bool>._))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetMembersDetailsCSVHandler_WithBrandNamesLongerThanMaxLength_ThrowsException()
        {
            // Arrange
            var complianceYear = 2016;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var storedProcedures = A.Fake<IStoredProcedures>();
            var csvWriter = A.Fake<ICsvWriter<MembersDetailsCsvData>>();

            A.CallTo(() => context.StoredProcedures)
                .Returns(storedProcedures);

            var csvData1 = new MembersDetailsCsvData
            {
                ProducerName = "Producer1",
                BrandNames = new string('A', GetMembersDetailsCsvHandler.MaxBrandNamesLength + 1)
            };

            var csvData2 = new MembersDetailsCsvData
            {
                ProducerName = "Producer2",
                BrandNames = "BrandName2"
            };

            var csvData3 = new MembersDetailsCsvData
            {
                ProducerName = "Producer3",
                BrandNames = new string('A', GetMembersDetailsCsvHandler.MaxBrandNamesLength + 1)
            };

            A.CallTo(() => storedProcedures
            .SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(A<int>._, A<bool>._, A<bool>._, A<Guid>._, A<Guid>._))
            .Returns(new List<MembersDetailsCsvData> { csvData1, csvData2, csvData3 });

            var handler = new GetMembersDetailsCsvHandler(authorization, context, csvWriter);
            var request = new GetMemberDetailsCsv(complianceYear, false, Guid.NewGuid(), Guid.NewGuid(), true);

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            // Assert
            Assert.NotNull(exception);
            Assert.Contains("Producer1", exception.Message);
            Assert.Contains("Producer3", exception.Message);
            Assert.Contains("brand names", exception.Message);
        }
    }
}