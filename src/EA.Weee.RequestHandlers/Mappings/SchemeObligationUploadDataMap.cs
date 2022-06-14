namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Admin.Obligation;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using Domain.Obligation;
    using EA.Weee.Core.DataReturns;
    using Prsd.Core.Mapper;

    public class SchemeObligationUploadDataMap : IMap<ObligationUpload, List<SchemeObligationUploadErrorData>>
    {
        public List<SchemeObligationUploadErrorData> Map(ObligationUpload source)
        {
            Condition.Requires(source).IsNotNull();

            var returnData = source.ObligationUploadErrors.Select(oe => new SchemeObligationUploadErrorData(
                oe.ErrorType.ToCoreEnumeration<SchemeObligationUploadErrorType>(),
                oe.Description,
                oe.SchemeIdentifier,
                oe.SchemeName,
                (WeeeCategory?)oe.Category)).ToList();

            return returnData;
        }
    }
}
