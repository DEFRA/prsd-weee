namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetSchemePublicInfoBySchemeId : IRequest<SchemePublicInfo>
    {
        public Guid SchemeId { get; private set; }

        public GetSchemePublicInfoBySchemeId(Guid schemeId)
        {
            SchemeId = schemeId;
        }
    }
}
