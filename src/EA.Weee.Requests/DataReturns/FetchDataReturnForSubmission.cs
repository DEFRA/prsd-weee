namespace EA.Weee.Requests.DataReturns
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DataReturns;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FetchDataReturnForSubmission : IRequest<DataReturnForSubmission>
    {
        public Guid DataReturnId { get; private set; }

        public FetchDataReturnForSubmission(Guid dataReturnId)
        {
            DataReturnId = dataReturnId;
        }
    }
}
