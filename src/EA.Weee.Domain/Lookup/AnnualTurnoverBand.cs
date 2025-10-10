namespace EA.Weee.Domain.Lookup
{
    using System.ComponentModel.DataAnnotations;

    public enum AnnualTurnoverBand
    {
        [Display(Name = "Less than or equal to one million pounds")]
        Lessthanorequaltoonemillionpounds = 0,

        [Display(Name = "Greater than one million pounds")]
        Greaterthanonemillionpounds = 1,

        [Display(Name = "Not applicable")]
        NotApplicable = 2
    }
}