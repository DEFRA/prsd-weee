namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;

    public partial class DirectRegistrant
    {
        public bool HasContact => Contact != null;

        public void AddOrUpdateMainContactPerson(Contact contactPerson)
        {
            Guard.ArgumentNotNull(() => contactPerson, contactPerson);

            Contact = contactPerson.OverwriteWhereNull(Contact);
        }
    }
}
