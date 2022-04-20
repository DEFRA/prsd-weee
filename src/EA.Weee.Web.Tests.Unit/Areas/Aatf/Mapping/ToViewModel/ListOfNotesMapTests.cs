namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ListOfNotesMapTests
    {
        private readonly Fixture fixture;
        private readonly IMapper mapper;

        private ListOfNotesMap map;

        public ListOfNotesMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new ListOfNotesMap(mapper);
            fixture = new Fixture();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            var exception = Record.Exception(() => map.Map(null));

            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListSource_EvidenceNoteDataShouldBeMapped()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();

            //act
            map.Map(notes);

            // assert
            foreach (var evidenceNoteData in notes)
            {
                A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(evidenceNoteData)).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public void Map_GivenListSource_EvidenceNoteRowViewModelShouldBeReturned()
        {
            //arrange
            var notes = new List<EvidenceNoteData>()
            {
                fixture.Create<EvidenceNoteData>(),
                fixture.Create<EvidenceNoteData>()
            };

            var models = new List<EvidenceNoteRowViewModel>()
            {
                fixture.Create<EvidenceNoteRowViewModel>(),
                fixture.Create<EvidenceNoteRowViewModel>()
            }.ToList();

            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(notes.ElementAt(0))).Returns(models.ElementAt(0));
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(notes.ElementAt(1))).Returns(models.ElementAt(1));

            //act
            var result = map.Map(notes);

            // assert
            result.Should().BeEquivalentTo(models);
        }
    }
}
