namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;

    public class AatfContact : Entity
    {
        public AatfContact()
        {
        }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Position { get; set; }

        public virtual string Telephone { get; set; }

        public virtual string Email { get; set; }
    }
}