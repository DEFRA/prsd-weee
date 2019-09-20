namespace EA.Weee.Requests.Test
{
    using Prsd.Core.Mediator;
    using System;

    public class UpdatePcsSubmissionWindowSettings : IRequest<bool>
    {
        public bool FixCurrentDate { get; set; }

        public DateTime? CurrentDate { get; set; }
    }
}
