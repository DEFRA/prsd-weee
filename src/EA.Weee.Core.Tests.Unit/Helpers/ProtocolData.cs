namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System.Collections;
    using System.Collections.Generic;
    using Domain.Evidence;
    using Prsd.Core.Domain;

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
