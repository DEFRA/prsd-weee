﻿namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataReturns;
    using Prsd.Core;

    public class ReturnQuarterWindow
    {
        public Return Return { get; private set; }

        public QuarterWindow QuarterWindow { get; private set; }

        public List<Aatf> Aatfs { get; private set; }

        public List<NonObligatedWeee> NonObligatedWeeeList { get; private set; }

        public List<WeeeReceivedAmount> ObligatedWeeeReceivedList { get; private set; }

        public List<WeeeSentOnAmount> ObligatedWeeeSentOnList { get; private set; }

        public List<WeeeReusedAmount> ObligatedWeeeReusedList { get; private set; }

        public Operator ReturnOperator { get; private set; }

        public List<ReturnScheme> ReturnSchemes { get; set; }

        public ReturnQuarterWindow(Return @return, QuarterWindow quarterWindow,
            List<Aatf> aatfs, List<NonObligatedWeee> nonObligatedWeeeList,
            List<WeeeReceivedAmount> obligatedReceivedList,
            List<WeeeReusedAmount> obligatedReusedList,
            Operator returnOperator,
            List<WeeeSentOnAmount> obligatedSentOnList,
            List<ReturnScheme> returnSchemes)
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
            this.ReturnOperator = returnOperator;
            this.ObligatedWeeeSentOnList = obligatedSentOnList;
            this.ReturnSchemes = returnSchemes;
        }

        public ReturnQuarterWindow(Return @return, QuarterWindow quarterWindow, List<NonObligatedWeee> nonObligatedWeeeList)
        {
            Guard.ArgumentNotNull(() => @return, @return);
            Guard.ArgumentNotNull(() => quarterWindow, quarterWindow);

            this.Return = @return;
            this.QuarterWindow = quarterWindow;
            this.NonObligatedWeeeList = nonObligatedWeeeList;
        }

        public ReturnQuarterWindow(Return @return, QuarterWindow quarterWindow)
        {
            Guard.ArgumentNotNull(() => @return, @return);
            Guard.ArgumentNotNull(() => quarterWindow, quarterWindow);

            this.Return = @return;
            this.QuarterWindow = quarterWindow;
        }
    }
}
