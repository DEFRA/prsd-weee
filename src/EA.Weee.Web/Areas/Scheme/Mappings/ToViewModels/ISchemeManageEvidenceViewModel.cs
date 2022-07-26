namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using Core.Scheme;

    public interface ISchemeManageEvidenceViewModel
    {
        SchemePublicInfo SchemeInfo { get; set; }

        bool CanSchemeManageEvidence { get; set; }
    }
}
