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

        public virtual int Id { get; set; }

        public string Question { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

        public string AlternativeDescription { get; set; }

        public virtual bool Selected { get; set; }

        public virtual bool DeSelected => OriginalSelected && !Selected;

        public virtual bool ReSelected => Selected && !OriginalSelected;

        public virtual bool HasError { get; set; }

        public virtual bool OriginalSelected { get; set; }
    }
}
