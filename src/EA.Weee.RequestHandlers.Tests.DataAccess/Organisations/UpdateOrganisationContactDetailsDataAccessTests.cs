namespace EA.Weee.RequestHandlers.Tests.DataAccess.Organisations
{
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class UpdateOrganisationContactDetailsDataAccessTests
    {
        /// <summary>
        /// This test ensures that the data access will return the domain object representing
        /// the specified organisation.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchOrganisationAsync_WithValidOrganisationId_ReturnsOrganisation()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arranage
                Guid organisationId = new Guid("C826DCE8-78EB-4BE4-B419-4DE73D1AD181");
                Organisation organisation = new Organisation()
                {
                    Id = organisationId
                };
                database.Model.Organisations.Add(organisation);

                database.Model.SaveChanges();

                UpdateOrganisationContactDetailsDataAccess dataAccess = new UpdateOrganisationContactDetailsDataAccess(database.WeeeContext);

                // Act
                Domain.Organisation.Organisation result = await dataAccess.FetchOrganisationAsync(organisationId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(organisationId, result.Id);
            }
        }

        /// <summary>
        /// This test ensures that an exception is thrown if the data access is unable to find
        /// the specified organisation.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchOrganisationAsync_WithInvalidOrganisationId_ThrowsException()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arranage
                Guid organisationId = new Guid("5EC74E27-8D5C-4D4D-8A49-20BBD5E9611E");
                UpdateOrganisationContactDetailsDataAccess dataAccess = new UpdateOrganisationContactDetailsDataAccess(database.WeeeContext);

                // Act
                Func<Task<Domain.Organisation.Organisation>> action = async () => await dataAccess.FetchOrganisationAsync(organisationId);

                // Assert
                await Assert.ThrowsAsync<Exception>(action);
            }
        }

        /// <summary>
        /// This test ensures that the data access will return the domain object representing
        /// the specified country.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchCountryAsync_WithValidCountryId_ReturnsCountry()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                Guid countryId = new Guid("184E1785-26B4-4AE4-80D3-AE319B103ACB"); // ID for UK - England

                UpdateOrganisationContactDetailsDataAccess dataAccess = new UpdateOrganisationContactDetailsDataAccess(database.WeeeContext);

                // Act
                Domain.Country result = await dataAccess.FetchCountryAsync(countryId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(countryId, result.Id);
                Assert.Equal(result.Name, "UK - England");
            }
        }

        /// <summary>
        /// This test ensures that an exception is thrown if the data access is unable to find
        /// the specified country.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchCountryAsync_WithInvalidCountryId_ThrowsException()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arranage
                Guid countryId = new Guid("5840BF0B-0CAF-4AF9-9881-F22DB7720F98");
                UpdateOrganisationContactDetailsDataAccess dataAccess = new UpdateOrganisationContactDetailsDataAccess(database.WeeeContext);

                // Act
                Func<Task<Domain.Country>> action = async () => await dataAccess.FetchCountryAsync(countryId);

                // Assert
                await Assert.ThrowsAsync<Exception>(action);
            }
        }

        /// <summary>
        /// This test ensures that the "SaveAsync" method always calls the underlying "SaveChangesAsync"
        /// method on the WeeeContext.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SaveAsync_Always_CallsSaveOnContext()
        {
            // Arrange
            WeeeContext context = A.Fake<WeeeContext>();

            UpdateOrganisationContactDetailsDataAccess dataAccess = new UpdateOrganisationContactDetailsDataAccess(context);

            // Act
            await dataAccess.SaveAsync();

            // Assert
            A.CallTo(() => context.SaveChangesAsync())
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
