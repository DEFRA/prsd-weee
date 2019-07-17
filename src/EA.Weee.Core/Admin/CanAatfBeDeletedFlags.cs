namespace EA.Weee.Core.Admin
{
    using System;

    [Flags]
    public enum CanAatfBeDeletedFlags
    {
        HasData = 1,
        OrganisationHasMoreAatfs = 2,
        HasActiveUsers = 4,
        Yes = 8
    }
}
