namespace EA.Weee.Web.ViewModels.Shared
{
    using System.Security.Principal;
    using EA.Weee.Web.Services;

    public class TitleViewModel
    {
        public IPrincipal User { get; set; }
        
        /// <summary>
        /// This property is used to determine whether the "sign out" button will
        /// use the internal or external controller's action.
        /// </summary>
        public bool UserIsInternal { get; set; }

        public BreadcrumbService Breadcrumb { get; set; }
        public bool ShowLinkToSelectOrganisation { get; set; }
    }
}