namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class AddReturnScheme : IRequest<Guid>
    {
        public Guid SchemeId { get; set; }

        public Guid ReturnId { get; set; }
    }
}
