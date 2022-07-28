namespace EA.Weee.Tests.Core.DataHelpers
{
    using System.Collections;
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Evidence;

    public class NoteTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var type in Enumeration.GetAll<NoteType>())
            {
                yield return new object[] { type };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
