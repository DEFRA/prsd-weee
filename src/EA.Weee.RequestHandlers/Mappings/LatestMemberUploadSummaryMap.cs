namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.PCS;
    using Domain.PCS;
    using Domain.Producer;
    using Prsd.Core.Mapper;

    public class LatestMemberUploadSummaryMap : IMap<IEnumerable<MemberUpload>, LatestMemberUploadList>
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
