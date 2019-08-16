namespace EA.Weee.Requests.Admin.Aatf
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class EditAatfContact : IRequest<bool>
    {
        public AatfContactData ContactData { get; set; }
    }
}
