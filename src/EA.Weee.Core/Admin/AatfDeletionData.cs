namespace EA.Weee.Core.Admin
{
    public class AatfDeletionData
    {
        public CanOrganisationBeDeletedFlags CanOrganisationBeDeletedFlags { get; private set; }

        public CanAatfBeDeletedFlags CanAatfBeDeletedFlags { get; private set; }

        public AatfDeletionData(CanOrganisationBeDeletedFlags canOrganisationBeDeletedFlags,
            CanAatfBeDeletedFlags canAatfBeDeletedFlags)
        {
            CanOrganisationBeDeletedFlags = canOrganisationBeDeletedFlags;
            CanAatfBeDeletedFlags = canAatfBeDeletedFlags;
        }
    }
}
