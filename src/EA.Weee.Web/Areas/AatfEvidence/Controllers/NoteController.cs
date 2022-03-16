namespace EA.Weee.Web.Areas.AatfEvidence.Controllers
{
    using System.Dynamic;
    using System.Web.Mvc;
    using ViewModels;

    public class NoteController : AatfEvidenceBaseController
    {
        public ActionResult Create()
        {
            return View(new CreateNoteViewModel());
        }
    }
}