namespace EA.Weee.Web.Areas.PCS.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Core.Shared;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.PCS.MemberRegistration;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.ViewModels.PCS;
    using ViewModels;
    using Weee.Requests.Shared;

    public class MemberRegistrationController : Controller
    {
        private readonly Func<IWeeeClient> apiClient;
        private readonly IFileConverterService fileConverter;

        public MemberRegistrationController(Func<IWeeeClient> apiClient, IFileConverterService fileConverter)
        {
            this.apiClient = apiClient;
            this.fileConverter = fileConverter;
        }

        [HttpGet]
        public async Task<ViewResult> AddOrAmendMembers(Guid id)
        {   
            using (var client = apiClient())
            {
                var orgExists = await client.SendAsync(User.GetAccessToken(), new VerifyOrganisationExists(id));
                if (orgExists)
                {
                    return View();
                }
            }

            throw new InvalidOperationException(string.Format("'{0}' is not a valid organisation Id", id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrAmendMembers(Guid id, AddOrAmendMembersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var fileData = fileConverter.Convert(model.File);
            using (var client = apiClient())
            {
                var validationId = await client.SendAsync(User.GetAccessToken(), new ValidateXmlFile(id, fileData));

                return RedirectToAction("ViewErrorsAndWarnings", "MemberRegistration", new { area = "PCS", memberUploadId = validationId });
            }
        }

        [HttpGet]
        public async Task<ViewResult> ViewErrorsAndWarnings(Guid id, Guid memberUploadId)
        {
            using (var client = apiClient())
            {
                var errors = await client.SendAsync(User.GetAccessToken(), new GetMemberUploadData(memberUploadId));

                if (errors.Any(e => e.ErrorLevel == ErrorLevel.Error))
                {
                    return View("XmlHasErrors", new MemberUploadResultViewModel { ErrorData = errors });
                }

                return View("XmlHasNoErrors", new MemberUploadResultViewModel { ErrorData = errors });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitXml(MemberUploadResultViewModel viewModel)
        {
            throw new NotImplementedException();
            // return RedirectToAction("SuccessfulSubmission");
        }

        [HttpGet]
        public ViewResult SuccessfulSubmission()
        {
            return View();
        }
    }
}