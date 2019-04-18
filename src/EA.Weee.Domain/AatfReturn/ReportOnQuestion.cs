namespace EA.Weee.Domain.AatfReturn
{
    using System.ComponentModel.DataAnnotations;
    using EA.Prsd.Core.Domain;

    public class ReportOnQuestion
    {
        [Key]
        public int Id { get; private set; }

        public string Question { get; private set; }

        public string Description { get; private set; }

        public int ParentId { get; private set; }
    }
}
