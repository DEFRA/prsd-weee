namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfActionAttribute : RequiredAttribute
    {
        //TODO:.... required for submission
    }
}