namespace EA.Weee.Web.Areas.Scheme.Controllers
{
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Controllers.Base;

    [ValidatePcsEvidenceEnabled]
    public abstract class SchemeEvidenceBaseController : ExternalSiteController
    {
    }
}