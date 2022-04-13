namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using EA.Prsd.Core.Domain;
    using System.Collections;
    using System.Collections.Generic;
    using Domain.Evidence;

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
