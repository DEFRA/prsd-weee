namespace EA.Weee.Tests.Core
{
    using AutoFixture;

    public class SimpleUnitTestBase
    {
        protected readonly Fixture TestFixture;

        public SimpleUnitTestBase()
        {
            TestFixture = new Fixture();
        }
    }
}
