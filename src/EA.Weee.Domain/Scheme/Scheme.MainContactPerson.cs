namespace EA.Weee.Domain.Scheme
{
    using Organisation;
    using Prsd.Core;

    public partial class Scheme
    {
        public bool HasContact => Contact != null;

        public void AddOrUpdateMainContactPerson(Contact contactPerson)
        {
            Guard.ArgumentNotNull(() => contactPerson, contactPerson);

            Contact = contactPerson.OverwriteWhereNull(Contact);
        }
    }
}
