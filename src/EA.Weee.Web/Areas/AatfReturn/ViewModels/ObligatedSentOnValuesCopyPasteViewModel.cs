﻿namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;

    public class ObligatedSentOnValuesCopyPasteViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public String SiteName { get; set; }

        public Guid WeeeSentOnId { get; set; }

        public Guid ReturnId { get; set; }

        public String[] B2bPastedValues { get; set; }

        public String[] B2cPastedValues { get; set; }

        public bool? IsEditDetails { get; set; }

        public bool? IsEditTonnage { get; set; }
    }
}