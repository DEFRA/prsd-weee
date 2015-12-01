namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using Domain.Scheme;
    using Prsd.Core.Mapper;

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
