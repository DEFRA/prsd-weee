namespace EA.Weee.Tests.Core.DataHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Weee.Core.Shared;

    public class UserStatusCoreData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var status in Enum.GetValues(typeof(UserStatus)))
            {
                yield return new object[] { status };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
