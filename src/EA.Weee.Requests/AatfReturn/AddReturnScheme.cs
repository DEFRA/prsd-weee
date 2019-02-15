namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class AddReturnScheme : IRequest<Guid>
    {
        public Guid SchemeId { get; set; }

        public Guid ReturnId { get; set; }
    }
}
