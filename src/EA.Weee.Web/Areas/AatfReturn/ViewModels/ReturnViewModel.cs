namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Scheme;

    public class ReturnViewModel : ReturnViewModelBase
    {
        public ReturnViewModel()
        {
        }

        public ReturnViewModel(Quarter quarter, QuarterWindow window, int year, string nonObligatedTonnageTotal, string nonObligatedTonnageTotalDcf, List<AatfObligatedData> obligatedTonnage, OperatorData returnOperator, Guid returnId) : base(quarter, window, year)
        {
            this.Year = year.ToString();
            this.NonObligatedTonnageTotal = nonObligatedTonnageTotal;
            this.NonObligatedTonnageTotalDcf = nonObligatedTonnageTotalDcf;
            this.AatfsData = obligatedTonnage;
            this.ReturnOperator = returnOperator;
            this.ReturnId = returnId;
        }

        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public Guid ReturnId { get; set; }

        [Display(Name = "Reporting period")]
        public override string Period => $"{Quarter} {QuarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)}";

        [Display(Name = "Compliance year")]
        public override string Year { get; }
        
        public string NonObligatedTonnageTotal { get; set; }

        public string NonObligatedTonnageTotalDcf { get; set; }
        
        public List<AatfObligatedData> AatfsData { get; set; }

        public OperatorData ReturnOperator { get; set; }

        public TaskListDisplayOptions ReportOnDisplayOptions { get; set; }

        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public IList<SchemeData> SchemeDataItems { get; set; }

        public ReturnData ReturnData { get; set; }
    }
}