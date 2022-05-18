namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using Services;

    public class ValidatePcsObligationsEnabledAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public ConfigurationService ConfigService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ConfigService.CurrentConfiguration.EnablePCSObligations)
            {
                throw new InvalidOperationException("Manage PCS obligations is not enabled.");
            }

            base.OnActionExecuting(context);
        }
    }
}