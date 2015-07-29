using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class Partnership
    {
        public string PartnershipName { get; set; }
        public List<string> PartnershipList { get; private set; }
        public ContactDetails PrincipalPlaceOfBusiness { get; private set; }

        public Partnership()
        {
            PartnershipList = new List<string>();
            PrincipalPlaceOfBusiness = new ContactDetails();
        }

        public static Partnership Create(IPartnershipSettings settings)
        {
            Partnership partnership = new Partnership();

            partnership.PartnershipName = RandomHelper.CreateRandomString(string.Empty, 1, 255);

            int numberOfParterships = RandomHelper.R.Next(1, 5);
            for (int index = 0; index < numberOfParterships; ++index)
            {
                partnership.PartnershipList.Add(RandomHelper.CreateRandomString(string.Empty, 1, 70));
            }

            partnership.PrincipalPlaceOfBusiness = ContactDetails.Create(settings);

            return partnership;
        }
    }
}
