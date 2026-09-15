namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers.Stubs
{
    using System.Collections.Generic;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Web.Areas.Admin.Controllers.Interfaces;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords;

    public partial class PopulateRemovePCSRecordsFilterViewModelStub : IPopulateRemovePCSRecordsFilterViewModel
    {
        private RemovePCSRecordsFilterViewModel returnValue;

        public PopulateRemovePCSRecordsFilterViewModelStub(RemovePCSRecordsFilterViewModel returnValue)
        {
            this.returnValue = returnValue;
        }

        public RemovePCSRecordsFilterViewModel PopulateRemovePCSRecordsFilterViewModel(RemovePCSRecordsFilterViewModel model,
            List<SchemeDataExceedingRetentionPeriod> data,
            List<SchemeData> schemes,
            List<string> allYearsStrings,
            int page,
            string selectedYear,
            string selectedScheme)
        {
            return returnValue;
        }
    }
}
