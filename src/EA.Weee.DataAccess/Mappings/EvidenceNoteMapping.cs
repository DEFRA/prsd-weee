namespace EA.Weee.DataAccess.Mappings
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    internal class EvidenceNoteMapping : EntityTypeConfiguration<Note>
    {
        public EvidenceNoteMapping()
        {
            ToTable("Note", "Evidence");
            HasKey(n => n.Id);
            Property(n => n.Reference).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity).IsRequired();
            Property(n => n.StartDate).IsRequired();
            Property(n => n.EndDate).IsRequired();
            Property(n => n.WasteType).IsOptional();
            Property(n => n.Protocol).IsOptional();
            Property(n => n.CreatedById).IsRequired();
            Property(n => n.CreatedDate).IsRequired();
            Property(n => n.OrganisationId).IsRequired();
            Property(n => n.AatfId).IsRequired();
            Property(n => n.ComplianceYear).IsRequired();
            Property(n => n.Status.Value).HasColumnName("Status").IsRequired();
            Property(n => n.NoteType.Value).HasColumnName("NoteType").IsRequired();

            HasRequired(n => n.Organisation);
            HasRequired(n => n.Recipient);
            HasRequired(n => n.Aatf);
        }
    }
}
