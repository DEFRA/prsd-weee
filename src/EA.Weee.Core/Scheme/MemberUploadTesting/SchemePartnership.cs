namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    using System.Collections.Generic;

    public class SchemePartnership
    {
        public string PartnershipName { get; set; }
        public List<string> PartnershipList { get; private set; }

        public SchemePartnership()
        {
            PartnershipList = new List<string>();
        }

        public static SchemePartnership Create(ISettings settings)
        {
            SchemePartnership schemePartnership = new SchemePartnership();

            if (!settings.IgnoreStringLengthConditions)
            {
                schemePartnership.PartnershipName = RandomHelper.CreateRandomString(string.Empty, 1, 255);
            }
            else
            {
                schemePartnership.PartnershipName = RandomHelper.CreateRandomString(string.Empty, 0, 1000);
            }

            int numberOfParterships = RandomHelper.R.Next(1, 5);
            for (int index = 0; index < numberOfParterships; ++index)
            {
                if (!settings.IgnoreStringLengthConditions)
                {
                    schemePartnership.PartnershipList.Add(RandomHelper.CreateRandomString(string.Empty, 1, 70));
                }
                else
                {
                    schemePartnership.PartnershipList.Add(RandomHelper.CreateRandomString(string.Empty, 0, 1000));
                }
            }

            return schemePartnership;
        }
    }
}
