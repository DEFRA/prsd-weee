namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetSchemeStatus : IRequest<SchemeStatus>
    {
        public Guid PcsId { get; set; }

        public GetSchemeStatus(Guid pcsId)
        {
            PcsId = pcsId;
        }
    }
}
