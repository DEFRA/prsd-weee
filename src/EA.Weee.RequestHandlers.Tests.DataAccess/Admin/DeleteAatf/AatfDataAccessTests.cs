namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.DeleteAatf
{
    using EA.Weee.RequestHandlers.Admin.DeleteAatf;
    using EA.Weee.Tests.Core.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AatfDataAccessTests
    {
        [Fact]
        public async void DoesAatfHaveData_HasReusedAndSentOnAndReceived_ReturnsTrue()
        {
            using (var dbWrapper = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(dbWrapper.Model);

                var organisation = modelHelper.CreateOrganisation();
                var scheme = modelHelper.CreateScheme(organisation);
                var @return = modelHelper.CreateReturn();
                var aatf = modelHelper.CreateAatf();
                dbWrapper.Model.SaveChanges();

                Domain.AatfReturn.WeeeReceived received = new Domain.AatfReturn.WeeeReceived(scheme.Id, aatf.Id, @return.Id);

                dbWrapper.WeeeContext.WeeeReceived.Add(received);
                dbWrapper.WeeeContext.SaveChanges();

                var dataAccess = new AatfDataAccess(dbWrapper.WeeeContext);

                var result = (await dataAccess.DoesAatfHaveData(aatf.Id));

                Assert.True(result);
            }
        }
    }
}
