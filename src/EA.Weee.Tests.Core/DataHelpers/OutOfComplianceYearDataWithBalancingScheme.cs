namespace EA.Weee.Tests.Core.DataHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class OutOfComplianceYearDataWithBalancingScheme : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new DateTime(2020, 2, 1), 2019, true };
            yield return new object[] { new DateTime(2020, 1, 1), 2022, false };
            yield return new object[] { new DateTime(2021, 2, 1), 2019, false };
            yield return new object[] { new DateTime(2021, 1, 1), 2022, true };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
