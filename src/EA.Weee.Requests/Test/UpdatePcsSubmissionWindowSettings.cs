namespace EA.Weee.Requests.Test
{
    using System;
    using Prsd.Core.Mediator;

    public class UpdatePcsSubmissionWindowSettings : IRequest<bool>
    {
        public bool FixCurrentDate { get; set; }

        public DateTime? CurrentDate { get; set; }
    }
}
