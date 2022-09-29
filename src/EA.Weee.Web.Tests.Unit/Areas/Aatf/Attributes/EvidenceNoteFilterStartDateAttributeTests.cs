namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Api.Client;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Services;
    using Web.Areas.Aatf.Attributes;
    using Weee.Requests.Shared;
    using Xunit;

    public class EvidenceNoteFilterStartDateAttributeTests
    {
        private readonly IWeeeClient client;
        private readonly IHttpContextService httpContextService;
        private readonly EvidenceNoteFilterStartDateAttribute attribute;
        private readonly DateTime currentDate;

        public EvidenceNoteFilterStartDateAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            httpContextService = A.Fake<IHttpContextService>();

            attribute = new EvidenceNoteFilterStartDateAttribute("EndDate")
            {
                Client = () => client,
                HttpContextService = httpContextService
            };

            currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
        }

        [Fact]
        public void EvidenceNoteFilterStartDateAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(EvidenceNoteFilterStartDateAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void EvidenceNoteFilterStartDateAttribute_CurrentDateShouldBeRetrievedFromCache()
        {
            //arrange
            var target = new ValidationTarget() { StartDate = currentDate, EndDate = currentDate.AddDays(1) };
            var context = new ValidationContext(target);

            var userToken = "token";
            A.CallTo(() => httpContextService.GetAccessToken()).Returns(userToken);

            //act
            attribute.Validate(target.StartDate, context);

            //assert
            A.CallTo(() => client.SendAsync(userToken, A<GetApiDate>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteFilterStartDateAttribute_GivenStartDateIsAfterToday_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            SystemTime.Freeze(currentDate);
            var target = new ValidationTarget() {StartDate = currentDate.AddDays(1), EndDate = currentDate.AddDays(2) };
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("The start date cannot be in the future. Select today's date or earlier.");
            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteFilterStartDateAttribute_GivenStartDateIsAfterEndDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate.AddDays(-1), EndDate = currentDate.AddDays(-2) };
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("Ensure the start date is before the end date");

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteFilterStartDateAttribute_GivenStartDateIsEqualToTheEndDate_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = currentDate };
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteFilterStartDateAttribute_GivenEndDateIsEmpty_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = DateTime.MinValue };
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteFilterStartDateAttribute_GivenEndDateIsNull_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = new ValidationTarget() { StartDate = currentDate, EndDate = null };
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        private class ValidationTarget
        {
            [EvidenceNoteFilterStartDate(nameof(EndDate))]
            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }
    }
}
