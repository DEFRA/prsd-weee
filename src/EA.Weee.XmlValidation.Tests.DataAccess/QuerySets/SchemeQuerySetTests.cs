namespace EA.Weee.XmlValidation.Tests.DataAccess.BusinessValidation.Rules.QuerySets
{
    using System;
    using EA.Weee.Tests.Core.Model;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using Xunit;

    public class SchemeQuerySetTests
    {
        [Fact]
        public void GetSchemeApprovalNumberByOrganisationId_OrganisationIdDoesNotExist_ReturnsNull()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var organisationId = Guid.NewGuid();

                SchemeQuerySet schemeQuerySet = new SchemeQuerySet(database.WeeeContext);

                // Act
                string result = schemeQuerySet.GetSchemeApprovalNumberByOrganisationId(organisationId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public void GetSchemeApprovalNumberByOrganisationId_OrganisationIdDoesExist_ReturnsApprovalNumber()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Scheme scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "ABC";
                
                database.Model.SaveChanges();

                SchemeQuerySet schemeQuerySet = new SchemeQuerySet(database.WeeeContext);

                // Act
                string result = schemeQuerySet.GetSchemeApprovalNumberByOrganisationId(scheme.OrganisationId);

                // Assert
                Assert.Equal("ABC", result);
            }
        }
    }
}
