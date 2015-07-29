using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class AuthorizedRepresentative
    {
        public OverseasProducer OverseasProducer { get; set; }

        public AuthorizedRepresentative()
        {

        }

        public static AuthorizedRepresentative Create(IAuthorizedRepresentativeSettings settings)
        {
            AuthorizedRepresentative authorizedRepresentative = new AuthorizedRepresentative();

            if (RandomHelper.OneIn(2))
            {
                authorizedRepresentative.OverseasProducer = OverseasProducer.Create(settings);
            }

            return authorizedRepresentative;
        }
    }
}
