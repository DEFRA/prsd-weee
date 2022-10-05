namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using Api.Client;
    using AutoFixture;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Services;
    using Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Web.Areas.Aatf.Attributes;
    using Web.ViewModels.Shared;
    using Weee.Requests.Shared;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceNoteStartDateAttributeTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient client;
        private readonly IHttpContextService httpContextService;
        private readonly IWeeeCache cache;
        private readonly EvidenceNoteStartDateAttribute attribute;
        private readonly DateTime currentDate;
        private readonly Guid organisationId;
        private readonly Guid aatfId;
        private const string AatfApprovalError = "Aatf Approval Error";
        public EvidenceNoteStartDateAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            httpContextService = A.Fake<IHttpContextService>();
            cache = A.Fake<IWeeeCache>();

            organisationId = TestFixture.Create<Guid>();
            aatfId = TestFixture.Create<Guid>();
            attribute = new EvidenceNoteStartDateAttribute("EndDate", AatfApprovalError)
            {
                Client = () => client,
                HttpContextService = httpContextService,
                Cache = cache
            };

            currentDate = new DateTime(2020, 1, 1);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>()
            {
                new AatfData()
                {
                    ApprovalDate = new DateTime(2020, 1, 1),
                    ComplianceYear = (short)currentDate.Year,
                    Id = aatfId,
                    AatfId = aatfId
                }
            });
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_ShouldBeDecoratedWith_AttributeUsageAttribute()
        {
            typeof(EvidenceNoteStartDateAttribute).Should().BeDecoratedWith<AttributeUsageAttribute>().Which.ValidOn
                .Should().Be(AttributeTargets.Property);
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_CurrentDateShouldBeRetrievedFromApi()
        {
            //arrange
            var target = GetValidationDefaultTarget(currentDate, currentDate.AddDays(1));
            var context = new ValidationContext(target);

            var userToken = "token";
            A.CallTo(() => httpContextService.GetAccessToken()).Returns(userToken);

            //act
            attribute.Validate(target.StartDate, context);

            //assert
            A.CallTo(() => client.SendAsync(userToken, A<GetApiDate>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsAfterToday_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            SystemTime.Freeze(currentDate);
            var target = GetValidationDefaultTarget(currentDate.AddDays(1), currentDate.AddDays(2));
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("The start date cannot be in the future. Select today's date or earlier.");
            SystemTime.Unfreeze();
        }

        public static IEnumerable<object[]> ValidStartDates =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 1, 31), new DateTime(2020, 1, 31) },
                new object[] { new DateTime(2020, 1, 31), new DateTime(2020, 1, 1) },
                new object[] { new DateTime(2020, 1, 1), new DateTime(2019, 12, 31) },
            };

        [Theory]
        [MemberData(nameof(ValidStartDates))]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsInAllowedComplianceYear_ValidationExceptionShouldNotBeThrown(DateTime currentDate, DateTime startDate)
        {
            //arrange
            SystemTime.Freeze(currentDate);

            var target = GetValidationDefaultTarget(startDate, startDate.AddDays(1));
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>()
            {
                new AatfData()
                {
                    ApprovalDate = startDate,
                    ComplianceYear = (short)startDate.Year,
                    Id = aatfId,
                    AatfId = aatfId
                }
            });

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        public static IEnumerable<object[]> InValidStartDates =>
            new List<object[]>
            {
                new object[] { new DateTime(2019, 12, 31) },
                new object[] { new DateTime(2018, 12, 31) }
            };

        [Theory]
        [MemberData(nameof(InValidStartDates))]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsBeforeCurrentComplianceYear_ValidationExceptionShouldBeThrown(DateTime date)
        {
            //arrange
            var currentDate = new DateTime(2020, 12, 31);
            SystemTime.Freeze(currentDate);

            var target = GetValidationDefaultTarget(date, date.AddDays(1));
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should()
                .Be("The start date must be within an open compliance year");

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsAfterEndDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = GetValidationDefaultTarget(currentDate.AddDays(-1), currentDate.AddDays(-2));
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
        public void EvidenceNoteStartDateAttribute_GivenStartDateIsEqualToTheEndDate_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = GetValidationDefaultTarget(currentDate, currentDate);
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenEndDateIsEmpty_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = GetValidationDefaultTarget(currentDate, DateTime.MinValue);
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenEndDateIsNull_NoValidationExceptionShouldBeThrown()
        {
            //arrange
            var currentDate = new DateTime(2020, 2, 1);
            SystemTime.Freeze(currentDate);

            var target = GetValidationDefaultTarget(currentDate, null);
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();

            SystemTime.Unfreeze();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenModelIsNotBasedOnEvidenceNoteViewModel_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var target = new InvalidValidationTarget() { StartDate = currentDate, EndDate = currentDate };
            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("Unable to validate the evidence note details");
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenValidStartAndEndDates_AatfsForOrganisationShouldBeRetrievedFromCache()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var target = GetValidationDefaultTarget(currentDate, currentDate);
            target.OrganisationId = organisationId;

            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            
            //act
            attribute.Validate(target.StartDate, context);

            //assert
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenValidStartAndEndDatesAndNoAatfCouldBeFound_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var target = GetValidationDefaultTarget(currentDate, currentDate);
            target.OrganisationId = organisationId;
            target.AatfId = aatfId;

            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>());

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be("Aatf is invalid to save evidence notes.");
        }

        [Fact]
        public void EvidenceNoteStartDateAttribute_GivenValidStartAndEndDatesAndNoAatfCouldBeFoundWithApprovalDateBeforeEnteredDate_ValidationExceptionShouldBeThrown()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var groupedAatfId = TestFixture.Create<Guid>();
            var target = GetValidationDefaultTarget(currentDate, currentDate);
            target.OrganisationId = organisationId;
            target.AatfId = aatfId;

            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(currentDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>()
            {
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = groupedAatfId,
                    ComplianceYear = (short)currentDate.Year,
                    ApprovalDate = currentDate.AddDays(1)
                },
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = TestFixture.Create<Guid>(),
                    ComplianceYear = (short)currentDate.Year,
                    ApprovalDate = currentDate.AddDays(1)
                },
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = groupedAatfId,
                    ComplianceYear = (short)(currentDate.Year + 1),
                    ApprovalDate = currentDate
                },
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = groupedAatfId,
                    ComplianceYear = (short)(currentDate.Year - 1),
                    ApprovalDate = currentDate
                }
            });

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.ValidationResult.ErrorMessage.Should().Be(AatfApprovalError);
        }

        public static IEnumerable<object[]> ValidStartAndApprovalDates =>
            new List<object[]>
            {
                new object[] { new DateTime(2020, 1, 1), new DateTime(2020, 1, 1) },
                new object[] { new DateTime(2020, 1, 2), new DateTime(2020, 1, 1) },
            };

        [Theory]
        [MemberData(nameof(ValidStartAndApprovalDates))]
        public void EvidenceNoteStartDateAttribute_GivenValidStartAndEndDatesAndAatfCanBeFoundWithApprovalDateBeforeEnteredDate_NoValidationExceptionShouldBeThrown(DateTime startDate, DateTime approvalDate)
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var aatfId = TestFixture.Create<Guid>();
            var groupedAatfId = TestFixture.Create<Guid>();
            var target = GetValidationDefaultTarget(startDate, startDate);
            target.OrganisationId = organisationId;
            target.AatfId = aatfId;

            var context = new ValidationContext(target);

            A.CallTo(() => client.SendAsync(A<string>._, A<GetApiDate>._)).Returns(startDate);
            A.CallTo(() => cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(new List<AatfData>()
            {
                new AatfData()
                {
                    Id = aatfId,
                    AatfId = groupedAatfId,
                    ComplianceYear = (short)startDate.Year,
                    ApprovalDate = approvalDate
                }
            });

            //act
            var result = Record.Exception(() => attribute.Validate(target.StartDate, context)) as ValidationException;

            //assert
            result.Should().BeNull();
        }

        private ValidationTarget GetValidationDefaultTarget(DateTime startDate, DateTime? endDateTime)
        {
           return new ValidationTarget() { StartDate = startDate, EndDate = endDateTime, AatfId = aatfId, OrganisationId = organisationId };
        }

        private class ValidationTarget : EvidenceNoteViewModel
        {
            [EvidenceNoteStartDate(nameof(EndDate), AatfApprovalError)]
            public override DateTime? StartDate { get; set; }

            public override DateTime? EndDate { get; set; }
        }

        private class InvalidValidationTarget
        {
            [EvidenceNoteStartDate(nameof(EndDate), AatfApprovalError)]
            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }
    }
}
