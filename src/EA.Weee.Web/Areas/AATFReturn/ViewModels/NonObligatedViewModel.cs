namespace EA.Weee.Web.Areas.AATF_Return.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core;

    public class IndexViewModel
    {
        public Guid NonObligatedId { get; private set; }

        public Guid ReturnId { get; private set; }

        public int CategoryId { get; private set; }

        public bool Dcf { get; private set; }

        public decimal Tonnage { get; private set; }
    }
}