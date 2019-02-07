namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;

    public class ReturnViewModel : ReturnViewModelBase
    {
        public ReturnViewModel()
        {
        }

        public ReturnViewModel(Quarter quarter, QuarterWindow window, int year, string nonObligatedTonnageTotal, string nonObligatedTonnageTotalDcf) : base(quarter, window, year)
        {
            this.Year = year.ToString();
            this.NonObligatedTonnageTotal = nonObligatedTonnageTotal;
            this.NonObligatedTonnageTotalDcf = nonObligatedTonnageTotalDcf;
        }
        
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public Guid ReturnId { get; set; }

        [Display(Name = "Reporting period")]
        public override String Period => $"{Quarter} {QuarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)}";

        [Display(Name = "Compliance year")]
        public override string Year { get; }

        public List<string> Aatfs { get; set; }

        public string NonObligatedTonnageTotal { get; set; }

        public string NonObligatedTonnageTotalDcf { get; set; }
    }
}