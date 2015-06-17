namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;

    public class JoinOrganisationViewModel
    {
        public Guid OrganisationToJoin { get; set; }

        public JoinOrganisationViewModel(Guid selected)
        {
            OrganisationToJoin = selected;
        }
    }
}