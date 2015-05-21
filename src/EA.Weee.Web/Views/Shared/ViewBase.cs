namespace EA.Weee.Web.Views.Shared
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Services;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "File contains generic and non-generic of the same class.")]
    public abstract class ViewBase : WebViewPage
    {
        public ConfigurationService Config { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "File contains generic and non-generic of the same class.")]
    public abstract class ViewBase<TModel> : WebViewPage<TModel>
    {
        public ConfigurationService Config { get; set; }
    }
}