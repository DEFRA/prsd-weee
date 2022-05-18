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
    using Prsd.Core.Mapper;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ListOfNotesViewModelBaseTests
    {
        private readonly IMapper mapper;
        private readonly TestListBase testClass;
        private readonly Fixture fixture;

        public ListOfNotesViewModelBaseTests()
        {
            mapper = A.Fake<IMapper>();

            testClass = new TestListBase(mapper);
            fixture = new Fixture();
        }

        private class TestListBase : ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>
        {
            public TestListBase(IMapper mapper) : base(mapper)
            {
            }
        }

        [Fact]
        public void ListOfNotesViewModelBase_ShouldBeAbstract()
        {
            typeof(ListOfNotesViewModelBase<>).Should().BeAbstract();
        }

        [Fact]
        public void ListOfNotesViewModelBase_ShouldHaveIEvidenceNoteRowViewModelAsT()
        {
            typeof(ListOfNotesViewModelBase<>).GetGenericArguments()[0].GetGenericParameterConstraints()[0].Name
                .Should().Be(nameof(IManageEvidenceViewModel));
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => testClass.Map(null, fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                fixture.Create<EvidenceNoteData>(),
                fixture.Create<EvidenceNoteData>(),
                fixture.Create<EvidenceNoteData>()
            };

            //act
            testClass.Map(notes, fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>());

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                fixture.Create<EvidenceNoteRowViewModel>(),
                fixture.Create<EvidenceNoteRowViewModel>(),
                fixture.Create<EvidenceNoteRowViewModel>()
            };

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = testClass.Map(notes, fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>());

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }
    }
}
