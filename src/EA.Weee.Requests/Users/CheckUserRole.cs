namespace EA.Weee.Requests.Users
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CheckUserRole : IRequest<bool>
    {
        public Roles Role { get; private set; }

        public CheckUserRole(Roles role)
        {
            this.Role = role;
        }
    }
}
