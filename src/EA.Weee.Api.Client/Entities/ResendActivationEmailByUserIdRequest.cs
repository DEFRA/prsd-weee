﻿namespace EA.Weee.Api.Client.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ResendActivationEmailByUserIdRequest
    {
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public string ActivationBaseUrl { get; set; }
    }
}
