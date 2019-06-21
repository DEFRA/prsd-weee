namespace EA.Weee.Web.ViewModels.Returns
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Scheme;

    public class ReturnViewModel : ReturnViewModelBase
    {
        public ReturnViewModel()
        {
        }

        public ReturnViewModel(ReturnData returnData) : base(returnData)
        {
        }

        public ReturnViewModel(ReturnData returnData, List<AatfObligatedData> obligatedTonnage, OrganisationData organisationData, TaskListDisplayOptions displayOptions) : base(returnData)
        {
            this.Year = returnData.Quarter.Year.ToString();
            this.AatfsData = obligatedTonnage;
            this.Organisation = organisationData;
            this.ReportOnDisplayOptions = displayOptions;
        }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        [Display(Name = "Reporting period")]
        public override string Period => $"{Quarter} {QuarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)}";

        [Display(Name = "Compliance year")]
        public override string Year { get; }

        public string NonObligatedTonnageTotal { get; set; }

        public string NonObligatedTonnageTotalDcf { get; set; }

        public string NonObligatedTotal { get; set; }

        public string ObligatedTotal { get; set; }

        public List<AatfObligatedData> AatfsData { get; set; }

        public OrganisationData Organisation { get; set; }

        public TaskListDisplayOptions ReportOnDisplayOptions { get; set; }

        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public IList<SchemeData> SchemeDataItems { get; set; }

        public ReturnData ReturnData { get; set; }

        public bool AnyAatfSchemes
        {
            get
            {
                if (AatfsData == null || !AatfsData.Any())
                {
                    return false;
                }

                if (AatfsData.Any(a => a.SchemeData.Any()))
                {
                    return true;
                }

                return false;
            }
        }
    }
}