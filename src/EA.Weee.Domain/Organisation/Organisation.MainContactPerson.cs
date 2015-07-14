namespace EA.Weee.Domain.Organisation
{
    using Prsd.Core;

    public partial class Organisation
    {
        public bool HasContact
        {
            get { return Contact != null; }
        }

        public void AddMainContactPerson(Contact contactPerson)
        {
            Guard.ArgumentNotNull(() => contactPerson, contactPerson);

            if (!HasContact)
            {
                Contact = contactPerson;
            }
            else
            {
                contactPerson.CopyValuesInto(Contact);
            }
        }
    }
}
