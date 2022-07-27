namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Services.Caching;

    public abstract class CheckNoteAttributeBase : ActionFilterAttribute
    {
        public IWeeeCache Cache { get; set; }

        public Func<IWeeeClient> Client { get; set; }

        public abstract Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid pcsId);

        protected Guid TryGetPcsId(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionParameters.TryGetValue("pcsId", out var idActionParameter))
            {
                throw new ArgumentException("No pcs ID was specified.");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var pcsIdActionParameter)))
            {
                throw new ArgumentException("The specified pcs ID is not valid.");
            }

            return pcsIdActionParameter;
        }

        protected Guid TryGetEvidenceNoteId(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionParameters.TryGetValue("evidenceNoteId", out var idActionParameter))
            {
                throw new ArgumentException("No evidence note id specified");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var evidenceNoteIdActionParameter)))
            {
                throw new ArgumentException("The specified evidence note id is not valid.");
            }

            return evidenceNoteIdActionParameter;
        }

        protected int TryGetComplianceYear(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionParameters.TryGetValue("complianceYear", out var idActionParameter))
            {
                throw new ArgumentException("No compliance year was specified.");
            }

            if (!(int.TryParse(idActionParameter.ToString(), out var complianceYearIdActionParameter)))
            {
                throw new ArgumentException("The specified compliance year is not valid.");
            }

            return complianceYearIdActionParameter;
        }
    }
}