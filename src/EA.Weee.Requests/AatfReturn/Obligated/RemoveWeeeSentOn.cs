namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class RemoveWeeeSentOn : IRequest<bool>
    {
        public Guid WeeeSentOnId { get; set; }

        public bool IsAatf { get; set; }

        public RemoveWeeeSentOn(Guid weeeSentOnId, bool isAatf = false)
        {
            WeeeSentOnId = weeeSentOnId;
            IsAatf = isAatf;
        }
    }
}
