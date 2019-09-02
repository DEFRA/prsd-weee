namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.UpdateSchemeInformation
{
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Scheme.UpdateSchemeInformation;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    using Organisation = Domain.Organisation.Organisation;
    using Scheme = Domain.Scheme.Scheme;

    public class UpdateSchemeInformationDataAccessTests
    {
        [Fact]
        public async Task Add_EntityShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                int originalSchemesCount = database.WeeeContext.Schemes.Count();
                UpdateSchemeInformationDataAccess dataAccess = new UpdateSchemeInformationDataAccess(database.WeeeContext);

                Organisation organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                Scheme scheme = ObligatedWeeeIntegrationCommon.CreateScheme(organisation);

                dataAccess.AddScheme(scheme);
                await dataAccess.SaveAsync();

                database.WeeeContext.Schemes.Where(p => Equals(scheme)).Should().NotBeNull();

                int newSchemesCount = database.WeeeContext.Schemes.Count();

                (newSchemesCount - originalSchemesCount).Should().Be(1);
            }
        }
    }
}
