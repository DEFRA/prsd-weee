namespace EA.Weee.Tests.Core.DataHelpers
{
    using System.Collections;
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Evidence;

    public class NoteStatusData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var status in Enumeration.GetAll<NoteStatus>())
            {
                yield return new object[] { status };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
