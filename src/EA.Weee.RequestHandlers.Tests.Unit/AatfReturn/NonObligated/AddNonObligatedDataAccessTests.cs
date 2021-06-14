namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using DataAccess;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.NonObligated;
    using System.Collections.Generic;
    using Weee.Tests.Core;
    using Xunit;

    public class NonObligatedDataAccessTests
    {
        private readonly NonObligatedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;

        public NonObligatedDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new NonObligatedDataAccess(context);
        }
    }
}
