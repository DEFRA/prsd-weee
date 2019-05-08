namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class RemoveWeeeSentOn : IRequest<bool>
    {
        public Guid WeeeSentOnId { get; set; }

        public RemoveWeeeSentOn(Guid weeeSentOnId)
        {
            WeeeSentOnId = weeeSentOnId;
        }
    }
}
