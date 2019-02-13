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
        public string Name { get; private set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        public string ApprovalNumber { get; private set; }

        public virtual AatfStatus AatfStatus { get; private set; }

        public virtual Operator Operator { get; private set; }

        protected Aatf()
        {
        }

        public Aatf(string name, UKCompetentAuthority competentAuthority, string approvalNumber, AatfStatus aatfStatus, Operator @operator)
        {
            Guard.ArgumentNotNullOrEmpty(() => name, name);
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);
            Guard.ArgumentNotNull(() => competentAuthority, competentAuthority);
            Guard.ArgumentNotNull(() => aatfStatus, aatfStatus);
            Guard.ArgumentNotNull(() => @operator, @operator);

            Name = name;
            CompetentAuthority = competentAuthority;
            ApprovalNumber = approvalNumber;
            AatfStatus = aatfStatus;
            Operator = @operator;
        }
    }
}
