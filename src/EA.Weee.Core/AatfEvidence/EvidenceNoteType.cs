namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum EvidenceNoteType
    {
        [Display(Name = "Evidence note")]
        Evidence = 1,

        [Display(Name = "Evidence note transfer")]
        Transfer = 2
    }
}