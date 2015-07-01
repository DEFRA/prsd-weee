namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Domain;
    using EA.Weee.Requests.Shared;
    using ErrorLevel = EA.Weee.Requests.Shared.ErrorLevel;

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
