namespace EA.Weee.Requests.Admin.Aatf
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class EditAatfDetails : IRequest<bool>
    {
        public AatfData Data { get; set; }
    }
}
