namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;

    public class CreateWeeeSentOnViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid SelectedWeeeSentOnId { get; set; }

        public Guid? WeeeSenOnId { get; set; }
    }
}