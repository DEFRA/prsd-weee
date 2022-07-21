namespace EA.Weee.Tests.Core.DataHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class NoteTypeCoreData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var noteType in Enum.GetValues(typeof(EA.Weee.Core.AatfEvidence.NoteType)))
            {
                yield return new[] { noteType };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}