﻿namespace EA.Weee.Requests.Organisations
{
    using System.ComponentModel.DataAnnotations;

    public enum OrganisationUserStatus
    {
        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "Approved")]
        Approved = 2,

        [Display(Name = "Refused")]
        Refused = 3
    }
}
