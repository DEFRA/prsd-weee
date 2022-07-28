namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using System;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class TransferEvidenceViewModelBaseTests
    {
        [Fact]
        public void TransferEvidenceViewModelBase_ShouldHaveSerializableAttribute()
        {
            typeof(TransferEvidenceViewModelBase).Should().BeDecoratedWith<SerializableAttribute>();
        }
    }
}
