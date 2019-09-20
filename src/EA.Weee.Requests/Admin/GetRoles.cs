namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;
    using Security;
    using System.Collections.Generic;

    public class GetRoles : IRequest<List<Role>>
    {
    }
}
