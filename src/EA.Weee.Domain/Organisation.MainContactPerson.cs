namespace EA.Weee.Domain
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using System;

    public partial class Organisation
    {
        public void AddMainContactPerson(Contact contactPerson)
        {
            Guard.ArgumentNotNull(() => contactPerson, contactPerson);
            if (Contact != null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot add Contact to Organisation {0}. This organisation already has a Contact.",
                        this.Id));
            }

            this.Contact = contactPerson;
        }
    }
}
