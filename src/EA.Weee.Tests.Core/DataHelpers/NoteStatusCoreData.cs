namespace EA.Weee.Tests.Core.DataHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class NoteStatusCoreData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var status in Enum.GetValues(typeof(EA.Weee.Core.AatfEvidence.NoteStatus)))
            {
                yield return new[] { status };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
