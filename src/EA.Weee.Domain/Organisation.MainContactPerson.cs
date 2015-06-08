namespace EA.Weee.Domain
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using System;

    public partial class Organisation
    {
       public void AddMainContactPerson(Contact contactPerson)
       {
           Contact = contactPerson;
       }

        public void RemoveContact()
        {
            Contact = null;
        }
    }
}
