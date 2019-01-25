namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using Prsd.Core.Mediator;

    public class NonObligatedRequest : IRequest<Guid>
    {
        public Guid NonObligatedId { get; set; }

        public Guid ReturnId { get; set; }

        public int CategoryId { get; set; }

        public bool Dcf { get; set; }

        public decimal Tonnage { get; set; }
    }
}
