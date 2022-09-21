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
    using Web.ViewModels.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class ListOfNotesViewModelBaseTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly TestListBase testClass;

        public ListOfNotesViewModelBaseTests()
        {
            mapper = A.Fake<IMapper>();
            
            testClass = new TestListBase(mapper);
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
        public void ListOfNotesViewModelBase_ShouldHaveIManageEvidenceViewModelAsT()
        {
            typeof(ListOfNotesViewModelBase<>).GetGenericArguments()[0].GetGenericParameterConstraints()[0].Name
                .Should().Be(nameof(IManageEvidenceViewModel));
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => testClass.MapBase(null, 1, 2));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };
            var noteData = new EvidenceNoteSearchDataResult(notes, 2);

            //act
            testClass.MapBase(noteData, 1, 2);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e => 
                e.SequenceEqual(noteData.Results.ToList())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = testClass.MapBase(noteData, 1, 3);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }
    }
}
