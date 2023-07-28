namespace EA.Weee.Tests.Core
{
    using System.Globalization;
    using System.Threading;
    using AutoFixture;

    public class SimpleUnitTestBase
    {
        protected readonly Fixture TestFixture;

        public SimpleUnitTestBase()
        {
            CultureInfo ci = new CultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            TestFixture = new Fixture();
        }
    }
}
