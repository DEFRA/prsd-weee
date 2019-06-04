namespace EA.Weee.Web.ViewModels.Returns
{
    using System.Globalization;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Prsd.Core;

    public abstract class ReturnViewModelBase : IReturnViewModel
    {
        protected readonly QuarterWindow QuarterWindow;
        private readonly Quarter quarter;
        private readonly int year;

        public virtual string Year => year.ToString();

        public virtual string Quarter => quarter.Q.ToString();

        public virtual string Period => $"{Quarter} {QuarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)} {Year}";

        public string CreatedBy { get; private set; }

        public string CreatedDate { get; private set; }

        public string SubmittedBy { get; private set; }

        public string SubmittedDate { get; private set; }

        public ReturnStatus ReturnStatus { get; private set; }

        protected ReturnViewModelBase(ReturnData returnData)
        {
            Guard.ArgumentNotNull(() => returnData, returnData);
            Guard.ArgumentNotNull(() => returnData.Quarter, returnData.Quarter);
            Guard.ArgumentNotNull(() => returnData.QuarterWindow, returnData.QuarterWindow);

            this.QuarterWindow = returnData.QuarterWindow;
            this.quarter = returnData.Quarter;
            this.year = returnData.Quarter.Year;
            this.CreatedBy = returnData.CreatedBy;
            this.CreatedDate = returnData.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
            this.SubmittedBy = string.IsNullOrWhiteSpace(returnData.SubmittedBy) ? "-" : returnData.SubmittedBy;
            this.SubmittedDate = returnData.SubmittedDate.HasValue ? returnData.SubmittedDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "-";
            this.ReturnStatus = returnData.ReturnStatus;
        }

        protected ReturnViewModelBase()
        {
        }
    }
}