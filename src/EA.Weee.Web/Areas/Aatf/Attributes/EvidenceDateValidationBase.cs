namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Services.Caching;

    public class EvidenceDateValidationBase : ValidationAttribute
    {
        protected IWeeeCache Cache => DependencyResolver.Current.GetService<IWeeeCache>();
    }
}