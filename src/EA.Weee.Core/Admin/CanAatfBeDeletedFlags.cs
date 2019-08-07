namespace EA.Weee.Core.Admin
{
    using System;

    [Flags]
    public enum CanAatfBeDeletedFlags
    {
        HasData = 1,
        OrganisationHasActiveUsers = 2,
        OrganisationHasOtherRelations = 4,
        CanDelete = 6,
        CanDeleteOrganisation = 8
    }
}
