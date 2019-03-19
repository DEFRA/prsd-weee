namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditSentOnAatfSite : AddSentOnAatfSite
    {
        public Guid WeeeSentOnId { get; private set; }
    }
}
