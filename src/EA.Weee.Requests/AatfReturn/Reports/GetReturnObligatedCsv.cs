namespace EA.Weee.Requests.AatfReturn.Reports
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class GetReturnObligatedCsv : IRequest<CSVFileData>
    {
        public Guid ReturnId { get; private set; }

        public GetReturnObligatedCsv(Guid returnId)
        {
            ReturnId = returnId;
        }
    }
}
