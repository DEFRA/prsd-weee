namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using Domain.AatfReturn;

    public class AatfSubmissionHistory
    {
        public virtual Guid ReturnId { get; set; }

        public virtual Return Return { get; set; }
    }
}
