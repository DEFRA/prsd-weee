namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin
{
    using System;
    using Core.DataReturns;
    using Core.Shared;
    using RequestHandlers.Admin;
    using Weee.Tests.Core.Model;
    using Xunit;
    public class GetAllComplianceYearsDataAccessTests
    {
        [Fact]
        public async void GetAllComplianceYears_ComplianceYearForIsMemberRegistrations_ReturnsDistinctMemberRegistrationsComplianceYears()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetAllComplianceYearsDataAccess dataAccess = new GetAllComplianceYearsDataAccess(database.WeeeContext);

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
                mu1.OrganisationId = organisationId;
                mu1.CreatedDate = DateTime.Now;
                mu1.CreatedById = user1.Id;

                var mu2 = modelHelper.CreateMemberUpload(scheme1);
                mu2.ComplianceYear = 2015;
                mu2.IsSubmitted = true;
                mu2.OrganisationId = organisationId;
                mu2.CreatedDate = DateTime.Now;
                mu2.CreatedById = user1.Id;

                var mu3 = modelHelper.CreateMemberUpload(scheme1);
                mu3.ComplianceYear = 2016;
                mu3.IsSubmitted = true;
                mu3.OrganisationId = organisationId;
                mu3.CreatedDate = DateTime.Now;
                mu3.CreatedById = user1.Id;

                database.Model.SaveChanges();

                var drv1 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 1, true);
                database.Model.SaveChanges();

                var dru1 = modelHelper.CreateDataReturnUpload(scheme1, drv1);
                dru1.ComplianceYear = 2015;
                dru1.Quarter = (int?)QuarterType.Q1;
                dru1.FileName = "DataReturnUpload1.xml";
                database.Model.SaveChanges();

                var drv2 = modelHelper.CreateDataReturnVersion(scheme1, 2015, 2, true);
                database.Model.SaveChanges();

                var dru2 = modelHelper.CreateDataReturnUpload(scheme1, drv2);
                dru2.ComplianceYear = 2015;
                dru2.Quarter = (int?)QuarterType.Q2;
                dru2.FileName = "DataReturnUpload2.xml";
                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.GetAllComplianceYears(ComplianceYearFor.MemberRegistrations);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Count, 2);
                Assert.Equal(result[0], 2016);
                Assert.Equal(result[1], 2015);
            }
        }
    }
}
