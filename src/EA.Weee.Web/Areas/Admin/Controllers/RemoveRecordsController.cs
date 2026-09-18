namespace EA.Weee.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords;

    public class RemoveRecordsController : AdminController
    {
        public RemoveRecordsController()
        {
        }

        [HttpGet]
        public ActionResult ChooseActivity()
        {
            RemoveRecordsViewModel viewModel = new RemoveRecordsViewModel();
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

            switch (viewModel.SelectedValue)
            {
                case InternalRemoveRecordsActivity.RemovePCS:
                    return RedirectToAction("Index", "RemovePCSRecords");

                default:
                    throw new NotSupportedException();
            }
        }

        private void PopulateViewModelPossibleValues(RemoveRecordsViewModel viewModel)
        {
            viewModel.PossibleValues = new List<string>() { InternalRemoveRecordsActivity.RemovePCS };
        }
    }
}