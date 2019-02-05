using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    public class AatfTaskListViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public Guid ReturnId { get; set; }

        public string Quarter { get; set; }

        public string Period { get; set; }

        public string Year { get; set; }

        public List<string> Aatfs { get; set; }
    }
}