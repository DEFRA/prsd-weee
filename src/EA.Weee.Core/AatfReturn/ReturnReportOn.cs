namespace EA.Weee.Core.AatfReturn
{
    using System;

    public class ReturnReportOn 
    {
        public ReturnReportOn(int reportOnQuestionId, Guid returnId)
        {
            ReportOnQuestionId = reportOnQuestionId;
            ReturnId = returnId;
        }

        public int ReportOnQuestionId { get; set; }

        public Guid ReturnId { get; set; }
    }
}
