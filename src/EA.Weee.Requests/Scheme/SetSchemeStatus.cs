namespace EA.Weee.Requests.Scheme
{
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class SetSchemeStatus : IRequest<Guid>
    {
        public Guid PcsId { get; set; }

        public SchemeStatus Status { get; set; }

        public SetSchemeStatus(Guid pcsId, SchemeStatus status)
        {
            PcsId = pcsId;
            Status = status;
        }
    }
}
