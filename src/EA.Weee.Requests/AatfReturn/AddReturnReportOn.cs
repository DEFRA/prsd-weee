namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class AddReturnReportOn : IRequest<Guid>
    {
        public Guid ReturnId { get; set; }

        public IList<> SelectedOptions { get; set; }
    }
}
