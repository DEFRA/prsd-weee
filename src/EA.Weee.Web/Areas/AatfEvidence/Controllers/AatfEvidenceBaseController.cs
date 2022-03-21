namespace EA.Weee.Web.Areas.AatfEvidence.Controllers
{
    using EA.Weee.Web.Areas.AatfEvidence.Attributes;
    using Web.Controllers.Base;

    [ValidateEvidenceEnabledAttribute]
    public abstract class AatfEvidenceBaseController : ExternalSiteController
    {
    }
}