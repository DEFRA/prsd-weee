namespace EA.Weee.Requests.Scheme
{
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class GetSchemeStatus : IRequest<SchemeStatus>
    {
        public Guid PcsId { get; set; }

        public GetSchemeStatus(Guid pcsId)
        {
            PcsId = pcsId;
        }
    }
}
