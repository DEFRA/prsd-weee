﻿namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class ReturnCanNotFindAatfViewModelMapTransfer
    {
        public ReturnCanNotFindAatfViewModelMapTransfer()
        {
        }

        public ReturnData Return { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public bool IsCanNotFindLinkClick { get; set; }

        public WeeeSentOnData WeeeSentOnData { get; set; }

        public string SearchTerm { get; set; }

        public List<string> SearchResults { get; set; }
    }
}