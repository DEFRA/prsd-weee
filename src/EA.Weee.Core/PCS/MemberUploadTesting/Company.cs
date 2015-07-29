using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class Company
    {
        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }
        public ContactDetails RegisteredOffice { get; set; }

        public static Company Create(ICompanySettings settings)
        {
            Company company = new Company();

            company.CompanyName = RandomHelper.CreateRandomString(string.Empty, 1, 50); //255?
            company.CompanyNumber = RandomHelper.CreateRandomStringOfNumbers(8, 8);
            company.RegisteredOffice = ContactDetails.Create(settings);

            return company;
        }
    }
}
