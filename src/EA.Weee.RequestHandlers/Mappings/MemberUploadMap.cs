namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Scheme;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class MemberUploadMap : IMap<MemberUpload, MemberUploadData>
    {
        public MemberUploadData Map(MemberUpload source)
        {
            return new MemberUploadData
            {
                OrganisationId = source.OrganisationId,
                Id = source.Id,
                ComplianceYear = source.ComplianceYear,
                SchemeId = source.SchemeId,
                IsSubmitted = source.IsSubmitted,
                TotalCharges = source.TotalCharges
            };
        }
    }
}
