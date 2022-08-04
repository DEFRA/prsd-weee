namespace EA.Weee.Tests.Core.DataHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class OutOfComplianceYearData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new DateTime(2020, 2, 1), 2019 };
            yield return new object[] { new DateTime(2020, 1, 1), 2022 };
            yield return new object[] { new DateTime(2021, 2, 1), 2020 };
            yield return new object[] { new DateTime(2021, 1, 1), 2022 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
