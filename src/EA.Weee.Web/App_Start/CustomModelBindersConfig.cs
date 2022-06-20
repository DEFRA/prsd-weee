namespace EA.Weee.Web.App_Start
{
    using System;
    using Autofac;
    using Autofac.Integration.Mvc;
    using FluentValidation.Mvc;
    using IdentityModel;
    using Infrastructure;
    using Owin;
    using Services;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using EA.Weee.Web.App_Start;

    public static class CustomModelBindersConfig
    {
        public static void RegisterCustomModelBinders()
        {
            ModelBinders.Binders.Add(typeof(DateTime?), new UKDateTimeModelBinder());
        }
    }
}
