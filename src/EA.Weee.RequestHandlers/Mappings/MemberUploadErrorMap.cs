namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

    public class MemberUploadErrorMap : IMap<MemberUploadError, ErrorData>
    {
        public ErrorData Map(MemberUploadError source)
        {
            return new ErrorData(source.Description, (ErrorLevel)source.ErrorLevel.Value);
        }
    }
}
