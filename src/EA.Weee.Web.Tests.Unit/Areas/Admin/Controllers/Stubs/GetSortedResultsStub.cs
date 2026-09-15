namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers.Stubs
{
    using System.Collections.Generic;
    using EA.Weee.Api.Client;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Web.Areas.Admin.Controllers.Interfaces;

    public partial class GetSortedResultsStub : IGetSortedResults
    {
        private List<SchemeDataExceedingRetentionPeriod> returnValue;

        public GetSortedResultsStub(List<SchemeDataExceedingRetentionPeriod> returnValue)
        {
            this.returnValue = returnValue;
        }

        public List<SchemeDataExceedingRetentionPeriod> GetSortedResults(IWeeeClient client, int? selectedYear, string selectedName)
        {
            return returnValue;
        }
    }
}
