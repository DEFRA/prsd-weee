namespace EA.Weee.Web.Areas.AatfEvidence.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core;

    public class CreateNoteMapTransfer
    {
        public List<SchemeData> Schemes { get; private set; }

        public bool IsEdit { get; private set; }

        public CreateNoteMapTransfer(List<SchemeData> schemes, bool isEdit)
        {
            Guard.ArgumentNotNull(() => schemes, schemes);

            Schemes = schemes;
            IsEdit = isEdit;
        }
    }
}