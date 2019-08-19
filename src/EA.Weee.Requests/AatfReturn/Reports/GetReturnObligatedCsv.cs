namespace EA.Weee.Requests.AatfReturn.Reports
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetReturnObligatedCsv : IRequest<CSVFileData>
    {
        public Guid ReturnId { get; private set; }

        public GetReturnObligatedCsv(Guid returnId)
        {
            ReturnId = returnId;
        }
    }
}
