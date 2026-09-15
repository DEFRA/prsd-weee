namespace EA.Weee.Web.Areas.Admin.Controllers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords;

    internal interface IGetAllYears
    {
        List<string> GetAllYears(IWeeeClient client);
    }

    internal interface IGetSchemesForYear
    {
        List<SchemeData> GetSchemesForYear(IWeeeClient client, int? selectedYear);
    }

    internal interface IGetSortedResults
    {
        List<SchemeDataExceedingRetentionPeriod> GetSortedResults(IWeeeClient client, int? selectedYear, string selectedName);
    }

    internal interface IPopulateRemovePCSRecordsFilterViewModel
    {
        RemovePCSRecordsFilterViewModel PopulateRemovePCSRecordsFilterViewModel(RemovePCSRecordsFilterViewModel model,
            List<SchemeDataExceedingRetentionPeriod> data,
            List<SchemeData> schemes,
            List<string> allYearsStrings,
            int page,
            string selectedYear,
            string selectedScheme);
    }
}
