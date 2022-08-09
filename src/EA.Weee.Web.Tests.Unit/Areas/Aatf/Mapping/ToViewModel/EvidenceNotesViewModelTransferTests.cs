namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using FluentAssertions;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceNotesViewModelTransferTests : SimpleUnitTestBase
    {
        [Fact]
        public void Map_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.NewGuid(), 
                Guid.NewGuid(), 
                null, 
                SystemTime.Now, 
                null,
                TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.Empty, Guid.NewGuid(), TestFixture.Create<EvidenceNoteSearchDataResult>(), SystemTime.Now, null, TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Map_GivenEvidenceNoteIdGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceNotesViewModelTransfer(Guid.NewGuid(), Guid.Empty, TestFixture.Create<EvidenceNoteSearchDataResult>(), SystemTime.Now, null, TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }
    }
}
