namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain.PCS;
    using Prsd.Core.Mapper;
    using Requests.Shared;

    public class MemberUploadErrorMap : IMap<MemberUploadError, MemberUploadErrorData>
    {
        public MemberUploadErrorData Map(MemberUploadError source)
        {
            return new MemberUploadErrorData
            {
                ErrorLevel = (ErrorLevel)source.ErrorLevel.Value,
                Description = source.Description
            };
        }
    }
}
