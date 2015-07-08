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
            //if (Contact != null)
            //{
            //    throw new InvalidOperationException(
            //        "Cannot add Contact person to Organisation. This organisation already has a Contact.");
            //}

            this.Contact = contactPerson;
        }
    }
}
