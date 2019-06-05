namespace EA.Weee.Web.Areas.Admin.Controllers.Base
{
    using System.Web.Mvc;
    using Filters;
    using Security;

    [AuthorizeClaims(Claims.CanAccessInternalArea)]
    public abstract class AdminController : Controller
    {
        /// <summary>
        /// This is here because we don't display the site address name, but the view model for it makes it required. There is also
        /// custom validation for the AATF/AE name property which copies the value into the SiteAddressName field, so if the name
        /// property is empty, it will display an error for siteAddressName. This removed that error.
        /// </summary>
        protected void PreventSiteAddressNameValidationErrors()
        {
            var key = "SiteAddressData.Name";
            if (ModelState.ContainsKey(key))
            {
                ModelState[key].Errors.Clear();
            }
        }
    }
}