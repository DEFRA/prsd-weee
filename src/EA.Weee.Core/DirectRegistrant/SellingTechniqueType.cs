﻿namespace EA.Weee.Core.DirectRegistrant
{
    using System.ComponentModel.DataAnnotations;
    public enum SellingTechniqueType
    {
        [Display(Name = "Direct selling to end user (mail, order, internet etc)")]
        DirectSellingToEndUser = 0,

        [Display(Name = "Indirect selling (other)")]
        IndirectSellingToEndUser = 1,

        [Display(Name = "Direct selling to end user (mail, order, internet etc) and Indirect selling (other)")]
        Both = 2
    }
}
