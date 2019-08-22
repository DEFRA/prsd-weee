namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;

    public class AddReturnReportOn : IRequest<bool>
    {
        public Guid ReturnId { get; set; }

        public IList<int> SelectedOptions { get; set; }

        public IList<int> DeselectedOptions { get; set; }

        public IList<ReportOnQuestion> Options { get; set; }

        public bool DcfSelectedValue { get; set; }
    }
}
