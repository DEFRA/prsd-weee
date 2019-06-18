namespace EA.Weee.Requests.Admin
{
    using System.Collections.Generic;
    using Core.Admin;
    using Prsd.Core.Mediator;
    using Prsd.Core.Security;

    public class GetLocalAreas : IRequest<List<LocalAreaData>>
    {
    }
}