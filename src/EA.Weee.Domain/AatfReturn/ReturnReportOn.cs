namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core.Domain;

    public class ReturnReportOn : Entity
    {
        public Guid ReturnId { get; private set; }

        public int ReportOnQuestionId { get; private set; }
    }
}
