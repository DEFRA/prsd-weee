namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Web.Mvc;
    using Filters;
    using Services;
    using Services.Caching;

    public class ValidateSchemeEvidenceEnabledAttribute : ActionFilterAttribute
    {
        public ConfigurationService ConfigService { get; set; }

        public IWeeeCache Cache { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.RouteData.Values.TryGetValue("pcsId", out var idActionParameter))
            {
                throw new ArgumentException("No pcsId specified");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var pcsId)))
            {
                throw new ArgumentException("The specified pcsId is not valid");
            }

            AsyncHelpers.RunSync(async () =>
            {
                var schemeInfo = await Cache.FetchSchemePublicInfo(pcsId);

                switch (schemeInfo.IsBalancingScheme)
                {
                    case false when !ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes:
                        throw new InvalidOperationException("PCS evidence notes are not enabled.");
                    case true when !ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes:
                        throw new InvalidOperationException("PBS evidence notes are not enabled.");
                }
            });

            base.OnActionExecuting(context);
        }
    }
}