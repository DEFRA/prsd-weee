namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using Core.Shared;

    public class AuthorizationRequiredViewModel
    {
        public SchemeStatus Status { get; set; }

        public bool ShowLinkToSelectOrganisation { get; set; }
    }
}