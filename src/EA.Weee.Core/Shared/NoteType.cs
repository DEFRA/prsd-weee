namespace EA.Weee.Core.Shared
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum NoteType
    {
        [Display(Name = "E")]
        Evidence = 1,
        [Display(Name = "T")]
        Transfer = 2
    }
}
