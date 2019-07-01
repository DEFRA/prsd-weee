namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetAatfSubmissionsTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsUkEeeDataWithSelectedComplianceYear()
        {
            using (var db = new DatabaseWrapper())
            {
                //Arrange
                var helper = new ModelHelper(db.Model);
                
                // Act
                var results = await db.StoredProcedures.GetAatfSubmissions(Guid.Parse("D890DC94-5361-445E-918F-AA5B00C0AC0E"));
            }
        }
    }
}
