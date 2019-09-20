namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;

    public class ReturnAndSchemeDataToReceivedPcsViewModelMapTransfer
    {
        public string AatfName { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public IList<SchemeData> SchemeDataItems { get; set; }

        public ReturnData ReturnData { get; set; }
    }
}