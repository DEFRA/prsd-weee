namespace EA.Weee.Requests.Admin.Aatf
{
    using System;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    [Serializable]
    public class EditAatfDetails : IRequest<bool>
    {
        public AatfData Data { get; set; }
    }
}
