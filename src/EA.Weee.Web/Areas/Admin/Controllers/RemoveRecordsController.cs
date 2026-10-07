namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords;

    public class RemoveRecordsController : AdminController
    {
        private static readonly IList<string> PossibleActivities = new List<string>
        {
            InternalRemoveRecordsActivity.RemovePCS,
            InternalRemoveRecordsActivity.RemoveAATF,
            InternalRemoveRecordsActivity.RemoveAE
        };

        [HttpGet]
        public ActionResult ChooseActivity()
        {
            var viewModel = new RemoveRecordsViewModel();
            PopulateViewModelPossibleValues(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseActivity(RemoveRecordsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewModelPossibleValues(viewModel);

                return View(viewModel);
            }
            return RedirectToActivity(viewModel.SelectedValue);
        }

        private ActionResult RedirectToActivity(string selectedValue)
        {
            switch (selectedValue)
            {
                case InternalRemoveRecordsActivity.RemovePCS:
                    return RedirectToAction("Index", "RemovePCSRecords");

                case InternalRemoveRecordsActivity.RemoveAATF:
                    return RedirectToAction("Index", "RemoveAATFRecords");

                case InternalRemoveRecordsActivity.RemoveAE:
                    return RedirectToAction("Index", "RemoveAERecords");

                default: throw new NotSupportedException($"Unsupported remove-records activity: {selectedValue}");
            }
        }

        private static void PopulateViewModelPossibleValues(RemoveRecordsViewModel viewModel)
        {
            viewModel.PossibleValues = PossibleActivities;
        }
    }
}