namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core;
    using ViewModels;

    public class CreateNoteMapTransfer
    {
        public List<SchemeData> Schemes { get; private set; }

        public EvidenceNoteViewModel ExistingModel { get; set; }

        public CreateNoteMapTransfer(List<SchemeData> schemes, EvidenceNoteViewModel existingModel)
        {
            Guard.ArgumentNotNull(() => schemes, schemes);

            ExistingModel = existingModel;
            Schemes = schemes;
        }
    }
}