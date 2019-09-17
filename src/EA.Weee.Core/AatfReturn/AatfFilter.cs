namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;

    public class AatfFilter
    {
        public AatfFilter()
        {
        }

        public string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public List<Guid> SelectedAuthority { get; set; }
    }
}
