namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme
{
    using EA.Weee.RequestHandlers.Scheme.GetSchemePublicInfo;
    using EA.Weee.Tests.Core.Model;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class GetSchemePublicInfoDataAccessTests
    {
        [Fact]
        public async void FetchSchemeByOrganisationId_WitValidOrganisationId_ReturnsScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetSchemePublicInfoDataAccess dataAccess = new GetSchemePublicInfoDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;

                database.Model.SaveChanges();

                // Act
                var result = await dataAccess.FetchSchemeByOrganisationId(organisationId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(organisationId, result.OrganisationId);
            }
        }

        [Fact]
        public async void FetchSchemeByOrganisationId_WithUnknownOrganisationId_ThrowsException()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetSchemePublicInfoDataAccess dataAccess = new GetSchemePublicInfoDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = new Guid("ED41564F-7486-4ED8-B563-CC3FCA9ECF39");

                database.Model.SaveChanges();

                // Act
                Func<Task<Domain.Scheme.Scheme>> action = async () => await dataAccess.FetchSchemeByOrganisationId(organisationId);

                // Assert
                await Assert.ThrowsAsync<Exception>(action);
            }
        }

        [Fact]
        public async void FetchSchemeByOrganisationId_WhenTwoSchemesHaveTheSameOrganisationId_ThrowsException()
        {
            using (var database = new DatabaseWrapper())
            {
                Guid organisationId = new Guid("72BB14DF-DCD5-4DBB-BBA9-4CFC26AD80F9");
                GetSchemePublicInfoDataAccess dataAccess = new GetSchemePublicInfoDataAccess(database.WeeeContext);

                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId;

                var scheme2 = modelHelper.CreateScheme();
                scheme2.Organisation = scheme1.Organisation;

                database.Model.SaveChanges();

                // Act
                Func<Task<Domain.Scheme.Scheme>> action = async () => await dataAccess.FetchSchemeByOrganisationId(organisationId);

                // Assert
                await Assert.ThrowsAsync<Exception>(action);
            }
        }
    }
}
