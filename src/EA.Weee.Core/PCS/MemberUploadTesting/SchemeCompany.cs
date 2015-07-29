using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class SchemeCompany
    {
        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }

        public static SchemeCompany Create(ICompanySettings settings)
        {
            SchemeCompany schemeCompany = new SchemeCompany();

            schemeCompany.CompanyName = RandomHelper.CreateRandomString(string.Empty, 1, 255);
            schemeCompany.CompanyNumber = RandomHelper.CreateRandomStringOfNumbers(8, 8);

            return schemeCompany;
        }
    }
}
