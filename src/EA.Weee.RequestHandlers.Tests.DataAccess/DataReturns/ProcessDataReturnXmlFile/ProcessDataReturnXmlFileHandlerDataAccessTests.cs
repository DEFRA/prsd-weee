namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class ProcessDataReturnXmlFileHandlerDataAccessTests
    {
        [Fact]
        public async Task FetchSchemeByOrganisationIdAsync_GetsSchemeForMatchingOrganisationId()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var organisation1 = helper.CreateOrganisation();
                helper.CreateScheme(organisation1);

                var organisation2 = helper.CreateOrganisation();
                var scheme2 = helper.CreateScheme(organisation2);

                database.Model.SaveChanges();

                var dataAccess = new ProcessDataReturnXmlFileDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.FetchSchemeByOrganisationIdAsync(organisation2.Id);

                // Assert
                Assert.Equal(scheme2.Id, result.Id);
            }
        }

        [Fact]
        public async Task FetchSchemeByOrganisationIdAsync_ThrowsInvalidOperationException_WhenNoMatchingOrganisationId()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var dataAccess = new ProcessDataReturnXmlFileDataAccess(database.WeeeContext);

                await Assert.ThrowsAsync<InvalidOperationException>(() => dataAccess.FetchSchemeByOrganisationIdAsync(Guid.NewGuid()));
            }
        }

        [Fact]
        public async Task AddAndSaveAsync_AddsDataReturnUpload_ToWeeeContextDataReturnsUploads()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var scheme = helper.CreateScheme();
                helper.GetOrCreateRegisteredProducer(scheme, 2016, "AAAA");

                database.Model.SaveChanges();

                var upload = new Domain.DataReturns.DataReturnUpload(domainHelper.GetScheme(scheme.Id), "Upload Data", new List<Domain.DataReturns.DataReturnUploadError>(), "File name", 2016, 1);

                var dataAccess = new ProcessDataReturnXmlFileDataAccess(database.WeeeContext);

                // Act
                await dataAccess.AddAndSaveAsync(upload);

                // Assert
                Assert.Single(database.WeeeContext.DataReturnsUploads, u => u.Id == upload.Id);
            }
        }
    }
}
