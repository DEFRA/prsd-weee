namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EA.Weee.Core;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using ViewModels.Shared;

    public class HomeController : Controller
    {
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;

        public HomeController(BreadcrumbService breadcrumb, IWeeeCache cache)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult _WeeeTitle()
        {
            bool userIsInternal = ((ClaimsIdentity)User.Identity).HasClaim(
                ClaimTypes.AuthenticationMethod, Claims.CanAccessInternalArea);
            
            TitleViewModel model = new TitleViewModel();
            model.User = User;
            model.UserIsInternal = userIsInternal;
            model.Breadcrumb = breadcrumb;

            string userIdString = User.GetUserId();
            if (userIdString != null)
            {
                Guid userId = new Guid(userIdString);

                // MVC 5 doesn't allow child actions to run asynchronously, so we
                // have to schedule this task and block the calling thread.
                // Furthermore, this task will be scheduled on a thread with different
                // synchronization context than the one used by ASP.NET so we wont
                // have access to HttpContext.Current.
                var task = Task.Run(async () =>
                {
                    return await cache.FetchUserActiveCompleteOrganisationCount(userId);
                });
                task.Wait();

                model.ShowLinkToSelectOrganisation = (task.Result > 1);
            }
            else
            {
                model.ShowLinkToSelectOrganisation = false;
            }

            return PartialView(model);
        }

        [AllowAnonymous]
        public ActionResult Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }
    }
}