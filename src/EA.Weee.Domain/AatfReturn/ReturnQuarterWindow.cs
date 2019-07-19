namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataReturns;
    using Organisation;
    using Prsd.Core;

    public class ReturnQuarterWindow
    {
        public virtual Return Return { get; private set; }

        public virtual QuarterWindow QuarterWindow { get; private set; }

        public List<Aatf> Aatfs { get; private set; }

        public List<NonObligatedWeee> NonObligatedWeeeList { get; private set; }

        public List<WeeeReceivedAmount> ObligatedWeeeReceivedList { get; private set; }

        public List<WeeeSentOnAmount> ObligatedWeeeSentOnList { get; private set; }

        public List<WeeeReusedAmount> ObligatedWeeeReusedList { get; private set; }

        public Organisation Organisation { get; private set; }

        public virtual List<ReturnScheme> ReturnSchemes { get; private set; }

        public List<ReturnReportOn> ReturnReportOns { get; private set; }

        public virtual DateTime SystemDateTime { get; private set; }

        public ReturnQuarterWindow(Return @return, 
            QuarterWindow quarterWindow,
            List<Aatf> aatfs, List<NonObligatedWeee> nonObligatedWeeeList,
            List<WeeeReceivedAmount> obligatedReceivedList,
            List<WeeeReusedAmount> obligatedReusedList,
            Organisation organisation,
            List<WeeeSentOnAmount> obligatedSentOnList,
            List<ReturnScheme> returnSchemes,
            List<ReturnReportOn> returnReportOns,
            DateTime systemDateTime)
        {
            Guard.ArgumentNotNull(() => @return, @return);
            Guard.ArgumentNotNull(() => quarterWindow, quarterWindow);
            Guard.ArgumentNotNull(() => returnSchemes, returnSchemes);

            this.Return = @return;
            this.QuarterWindow = quarterWindow;
            this.Aatfs = aatfs;
            this.NonObligatedWeeeList = nonObligatedWeeeList;
            this.ObligatedWeeeReceivedList = obligatedReceivedList;
            this.ObligatedWeeeReusedList = obligatedReusedList;
            this.Organisation = organisation;
            this.ObligatedWeeeSentOnList = obligatedSentOnList;
            this.ReturnSchemes = returnSchemes;
            this.ReturnReportOns = returnReportOns;
            this.SystemDateTime = systemDateTime;
        }
    }
}
