namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.Web.Mvc;
    using Services;

    public class ValidateAatfEvidenceEnabledAttribute : ActionFilterAttribute
    {
        public ConfigurationService ConfigService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes)
            {
                throw new InvalidOperationException("AATF evidence notes are not enabled.");
            }

            base.OnActionExecuting(context);
        }
    }
}