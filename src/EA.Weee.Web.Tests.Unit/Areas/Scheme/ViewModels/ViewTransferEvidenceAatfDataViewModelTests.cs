namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class ViewTransferEvidenceAatfDataViewModelTests
    {
        private readonly Fixture fixture;

        public ViewTransferEvidenceAatfDataViewModelTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ViewTransferEvidenceAatfDataViewModel_ShouldHaveSerializableAttribute()
        {
            typeof(ViewTransferEvidenceAatfDataViewModel).Should().BeDecoratedWith<SerializableAttribute>();
        }
    }
}
