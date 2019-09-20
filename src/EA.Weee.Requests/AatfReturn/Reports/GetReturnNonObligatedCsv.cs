namespace EA.Weee.Requests.AatfReturn.Reports
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class GetReturnNonObligatedCsv : IRequest<CSVFileData>
    {
        public Guid ReturnId { get; private set; }

        public GetReturnNonObligatedCsv(Guid returnId)
        {
            ReturnId = returnId;
        }
    }
}
