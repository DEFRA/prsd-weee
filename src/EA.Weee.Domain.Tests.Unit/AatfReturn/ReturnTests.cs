﻿namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using System.Runtime.InteropServices;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Xunit;

    public class ReturnTests
    {
        [Fact]
        public void Return_GivenOrganisationIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(null, A.Dummy<Quarter>(), A.Dummy<string>(), A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_QuarterIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Organisation>(), null, A.Dummy<string>(), A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Return_CreatedByIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Organisation>(), A.Dummy<Quarter>(), null, A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        public void Return_CreatedByIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Return(A.Dummy<Organisation>(), A.Dummy<Quarter>(), value, A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Return_GivenValidParameters_ReturnPropertiesShouldBeSet()
        {
            var organisation = A.Fake<Organisation>();
            var quarter = A.Fake<Quarter>();
            var userId = "user";
            FacilityType facilityType = FacilityType.Aatf;

            SystemTime.Freeze(new DateTime(2019, 05, 2));
            var @return = new Return(organisation, quarter, userId, facilityType);
            SystemTime.Unfreeze();

            @return.Organisation.Should().Be(organisation);
            @return.Quarter.Should().Be(quarter);
            @return.ReturnStatus.Should().Be(ReturnStatus.Created);
            @return.CreatedById.Should().Be(userId);
            @return.CreatedDate.Date.Should().Be(new DateTime(2019, 05, 2));
            @return.SubmittedDate.Should().BeNull();
            @return.SubmittedById.Should().BeNull();
            @return.FacilityType.Should().Be(facilityType);
        }

        [Theory]
        [InlineData("")]
        public void UpdateSubmitted_GivenSubmittedByIsEmpty_ThrowsArgumentException(string value)
        {
            var @return = new Return(A.Dummy<Organisation>(), A.Dummy<Quarter>(), "me", A.Fake<FacilityType>());

            Action update = () =>
            {
                @return.UpdateSubmitted(value);
            };

            update.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateSubmitted_GivenSubmittedByIsNull_ThrowsArgumentNullException()
        {
            var @return = new Return(A.Dummy<Organisation>(), A.Dummy<Quarter>(), "me", A.Dummy<FacilityType>());

            Action update = () =>
            {
                @return.UpdateSubmitted(null);
            };

            update.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateSubmitted_GivenSubmittedBy_ReturnSubmittedPropertiesShouldBeSet()
        {
            var @return = new Return(A.Dummy<Organisation>(), A.Dummy<Quarter>(), "me", A.Dummy<FacilityType>());

            SystemTime.Freeze(new DateTime(2019, 05, 2));
            @return.UpdateSubmitted("me2");
            SystemTime.Unfreeze();

            @return.SubmittedDate.Value.Date.Should().Be(new DateTime(2019, 05, 2));
            @return.SubmittedById.Should().Be("me2");
            @return.ReturnStatus.Should().Be(ReturnStatus.Submitted);
        }

        [Fact]
        public void UpdateSubmitted_GivenReturnIsNotCreatedStatus_InvalidOperationExceptionExpected()
        {
            var @return = new Return(A.Dummy<Organisation>(), A.Dummy<Quarter>(), "me", A.Fake<FacilityType>()) { ReturnStatus = ReturnStatus.Submitted };

            Action update = () =>
            {
                @return.UpdateSubmitted("me2");
            };

            update.Should().Throw<InvalidOperationException>();
        }
    }
}
