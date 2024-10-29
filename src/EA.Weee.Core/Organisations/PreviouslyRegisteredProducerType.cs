namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel.DataAnnotations;

    public enum PreviouslyRegisteredProducerType
    {
        [Display(Name = "Yes, I have previously been a member of a producer compliance scheme")]
        YesPreviousSchemeMember = 1,

        [Display(Name = "Yes, I have previously been registered directly as a small producer")]
        YesPreviousSmallProducer = 2,

        [Display(Name = "No")]
        No = 3,
    }
}