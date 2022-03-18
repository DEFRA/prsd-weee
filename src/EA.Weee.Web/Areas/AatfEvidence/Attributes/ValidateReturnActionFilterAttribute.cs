namespace EA.Weee.Web.Areas.AatfEvidence.Attributes
{
    using Api.Client;
    using Services;
    using System;
    using System.Web.Mvc;

    public class ValidateReturnActionFilterAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

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