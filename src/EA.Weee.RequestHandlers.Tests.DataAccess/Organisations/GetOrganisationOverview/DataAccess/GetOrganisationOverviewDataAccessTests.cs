namespace EA.Weee.RequestHandlers.Tests.DataAccess.Organisations.GetOrganisationOverview.DataAccess
{
    using System.Threading.Tasks;
    using EA.Weee.Tests.Core.Model;
    using RequestHandlers.Organisations.GetOrganisationOverview.DataAccess;
    using Xunit;

    public class GetOrganisationOverviewDataAccessTests
    {
        [Fact]
        public async void HasMemberSubmissions_ForOganisationWithNoAssociatedScheme_ReturnsFalse()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var scheme1 = helper.CreateScheme();

                var upload1 = helper.CreateMemberUpload(scheme1);
                upload1.IsSubmitted = true;
                upload1.ComplianceYear = 2016;

                var upload2 = helper.CreateMemberUpload(scheme1);
                upload2.IsSubmitted = true;
                upload2.ComplianceYear = 2016;

                var organisation = helper.CreateOrganisation();

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasMemberSubmissions(organisation.Id);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async void HasMemberSubmissions_ForOganisationWithAssociatedScheme_AndSubmissions_ReturnsTrue()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var scheme = helper.CreateScheme();

                var upload1 = helper.CreateMemberUpload(scheme);
                upload1.IsSubmitted = true;
                upload1.ComplianceYear = 2016;

                var upload2 = helper.CreateMemberUpload(scheme);
                upload2.IsSubmitted = false;
                upload2.ComplianceYear = 2016;

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasMemberSubmissions(scheme.OrganisationId);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async void HasMemberSubmissions_ForOganisationWithAssociatedScheme_AndNoSubmission_ReturnsFalse()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var scheme = helper.CreateScheme();

                var upload1 = helper.CreateMemberUpload(scheme);
                upload1.IsSubmitted = false;
                upload1.ComplianceYear = 2016;

                var upload2 = helper.CreateMemberUpload(scheme);
                upload2.IsSubmitted = false;
                upload2.ComplianceYear = 2016;

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasMemberSubmissions(scheme.OrganisationId);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async void HasMultipleOrganisationUsers_ForOrganisationWithNoUser_ReturnsFalse()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var organisation = helper.CreateOrganisation();

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasMultipleManageableOrganisationUsers(organisation.Id);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async void HasMultipleManageableOrganisationUsers_ForOrganisationWithOneManageableUser_ReturnsFalse()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var organisation = helper.CreateOrganisation();
                helper.CreateOrganisationUser(organisation, "Test user");

                // Rejected user
                helper.CreateOrganisationUser(organisation, "Test user", 3);

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasMultipleManageableOrganisationUsers(organisation.Id);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async void HasMultipleManageableOrganisationUsers_ForOrganisationWithMoreThanOneManageableUser_ReturnsTrue()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var organisation = helper.CreateOrganisation();
                helper.CreateOrganisationUser(organisation, "Test user1");
                helper.CreateOrganisationUser(organisation, "Test user2");

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasMultipleManageableOrganisationUsers(organisation.Id);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async void HasMultipleManageableOrganisationUsers_ForOrganisationWithMoreThanOneUser_AllRejectedStatuses_ReturnsFalse()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var organisation = helper.CreateOrganisation();

                // Rejected users
                helper.CreateOrganisationUser(organisation, "Test user1", 3);
                helper.CreateOrganisationUser(organisation, "Test user2", 3);

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasMultipleManageableOrganisationUsers(organisation.Id);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task HasDataReturnSubmissions_ForOganisationWithNoAssociatedScheme_ReturnsFalse()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var organisation = helper.CreateOrganisation();

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasDataReturnSubmissions(organisation.Id);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task HasDataReturnSubmissions_ForOganisationWithAssociatedScheme_AndSubmittedDataReturnVersion_ReturnsTrue()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var scheme = helper.CreateScheme();
                helper.CreateDataReturnVersion(scheme, 2016, 1, true);

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasDataReturnSubmissions(scheme.OrganisationId);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task HasMemberSubmissions_ForOganisationWithAssociatedScheme_AndNoSubmittedDataReturnVersion_ReturnsFalse()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                var scheme = helper.CreateScheme();
                helper.CreateDataReturnVersion(scheme, 2016, 1, false);

                database.Model.SaveChanges();

                var dataAccess = new GetOrganisationOverviewDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.HasDataReturnSubmissions(scheme.OrganisationId);

                // Assert
                Assert.False(result);
            }
        }
    }
}
