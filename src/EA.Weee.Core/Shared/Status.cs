﻿namespace EA.Weee.Core.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum Status
    {
        [Display(Name = "Incomplete")]
        Incomplete = 1,
        [Display(Name = "Complete")]
        Complete = 2
    }
}
