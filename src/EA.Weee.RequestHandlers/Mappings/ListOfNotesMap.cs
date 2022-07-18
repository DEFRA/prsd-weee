namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core;
    using EA.Weee.Domain.Evidence;
    using System.Collections.Generic;

    public class ListOfNotesMap
    {
        public List<Note> ListOfNotes { get; protected set; }

        public List<int> CategoryFilter { get; set; }

        public bool IncludeTonnage { get; set; }

        public ListOfNotesMap(List<Note> listOfNotes, bool includeTonnage)
        {
            Guard.ArgumentNotNull(() => listOfNotes, listOfNotes);
            ListOfNotes = listOfNotes;
            CategoryFilter = new List<int>();
            IncludeTonnage = includeTonnage;
        }
    }
}
