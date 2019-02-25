namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using Core.AatfReturn;

    public class ReturnToObligatedViewModelTransfer
    {
        public ReturnData ReturnData { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid SchemeId { get; set; }

        public Guid AatfId { get; set; }

        public Guid ReturnId { get; set; }

        public ReturnToObligatedViewModelTransfer()
        {
            ReturnData = new ReturnData();
        }
    }
}