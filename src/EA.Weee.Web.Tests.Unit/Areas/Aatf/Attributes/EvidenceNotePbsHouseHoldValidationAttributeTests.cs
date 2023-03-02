namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using AutoFixture;
    using Core.AatfEvidence;
    using FakeItEasy;
    using FluentAssertions;
    using Services.Caching;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Scheme;
    using Web.Areas.Aatf.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceNotePbsHouseHoldValidationAttributeTests : SimpleUnitTestBase
    {
        private readonly IWeeeCache cache;
        private readonly EvidenceNotePbsHouseHoldValidationAttribute attribute;
        private const string Error = "PBS non household selection error";
        
        public EvidenceNotePbsHouseHoldValidationAttributeTests()
        {
            cache = A.Fake<IWeeeCache>();

            attribute = new EvidenceNotePbsHouseHoldValidationAttribute("RecipientId")
            {
                Cache = cache,
                ErrorMessage = Error
            };
        }

        [Fact]
        public void EvidenceNotePbsHouseHoldValidationAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(EvidenceNotePbsHouseHoldValidationAttribute)
                .Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn.Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void IsValid_GivenRecipientIdIsNull_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var target = GetValidationDefaultTarget(null, null);
            var context = new ValidationContext(target);

            //act
            var result = attribute.GetValidationResult(target.Obligation, context);

            //assert
            result.Should().Be(ValidationResult.Success);
        }

        [Fact]
        public void IsValid_GivenObligationIsNull_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var target = GetValidationDefaultTarget(null, null);
            var context = new ValidationContext(target);

            //act
            var result = attribute.GetValidationResult(target.Obligation, context);

            //assert
            result.Should().Be(ValidationResult.Success);
        }

        [Fact]
        public void IsValid_GivenRecipientIdAndObligation_SchemeByOrganisationShouldBeRetrievedFromCache()
        {
            //arrange
            var recipientId = TestFixture.Create<Guid>();
            var obligation = TestFixture.Create<WasteType>();

            var target = GetValidationDefaultTarget(recipientId, obligation);
            var context = new ValidationContext(target);

            //act
            attribute.GetValidationResult(target.Obligation, context);

            //assert
            A.CallTo(() => cache.FetchSchemePublicInfo(recipientId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void IsValid_GivenRecipientIsPbsAndHouseHoldSelectedAsObligation_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var recipientId = TestFixture.Create<Guid>();
            var obligation = WasteType.Household;
            var schemeInfo = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.IsBalancingScheme, true)
                .Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(A<Guid>._)).Returns(schemeInfo);

            var target = GetValidationDefaultTarget(recipientId, obligation);
            var context = new ValidationContext(target);

            //act
            var result = attribute.GetValidationResult(target.Obligation, context);

            //assert
            result.Should().Be(ValidationResult.Success);
        }

        [Fact]
        public void IsValid_GivenRecipientIsPbsAndNonHouseHoldSelectedAsObligation_ValidationErrorShouldBeReturned()
        {
            //arrange
            var recipientId = TestFixture.Create<Guid>();
            var obligation = WasteType.NonHousehold;
            var schemeInfo = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.IsBalancingScheme, true)
                .Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(A<Guid>._)).Returns(schemeInfo);

            var target = GetValidationDefaultTarget(recipientId, obligation);
            var context = new ValidationContext(target);

            //act
            var result = attribute.GetValidationResult(target.Obligation, context);

            //assert
            result.ErrorMessage.Should().Be(Error);
        }

        [Theory]
        [InlineData(WasteType.NonHousehold)]
        [InlineData(WasteType.Household)]
        public void IsValid_GivenRecipientIsNotPbsAndObligationSelected_ValidationSuccessShouldBeReturned(WasteType wasteType)
        {
            //arrange
            var recipientId = TestFixture.Create<Guid>();
            var obligation = wasteType;
            var schemeInfo = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.IsBalancingScheme, false)
                .Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(A<Guid>._)).Returns(schemeInfo);

            var target = GetValidationDefaultTarget(recipientId, obligation);
            var context = new ValidationContext(target);

            //act
            var result = attribute.GetValidationResult(target.Obligation, context);

            //assert
            result.Should().Be(ValidationResult.Success);
        }

        private ValidationTarget GetValidationDefaultTarget(Guid? recipientId, WasteType? obligation)
        {
            return new ValidationTarget() { RecipientId = recipientId, Obligation = obligation };
        }

        private class ValidationTarget
        {
            public Guid? RecipientId { get; set; }

            [EvidenceNotePbsHouseHoldValidationAttribute(nameof(RecipientId))]
            public WasteType? Obligation { get; set; }
        }
    }
}
