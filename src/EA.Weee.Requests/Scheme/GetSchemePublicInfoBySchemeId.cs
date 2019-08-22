namespace EA.Weee.Requests.Scheme
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System;

    public class GetSchemePublicInfoBySchemeId : IRequest<SchemePublicInfo>
    {
        public Guid SchemeId { get; private set; }

        public GetSchemePublicInfoBySchemeId(Guid schemeId)
        {
            SchemeId = schemeId;
        }
    }
}
