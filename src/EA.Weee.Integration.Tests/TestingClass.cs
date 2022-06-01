namespace EA.Weee.Integration.Tests
{
    using System;

    public static class TestingStatus
    {
        public static volatile bool IsDbReseeded = false;
        public static volatile bool IsDbSeedingFaulted = false;
        public static volatile bool IsDbBuilt = false;
    }
}
