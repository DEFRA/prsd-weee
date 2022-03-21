namespace EA.Weee.Web.Areas.AatfEvidence.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core;

    public class CreateNoteMapTransfer
    {
        public List<SchemeData> Schemes { get; private set; }

        public CreateNoteMapTransfer(List<SchemeData> schemes)
        {
            Guard.ArgumentNotNull(() => schemes, schemes);

            Schemes = schemes;
        }
    }
}