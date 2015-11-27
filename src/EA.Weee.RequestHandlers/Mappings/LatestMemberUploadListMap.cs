namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Scheme;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;

    public class LatestMemberUploadListMap : IMap<IEnumerable<MemberUpload>, LatestMemberUploadList>
    {
        public LatestMemberUploadList Map(IEnumerable<MemberUpload> source)
        {
            return new LatestMemberUploadList
            {
                LatestMemberUploads =
                    source.Select(mu => new LatestMemberUpload
                    {
                        ComplianceYear = mu.ComplianceYear.Value,
                        UploadId = mu.Id
                    })
            };
        }
    }
}
