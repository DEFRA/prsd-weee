namespace EA.Weee.Core.AatfReturn
{
    public class ReportOnQuestion
    {
        public ReportOnQuestion(string question, string description, int parentId)
        {
            Question = question;
            Description = description;
            ParentId = parentId;
        }

        public string Question { get; set; }

        public string Description { get; set; }

        public int ParentId { get; set; }
    }
}
