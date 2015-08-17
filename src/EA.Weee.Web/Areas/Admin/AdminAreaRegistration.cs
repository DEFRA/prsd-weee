﻿namespace EA.Weee.Web.Areas.Admin
{
    using System.Web.Mvc;
    using Controllers;
    using Infrastructure;

    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapLowercaseDashedRoute(
                name: "admin_default",
                url: "admin/{controller}/{action}/{id}",
                defaults: new { action = "Index", controller = "Home", id = UrlParameter.Optional },
                namespaces: new[] { typeof(HomeController).Namespace });
        }
    }
}