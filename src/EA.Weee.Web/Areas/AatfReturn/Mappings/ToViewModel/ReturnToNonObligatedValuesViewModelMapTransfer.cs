namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class ReturnToNonObligatedValuesViewModelMapTransfer
    {
        public ReturnData ReturnData { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public bool Dcf { get; set; }

        public string PastedData { get; set; }

        public ReturnToNonObligatedValuesViewModelMapTransfer()
        {
            ReturnData = new ReturnData();
        }
    }
}