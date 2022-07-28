namespace EA.Weee.Tests.Core.DataHelpers
{
    using System.Collections;
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Scheme;

    public class SchemeStatusData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var status in Enumeration.GetAll<SchemeStatus>())
            {
                yield return new object[] { status };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
