namespace EA.Weee.Web.Areas.AatfEvidence.Controllers
{
    using System.Web.Mvc;
    using Aatf.Attributes;
    using Web.Controllers.Base;

    [ValidateAatfEvidenceEnabled]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public abstract class AatfEvidenceBaseController : ExternalSiteController
    {
    }
}