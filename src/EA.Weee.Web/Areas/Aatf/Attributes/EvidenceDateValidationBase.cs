namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Services.Caching;

    public class EvidenceDateValidationBase : ValidationAttribute
    {
        private IWeeeCache cache;
        public IWeeeCache Cache
        {
            get => cache ?? DependencyResolver.Current.GetService<IWeeeCache>();
            set => cache = value;
        }
    }
}