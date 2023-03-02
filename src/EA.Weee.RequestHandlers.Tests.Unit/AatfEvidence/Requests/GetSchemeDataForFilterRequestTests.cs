namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GetSchemeDataForFilterRequestTests : SimpleUnitTestBase
    {
        [Fact]
        public void GetSchemeDataForFilterRequest_ShouldHaveSerializableAttribute()
        {
            typeof(GetSchemeDataForFilterRequest).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void GetSchemeDataForFilterRequest_ShouldDeriveFrom_EvidenceEntityIdDisplayNameDataBase()
        {
            typeof(GetSchemeDataForFilterRequest).Should().BeDerivedFrom<EvidenceEntityIdDisplayNameDataBase>();
        }

        [Fact]
        public void GetSchemeDataForFilterRequest_GivenNullStatusList_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => NewRequest(RecipientOrTransfer.Recipient,
                noteTypesList: new List<NoteType>()
                {
                    NoteType.Evidence
                }));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetSchemeDataForFilterRequest_GivenEmptyStatusList_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => NewRequest(RecipientOrTransfer.Recipient,
                noteStatusList: new List<NoteStatus>(),
                noteTypesList: new List<NoteType>()
                {
                    NoteType.Evidence
                }));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetSchemeDataForFilterRequest_GivenNullNoteTypesList_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => NewRequest(RecipientOrTransfer.Recipient,
                noteStatusList: new List<NoteStatus>()
                {
                    NoteStatus.Approved
                }));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetSchemeDataForFilterRequest_GivenEmptyNoteTypesList_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => NewRequest(RecipientOrTransfer.Recipient,
                noteStatusList: new List<NoteStatus>()
                {
                    NoteStatus.Approved
                },
                noteTypesList: new List<NoteType>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetSchemeDataForFilterRequest_GivenInvalidComplianceYear_ArgumentOutOfRangeExceptionExpected(int complianceYear)
        {
            //act
            var exception = Record.Exception(() => NewRequest(RecipientOrTransfer.Recipient,
                noteStatusList: new List<NoteStatus>()
                {
                    NoteStatus.Approved
                },
                noteTypesList: new List<NoteType>()
                {
                    NoteType.Evidence
                },
                complianceYear: complianceYear));

            //assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetSchemeDataForFilterRequest_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var recipientOrTransfer = TestFixture.Create<RecipientOrTransfer>();
            var aatfId = TestFixture.Create<Guid?>();
            var complianceYear = TestFixture.Create<int>();
            var noteStatusList = TestFixture.CreateMany<NoteStatus>().ToList();
            var noteTypesList = TestFixture.CreateMany<NoteType>().ToList();

            //act
            var result = NewRequest(recipientOrTransfer,
                aatfId,
                complianceYear,
                noteStatusList,
                noteTypesList);

            //assert
            result.RecipientOrTransfer.Should().Be(recipientOrTransfer);
            result.AatfId.Should().Be(aatfId);
            result.ComplianceYear.Should().Be(complianceYear);
            result.AllowedStatuses.Should().BeEquivalentTo(noteStatusList);
            result.AllowedNoteTypes.Should().BeEquivalentTo(noteTypesList);
        }

        private GetSchemeDataForFilterRequest NewRequest(RecipientOrTransfer recipientOrTransfer = RecipientOrTransfer.Recipient,
            Guid? aatfId = null, int complianceYear = 2022, List<NoteStatus> noteStatusList = null, List<NoteType> noteTypesList = null)
        {
            return new GetSchemeDataForFilterRequest(recipientOrTransfer,
                aatfId,
                complianceYear,
                noteStatusList,
                noteTypesList);
        }
    }
}
