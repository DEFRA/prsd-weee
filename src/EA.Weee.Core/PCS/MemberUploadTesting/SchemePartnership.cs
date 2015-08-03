﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class SchemePartnership
    {
        public string PartnershipName { get; set; }
        public List<string> PartnershipList { get; private set; }

        public SchemePartnership()
        {
            PartnershipList = new List<string>();
        }

        public static SchemePartnership Create(IPartnershipSettings settings)
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
