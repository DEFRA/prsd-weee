namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum NoteStatus
    {
        [Display(Name = "Draft")]
        Draft = 1,
        [Display(Name = "Submitted")]
        Submitted = 2
    }
}
