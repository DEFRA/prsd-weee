namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System.Collections;
    using System.Collections.Generic;
    using Domain.Evidence;
    using Prsd.Core.Domain;

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
