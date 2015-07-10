namespace EA.Weee.Domain
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using System;

    public partial class Organisation
    {
        public bool HasContact
        {
            get { return Contact != null; }
        }

        public void AddMainContactPerson(Contact contactPerson)
        {
            Guard.ArgumentNotNull(() => contactPerson, contactPerson);

            this.Contact = contactPerson;
        }
    }
}
