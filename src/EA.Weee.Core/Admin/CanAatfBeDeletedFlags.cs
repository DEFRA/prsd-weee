namespace EA.Weee.Core.Admin
{
    using System;

    [Flags]
    public enum CanAatfBeDeletedFlags
    {
        HasData = 1,
        OrganisationHasActiveUsers = 2,
        CanDelete = 4,
        CanDeleteOrganisation = 8,
        HasNotes = 9,
        IsNotLatest = 10,
    }
}
