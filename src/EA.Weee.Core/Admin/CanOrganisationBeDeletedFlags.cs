namespace EA.Weee.Core.Admin
{
    using System;

    [Flags]
    public enum CanOrganisationBeDeletedFlags
    {
        HasActiveUsers = 1,
        HasReturns = 2
    }
}
