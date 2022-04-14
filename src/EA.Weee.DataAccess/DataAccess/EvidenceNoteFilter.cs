namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Evidence;

    public class EvidenceNoteFilter
    {
        public Guid? AatfId { get; set; }

        public Guid? OrganisationId { get; set; }

        public Guid? SchemeId { get; set; }

        public List<NoteStatus> AllowedStatuses { get; set; }

        private string searchRef;
        public string SearchRef
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(searchRef))
                {
                    return new string(searchRef.Trim().Where(char.IsDigit).ToArray());
                }

                return searchRef;
            }
            set => searchRef = value;
        }
    }
}
