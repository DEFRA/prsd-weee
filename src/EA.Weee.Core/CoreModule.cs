﻿namespace EA.Weee.Core
{
    using Autofac;
    using Configuration.EmailRules;

    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Email Rules
            builder.RegisterType<RuleSectionChecker>().As<IRuleSectionChecker>();
            builder.RegisterType<RuleChecker>().As<IRuleChecker>();

            // Register the helper classes
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .Where(t => t.Namespace.Contains("Helpers"))
                .AsImplementedInterfaces();
        }
    }
}
