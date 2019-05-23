namespace EA.Weee.Requests.AatfReturn.Internal
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class EditAatfDetails : IRequest<bool>
    {
        public AatfData Data { get; set; }
    }
}
