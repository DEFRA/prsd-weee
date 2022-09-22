﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Scheme;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FluentAssertions;
    using Prsd.Core;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class SchemeTabViewModelMapTransferTests : SimpleUnitTestBase
    {
        private const int PageNumber = 2;
        private const int PageSize = 3;

        [Fact]
        public void SchemeTabViewModelMapTransfer_GiveListOfNotesIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new SchemeTabViewModelMapTransfer(Guid.NewGuid(),
                null,
                TestFixture.Create<SchemePublicInfo>(),
                SystemTime.Now,
                TestFixture.Create<int>(),
                PageNumber,
                PageSize));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void SchemeTabViewModelMapTransfer_GivenOrganisationGuidIsEmpty_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new SchemeTabViewModelMapTransfer(Guid.Empty,
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                TestFixture.Create<SchemePublicInfo>(),
                SystemTime.Now,
                TestFixture.Create<int>(),
                PageNumber,
                PageSize));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SchemeTabViewModelMapTransfer_Constructor_GivenPageNumberIsLessThanOne_ShouldThrowAnException(int pageNumber)
        {
            //act
            var exception = Record.Exception(() => new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                TestFixture.Create<SchemePublicInfo>(),
                SystemTime.Now,
                TestFixture.Create<int>(),
                pageNumber,
                PageSize));

            // assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SchemeTabViewModelMapTransfer_Constructor_GivenPageSizeIsLessThanOne_ShouldThrowAnException(int pageSize)
        {
            //act
            var exception = Record.Exception(() => new SchemeTabViewModelMapTransfer(TestFixture.Create<Guid>(),
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                TestFixture.Create<SchemePublicInfo>(),
                SystemTime.Now,
                TestFixture.Create<int>(),
                PageNumber,
                pageSize));

            // assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void SchemeTabViewModelMapTransfer_Constructor_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYear = TestFixture.Create<int>();
            var schemeInfo = TestFixture.Create<SchemePublicInfo>();

            //act
            var mapper = new SchemeTabViewModelMapTransfer(organisationId, noteData, schemeInfo, currentDate, complianceYear, PageNumber, PageSize);

            //assert
            mapper.Scheme.Should().Be(schemeInfo);
            mapper.CurrentDate.Should().Be(currentDate);
            mapper.SelectedComplianceYear.Should().Be(complianceYear);
            mapper.NoteData.Should().Be(noteData);
            mapper.PageNumber.Should().Be(PageNumber);
            mapper.PageSize.Should().Be(PageSize);
        }
    }
}
