namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System.Collections;
    using System.Collections.Generic;
    using Domain.Evidence;

    public class WasteTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var waste in typeof(WasteType).GetEnumValues())
            {
                yield return new[] { waste };
            }

            yield return new object[] { null };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
