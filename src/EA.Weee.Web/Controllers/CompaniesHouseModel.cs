namespace EA.Weee.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Web;

    public class CompaniesHouseModel
    {
        public string RegistrationNumber { get; set; }

        public string Error { get; set; }

        public Organisation Organisation { get; set; }

        public Meta _meta { get; set; }

        public Info _info { get; set; }
    }
}