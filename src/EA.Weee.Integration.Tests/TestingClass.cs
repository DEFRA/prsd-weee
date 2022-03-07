using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Integration.Tests
{
    public static class TestingStatus
    {
        public static volatile bool IsDbReseeded = false;
        public static volatile bool IsDbSeedingFaulted = false;
    }

    public class DatabaseSeedingFailureException : Exception
    {
    }
}
