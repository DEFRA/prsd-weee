namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Aatf : Entity
    {
        public virtual string Name { get; private set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        public virtual string ApprovalNumber { get; private set; }

        public virtual AatfStatus AatfStatus { get; private set; }

        public virtual Operator Operator { get; private set; }
        public virtual AatfAddress SiteAddress { get; private set; }
        public virtual AatfSize Size { get; private set; }
        public virtual DateTime? ApprovalDate { get; private set; }

        public virtual AatfContact Contact { get; private set; }

        protected Aatf()
        {
        }

        public Aatf(string name, UKCompetentAuthority competentAuthority, string approvalNumber, AatfStatus aatfStatus, Operator @operator, AatfAddress aatfSiteAddress, AatfSize aatfSize, DateTime approvalDate, AatfContact contact)
        {
            Guard.ArgumentNotNullOrEmpty(() => name, name);
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);
            Guard.ArgumentNotNull(() => competentAuthority, competentAuthority);
            Guard.ArgumentNotNull(() => aatfStatus, aatfStatus);
            Guard.ArgumentNotNull(() => @operator, @operator);
            Guard.ArgumentNotNull(() => contact, contact);
            Guard.ArgumentNotNull(() => aatfSize, aatfSize);
            Guard.ArgumentNotNull(() => aatfSiteAddress, aatfSiteAddress);

            Name = name;
            CompetentAuthority = competentAuthority;
            ApprovalNumber = approvalNumber;
            AatfStatus = aatfStatus;
            Operator = @operator;
            Size = aatfSize;
            SiteAddress = aatfSiteAddress;
            ApprovalDate = approvalDate;
            Contact = contact;
        }
    }
}
