namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfEvidence;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class RequiredSubmitActionAttributeTests
    {
        private readonly List<ValidationResult> validationResults;

        public RequiredSubmitActionAttributeTests()
        {
            validationResults = new List<ValidationResult>();
        }

        [Fact]
        public void RequiredSubmitActionAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(RequiredSubmitActionAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void RequiredSubmitActionAttribute_ShouldBeDerivedFrom_RequiredAttribute()
        {
            typeof(RequiredSubmitActionAttribute).Should().BeDerivedFrom<RequiredAttribute>();
        }

        [Fact]
        public void
            RequiredSubmitActionAttribute_GivenNotSubmitActionPropertyRequiredSubmitActionPropertiesDoNotHaveValue_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = new EvidenceNoteViewModel()
            {
                WasteTypeValue = null,
                ProtocolValue = null,
                ReceivedId = Guid.NewGuid(),
                StartDate = date,
                EndDate = date,
                Action = ActionEnum.Save
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();
            validationResults.Should().BeEmpty();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void
            RequiredSubmitActionAttribute_GivenSubmitActionAndRequiredSubmitActionPropertiesDoNotHaveValue_ValidationFailShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = new EvidenceNoteViewModel()
            {
                WasteTypeValue = null,
                ProtocolValue = null,
                ReceivedId = Guid.NewGuid(),
                StartDate = date,
                EndDate = date,
                Action = ActionEnum.Submit
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeFalse();

            validationResults.Select(v => v.ErrorMessage).Should().BeEquivalentTo(new List<string>()
            {
                "Select a type of waste",
                "Select actual or protocol",
            });

            SystemTime.Unfreeze();
        }

        [Fact]
        public void
            RequiredSubmitActionAttribute_GivenSubmitActionAndRequiredSubmitActionPropertiesDoHaveValue_ValidationSuccessShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 3, 1);
            SystemTime.Freeze(date);

            var target = new EvidenceNoteViewModel()
            {
                WasteTypeValue = WasteType.Household,
                ProtocolValue = Protocol.SiteSpecificProtocol,
                ReceivedId = Guid.NewGuid(),
                StartDate = date,
                EndDate = date,
                Action = ActionEnum.Submit
            };

            var context = new ValidationContext(target);

            var result = Validator.TryValidateObject(target, context, validationResults, true);

            //assert
            result.Should().BeTrue();

            validationResults.Should().BeEmpty();

            SystemTime.Unfreeze();
        }

        private class NotValidValidationTarget
        {
            [RequiredSubmitAction]
            public DateTime? StartDate { get; set; }
        }
    }
}
