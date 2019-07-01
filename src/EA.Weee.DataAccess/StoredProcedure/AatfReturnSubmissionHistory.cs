namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using Domain.AatfReturn;

    public class AatfReturnSubmissionHistory
    {
        public virtual Guid ReturnId { get; set; }

        public virtual Return Return { get; set; }
    }
}
