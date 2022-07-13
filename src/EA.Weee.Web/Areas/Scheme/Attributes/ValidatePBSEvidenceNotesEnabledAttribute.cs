namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using Services;

    public class ValidatePBSEvidenceNotesEnabledAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public ConfigurationService ConfigService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes)
            {
                throw new InvalidOperationException("Manage PBS evidence notes is not enabled.");
            }

            base.OnActionExecuting(context);
        }
    }
}
