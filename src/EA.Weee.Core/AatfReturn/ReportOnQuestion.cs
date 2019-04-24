﻿namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;

    public class ReportOnQuestion
    {
        public ReportOnQuestion(int id, string question, string description, int parentId)
        {
            Id = id;
            Question = question;
            Description = description;
            ParentId = parentId;
        }

        public int Id { get; set; }

        public string Question { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

        public string SelectedValue { get; set; }

        public IList<string> PossibleValues => new List<string>() { "Yes", "No" };
    }
}
