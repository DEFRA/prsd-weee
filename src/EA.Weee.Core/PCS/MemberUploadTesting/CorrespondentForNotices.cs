using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
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
