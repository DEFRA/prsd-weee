namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class AddReturnReportOn : IRequest<bool>
    {
        public Guid ReturnId { get; set; }

        public IList<int> SelectedOptions { get; set; }

        public IList<int> DeselectedOptions { get; set; }

        public IList<ReportOnQuestion> Options { get; set; }

        public string DcfSelectedValue { get; set; }
    }
}
