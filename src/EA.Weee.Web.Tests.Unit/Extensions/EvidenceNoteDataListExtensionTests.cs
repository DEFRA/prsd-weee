namespace EA.Weee.Web.Tests.Unit.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Organisations;
    using Core.Scheme;
    using FluentAssertions;
    using Web.Extensions;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceNoteDataListExtensionTests : SimpleUnitTestBase
    {
        [Fact]
        public void GivenEvidenceNoteList_SchemeOrganisationDataList_ShouldBeCreated()
        {
            //arrange
            var duplicateId = TestFixture.Create<Guid>();

            var note1 = TestFixture.Build<EvidenceNoteData>()
                .With(x => x.RecipientOrganisationData, 
                    TestFixture.Build<OrganisationData>()
                        .With(o => o.OrganisationName, "PBS")
                        .With(o => o.IsBalancingScheme, true)
                        .Create())
                .Create();
            var note2 = TestFixture.Build<EvidenceNoteData>()
                .With(x => x.RecipientSchemeData, 
                    TestFixture.Build<SchemeData>().With(s => s.SchemeName, "Z").Create())
                .With(x => x.RecipientOrganisationData,
                    TestFixture.Build<OrganisationData>()
                        .With(o => o.IsBalancingScheme, false)
                        .With(o => o.Id, duplicateId)
                        .Create())
                .Create();
            var note3 = TestFixture.Build<EvidenceNoteData>()
                .With(x => x.RecipientSchemeData,
                    TestFixture.Build<SchemeData>().With(s => s.SchemeName, "Z").Create())
                .With(x => x.RecipientOrganisationData,
                    TestFixture.Build<OrganisationData>()
                        .With(o => o.IsBalancingScheme, false)
                        .With(o => o.Id, duplicateId)
                        .Create())
                .Create();
            var note4 = TestFixture.Build<EvidenceNoteData>()
                .With(x => x.RecipientSchemeData,
                    TestFixture.Build<SchemeData>().With(s => s.SchemeName, "A").Create())
                .With(x => x.RecipientOrganisationData,
                    TestFixture.Build<OrganisationData>()
                        .With(o => o.IsBalancingScheme, false)
                        .Create())
                .Create();

            var notes = new List<EvidenceNoteData>() { note1, note2, note3, note4 };

            //act
            var result = notes.CreateOrganisationSchemeDataList();

            //assert
            result.Count.Should().Be(3);
            result.ElementAt(0).Id.Should().Be(note4.RecipientOrganisationData.Id);
            result.ElementAt(0).DisplayName.Should().Be(note4.RecipientSchemeData.SchemeName);
            result.ElementAt(1).Id.Should().Be(note1.RecipientOrganisationData.Id);
            result.ElementAt(1).DisplayName.Should().Be(note1.RecipientOrganisationData.OrganisationName);
            result.ElementAt(2).Id.Should().Be(note2.RecipientOrganisationData.Id);
            result.ElementAt(2).DisplayName.Should().Be(note2.RecipientSchemeData.SchemeName);
        }
    }
}
