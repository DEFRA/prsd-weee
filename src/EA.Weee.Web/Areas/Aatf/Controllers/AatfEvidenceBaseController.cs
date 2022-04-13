namespace EA.Weee.Web.Areas.AatfEvidence.Controllers
{
    using System.Web.Mvc;
    using Aatf.Attributes;
    using Web.Controllers.Base;

    [ValidateAatfEvidenceEnabled]

    public abstract class AatfEvidenceBaseController : ExternalSiteController
    {
    }
}