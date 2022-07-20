namespace EA.Weee.Tests.Core.DataHelpers
{
    using System.Collections;
    using System.Collections.Generic;
    using EA.Weee.Domain.Evidence;

    public class ProtocolData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var protocol in typeof(Protocol).GetEnumValues())
            {
                yield return new[] { protocol };
            }

            yield return new object[] { null };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
