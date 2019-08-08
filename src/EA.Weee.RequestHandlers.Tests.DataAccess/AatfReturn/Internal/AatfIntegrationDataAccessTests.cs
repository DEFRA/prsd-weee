namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfIntegrationDataAccessTests
    {
        private readonly Fixture fixture;

        public AatfIntegrationDataAccessTests()
        {
            fixture = new Fixture();
        }
    }
}
