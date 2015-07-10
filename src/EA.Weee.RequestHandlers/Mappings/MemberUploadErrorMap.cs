namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain;
    using EA.Weee.Requests.Shared;
    using ErrorLevel = Core.Shared.ErrorLevel;

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
