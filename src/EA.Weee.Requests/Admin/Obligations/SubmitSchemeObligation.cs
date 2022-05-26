namespace EA.Weee.Requests.Admin.Obligations
{
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class SubmitSchemeObligation : IRequest<object>
    {
        public FileInfo FileInfo { get; set; }
    }
}
