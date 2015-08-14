namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System.ComponentModel.DataAnnotations;

    public enum CoreEnumeration 
    {
        [Display(Name = "Something")]
        Something = 1,
        [Display(Name = "Something Else")]
        SomethingElse = 2
    }
}
