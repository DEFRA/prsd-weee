namespace EA.Weee.Web.Validators
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property)]
    public class HttpPostedFileTypeAttribute : ValidationAttribute
    {
        private readonly string[] allowedContentTypes;

        public HttpPostedFileTypeAttribute(params string[] allowedContentTypes)
        {
            this.allowedContentTypes = allowedContentTypes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var httpPostedFile = value as HttpPostedFileBase;
            if (httpPostedFile != null && !IsAllowedContentType(httpPostedFile.ContentType))
            {
                return new ValidationResult(string.Format("The file '{0}' is not a valid file", httpPostedFile.FileName));
            }

            return base.IsValid(value, validationContext);
        }

        public override bool IsValid(object value)
        {
            var httpPostedFile = value as HttpPostedFileBase;
            if (httpPostedFile != null && !IsAllowedContentType(httpPostedFile.ContentType))
            {
                return false;
            }

            return true;
        }

        private bool IsAllowedContentType(string fileType)
        {
            if (!string.IsNullOrEmpty(fileType) && allowedContentTypes.Select(ct => ct.ToLower()).Contains(fileType.ToLower()))
            {
                return true;
            }

            return false;
        }
    }
}