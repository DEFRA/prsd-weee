﻿namespace EA.Weee.Web.Modules
{
    using System.Configuration;
    using Api.Client;
    using Autofac;
    using Prsd.Core.Web.OAuth;
    using Prsd.Core.Web.OpenId;
    using Services;

    public class ApiClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                return new WeeeClient(config.ApiUrl);
            }).As<IWeeeClient>();

            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                return new OAuthClient(config.ApiUrl, config.ApiClientId, config.ApiSecret);
            }).As<IOAuthClient>();

            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var config = cc.Resolve<IAppConfiguration>();
                return new UserInfoClient(config.ApiUrl);
            }).As<IUserInfoClient>();
        }
    }
}