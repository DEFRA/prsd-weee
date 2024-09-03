namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel.DataAnnotations;

    public enum TonnageType
    {
        [Display(Name = "5 tonnes or more")]
        FiveTonnesOrMore = 1,

        [Display(Name = "Less than 5 tonnes")]
        LessThanFiveTonnes = 2,
    }
}