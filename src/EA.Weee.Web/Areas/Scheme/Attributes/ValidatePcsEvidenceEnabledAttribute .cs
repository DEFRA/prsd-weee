namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using Services;

    public class ValidatePcsEvidenceEnabledAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public ConfigurationService ConfigService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes)
            {
                throw new InvalidOperationException("PCS evidence notes are not enabled.");
            }

            base.OnActionExecuting(context);
        }
    }
}