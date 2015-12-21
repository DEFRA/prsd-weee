namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class MemberUploadErrorMap : IMap<MemberUploadError, UploadErrorData>
    {
        public UploadErrorData Map(MemberUploadError source)
        {
            return new UploadErrorData
            {
                ErrorLevel = (ErrorLevel)source.ErrorLevel.Value,
                Description = source.Description
            };
        }
    }
}
