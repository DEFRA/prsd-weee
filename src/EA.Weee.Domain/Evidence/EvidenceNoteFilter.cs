namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Prsd.Core.Domain;

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
                var allowedNoteTypes = Enumeration.GetAll<Domain.Evidence.NoteType>().Select(n => n.DisplayName.ToCharArray()[0]).ToArray();

                if (!string.IsNullOrWhiteSpace(searchRef))
                {
                    return searchRef.Trim().Trim(allowedNoteTypes);
                }

                return searchRef;
            }
            set => searchRef = value;
        }
    }
}
