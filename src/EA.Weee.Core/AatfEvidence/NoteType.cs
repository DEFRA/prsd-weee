namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core.Domain;

    [Serializable]
    public enum NoteType
    {
        [Display(Name = "E")]
        Evidence = 1,
        [Display(Name = "T")]
        Transfer = 2
    }
}
