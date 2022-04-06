namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core;
    using EA.Weee.Domain.Evidence;
    using System.Collections.Generic;

    public class ListOfNotesMap
    {
        public List<Note> ListOfNotes { get; protected set; }

        public ListOfNotesMap(List<Note> listOfNotes)
        {
            Guard.ArgumentNotNull(() => listOfNotes, listOfNotes);
            ListOfNotes = listOfNotes;
        }
    }
}
