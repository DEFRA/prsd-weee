﻿namespace EA.Weee.Core.Scheme.MemberUploadTesting
{
    public class CorrespondentForNotices
    {
        public ContactDetails ContactDetails { get; set; }

        public CorrespondentForNotices()
        {
        }

        public static CorrespondentForNotices Create(ICorrespondentForNoticesSettings settings)
        {
            CorrespondentForNotices correspondentForNotices = new CorrespondentForNotices();

            if (RandomHelper.OneIn(2))
            {
                correspondentForNotices.ContactDetails = ContactDetails.Create(settings);
            }

            return correspondentForNotices;
        }
    }
}
