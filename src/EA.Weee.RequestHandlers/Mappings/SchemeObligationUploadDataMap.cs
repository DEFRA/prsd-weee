namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Core.Admin.Obligation;
    using Core.Helpers;
    using Domain.Obligation;
    using Prsd.Core.Mapper;

    internal class SchemeObligationUploadDataMap : IMap<ObligationUpload, SchemeObligationUploadData>
    {
        public SchemeObligationUploadData Map(ObligationUpload source)
        {
            return new SchemeObligationUploadData()
            {
                ErrorData = source.ObligationUploadErrors.Select(oe => new SchemeObligationUploadErrorData(
                    oe.ErrorType.ToCoreEnumeration<SchemeObligationUploadErrorType>(),
                    oe.Description,
                    oe.SchemeIdentifier,
                    oe.SchemeName,
                    oe.Category)).ToList()
            };
        }
    }
}
