namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    using System.Collections.Generic;

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

        public static Partnership Create(ISettings settings)
        {
            Partnership partnership = new Partnership();

            if (!settings.IgnoreStringLengthConditions)
            {
                partnership.PartnershipName = RandomHelper.CreateRandomString(string.Empty, 1, 255);
            }
            else
            {
                partnership.PartnershipName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
            }

            int numberOfParterships = RandomHelper.R.Next(1, 5);
            for (int index = 0; index < numberOfParterships; ++index)
            {
                if (!settings.IgnoreStringLengthConditions)
                {
                    partnership.PartnershipList.Add(RandomHelper.CreateRandomString(string.Empty, 1, 70));
                }
                else
                {
                    partnership.PartnershipList.Add(RandomHelper.CreateRandomString(string.Empty, 0, 1000));
                }
            }

            partnership.PrincipalPlaceOfBusiness = ContactDetails.Create(settings, true);

            return partnership;
        }
    }
}
