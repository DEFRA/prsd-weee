namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{    
    using System;

    public class ReturnAndAatfToSentOnCreateWeeeSentOnViewModelMapTransfer
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid SelectedAatfId { get; set; }

        public Guid? WeeeSentOnId { get; set; }
    }
}