namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;

    public class ReportOnQuestion
    {
        public ReportOnQuestion(int id, string question, string description, int? parentId, string alternativeDescription)
        {
            Id = id;
            Question = question;
            Description = description;
            ParentId = parentId;
            AlternativeDescription = alternativeDescription;
        }

        public ReportOnQuestion()
        {
        }

        public int Id { get; set; }

        public string Question { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

        public string AlternativeDescription { get; set; }

        public bool Selected { get; set; }

        public bool Deselected { get; set; }

        public bool HasError { get; set; }
    }
}
