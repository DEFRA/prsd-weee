namespace EA.Weee.Domain.AatfReturn
{
    using System;

    public class ReturnReportOn : ReturnEntity, IReturnOption
    {
        public ReturnReportOn()
        {
        }

        public ReturnReportOn(Guid returnId, int reportOnQuestId)
        {
            this.ReturnId = returnId;
            this.ReportOnQuestionId = reportOnQuestId;
        }

        public int ReportOnQuestionId { get; private set; }
    }
}
