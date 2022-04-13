namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using Web.Areas.Aatf.ViewModels;

    public class ManageEvidenceNotesViewModelMapTests
    {
        private readonly ManageEvidenceNoteViewModel map;
        private readonly Fixture fixture;

        /// <summary>
        /// TODO:
        /// </summary>
        public ManageEvidenceNotesViewModelMapTests()
        {
            map = new ManageEvidenceNoteViewModel();

            fixture = new Fixture();
        }
    }
}
