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

        public List<NonObligatedWeee> NonObligatedWeeeList { get; private set; }

        public List<WeeeReceivedAmount> ObligatedWeeeList { get; private set; }

        public Operator ReturnOperator { get; private set; }

        public ReturnQuarterWindow(Return @return, QuarterWindow quarterWindow, List<NonObligatedWeee> nonObligatedWeeeList, List<WeeeReceivedAmount> obligatedWeeeList, Operator returnOperator)
        {
            Guard.ArgumentNotNull(() => @return, @return);
            Guard.ArgumentNotNull(() => quarterWindow, quarterWindow);

            this.Return = @return;
            this.QuarterWindow = quarterWindow;
            this.NonObligatedWeeeList = nonObligatedWeeeList;
            this.ObligatedWeeeList = obligatedWeeeList;
            this.ReturnOperator = returnOperator;
        }

        public ReturnQuarterWindow(Return @return, QuarterWindow quarterWindow, List<NonObligatedWeee> nonObligatedWeeeList)
        {
            Guard.ArgumentNotNull(() => @return, @return);
            Guard.ArgumentNotNull(() => quarterWindow, quarterWindow);

            this.Return = @return;
            this.QuarterWindow = quarterWindow;
            this.NonObligatedWeeeList = nonObligatedWeeeList;
        }

        public ReturnQuarterWindow(Return @return, QuarterWindow quarterWindow, List<NonObligatedWeee> nonObligatedWeeeList, List<WeeeReceivedAmount> obligatedWeeeList)
        {
            Guard.ArgumentNotNull(() => @return, @return);
            Guard.ArgumentNotNull(() => quarterWindow, quarterWindow);

            this.Return = @return;
            this.QuarterWindow = quarterWindow;
            this.NonObligatedWeeeList = nonObligatedWeeeList;
            this.ObligatedWeeeList = obligatedWeeeList;
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
