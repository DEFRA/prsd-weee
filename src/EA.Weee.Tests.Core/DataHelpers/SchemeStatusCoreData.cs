namespace EA.Weee.Tests.Core.DataHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Evidence;
    using Weee.Core.Shared;

    public class SchemeStatusCoreData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var status in Enum.GetValues(typeof(SchemeStatus)))
            {
                yield return new object[] { status };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
