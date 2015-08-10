namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Scheme;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class LatestMemberUploadListMap : IMap<IEnumerable<MemberUpload>, LatestMemberUploadList>
    {
        public LatestMemberUploadList Map(IEnumerable<MemberUpload> source)
        {
            return new LatestMemberUploadList
            {
                LatestMemberUploads =
                    source.Select(mu => new LatestMemberUpload
                    {
                        ComplianceYear = mu.ComplianceYear,
                        UploadId = mu.Id,
                        CsvFileSizeEstimate = Encoding.UTF8.GetByteCount(mu.Scheme.GetProducerCSV(mu.ComplianceYear))      
                    })
            };
        }
    }
}
