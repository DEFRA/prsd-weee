namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.ViewModels.Shared;
    using Xunit;

    public class ListOfSchemeNotesViewModelBaseTests
    {
        private readonly IMapper mapper;
        private readonly Fixture fixture;

        public ListOfSchemeNotesViewModelBaseTests()
        {
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();
        }

        [Fact]
        public void ListOfSchemeNotesViewModelBase_ShouldBeAbstract()
        {
            typeof(ListOfSchemeNotesViewModelBase<>).Should().BeAbstract();
        }

        [Fact]
        public void ListOfSchemeNotesViewModelBase_ShouldHaveISchemeManageEvidenceViewModelAsT()
        {
            typeof(ListOfSchemeNotesViewModelBase<>).GetGenericArguments()[0].GetGenericParameterConstraints()[0].Name
                .Should().Be(nameof(ISchemeManageEvidenceViewModel));
        }

        [Fact]
        public void ListOfSchemeNotesViewModelBase_ShouldHaveIManageEvidenceViewModelAsT()
        {
            typeof(ListOfSchemeNotesViewModelBase<>).GetGenericArguments()[0].GetGenericParameterConstraints()[1].Name
                .Should().Be(nameof(IManageEvidenceViewModel));
        }
    }
}
