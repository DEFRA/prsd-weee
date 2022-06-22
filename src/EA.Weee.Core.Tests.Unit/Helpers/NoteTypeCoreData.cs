namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using EA.Weee.Core.Shared;

    public class NoteTypeCoreData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var noteType in Enum.GetValues(typeof(NoteType)))
            {
                yield return new[] { noteType };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}