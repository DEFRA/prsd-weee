namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class RemoveScheme : IRequest<bool>
    {
        public Guid SchemeId { get; set; }
    }
}
