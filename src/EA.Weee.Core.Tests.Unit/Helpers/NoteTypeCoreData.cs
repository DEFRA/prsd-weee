namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class NoteTypeCoreData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var status in Enum.GetValues(typeof(EA.Weee.Core.AatfEvidence.NoteType)))
            {
                yield return new[] { status };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}