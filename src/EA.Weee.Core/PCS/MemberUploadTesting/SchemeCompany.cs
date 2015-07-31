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

            if (!settings.IgnoreStringLengthConditions)
            {
                schemeCompany.CompanyName = RandomHelper.CreateRandomString(string.Empty, 1, 255);
                schemeCompany.CompanyNumber = RandomHelper.CreateRandomStringOfNumbers(8, 8);
            }
            else
            {
                schemeCompany.CompanyName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
                schemeCompany.CompanyNumber = RandomHelper.CreateRandomStringOfNumbers(0, 1000);
            }

            return schemeCompany;
        }
    }
}
