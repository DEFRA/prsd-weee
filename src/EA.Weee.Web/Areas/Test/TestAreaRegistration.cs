namespace EA.Weee.Web.Areas.Test
{
    using Controllers;
    using Infrastructure;
    using Services;
    using System.Web.Mvc;
    
    /// <summary>
    /// Registration for the "Test" area.
    /// 
    /// Routes to this area will only be registered if the "EnableTestArea"
    /// configuration key in the web.config has been defined and set to "True".
    /// 
    /// This area should not be enabled on the production environment.
    /// </summary>
    public class TestAreaRegistration : AreaRegistration
    {
        private bool testAreaEnabled;

        public TestAreaRegistration()
        {
            ConfigurationService config = DependencyResolver.Current.GetService<ConfigurationService>();
            testAreaEnabled = config.CurrentConfiguration.EnableTestArea;
        }

        public override string AreaName
        {
            get { return "Test"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (testAreaEnabled)
            {
                context.MapLowercaseDashedRoute(
                    name: "Test_default",
                    url: "Test/{controller}/{action}/{entityId}",
                    defaults: new { action = "Index", controller = "Home", entityId = UrlParameter.Optional },
                    namespaces: new[] { typeof(HomeController).Namespace });
            }
        }
    }
}