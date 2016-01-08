namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using Domain.Scheme;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class MemberUploadTests
    {
        [Fact]
        public void Submit_WhenNotYetSubmitted_MarksMemberUploadAsSubmitted()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            // Act
            memberUpload.Submit(A.Dummy<User>());

            // Assert
            Assert.True(memberUpload.IsSubmitted);
        }

        [Fact]
        public void Submit_WhenAlreadySubmitted_ThrowInvalidOperationException()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            memberUpload.Submit(A.Dummy<User>());

            // Act
            Action action = () => memberUpload.Submit(A.Dummy<User>());

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void SetProcessTime_WhenCurrentValueIsZero_SetProcessTime()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            // Act
            memberUpload.SetProcessTime(TimeSpan.FromSeconds(15));

            // Assert
            Assert.Equal(TimeSpan.FromSeconds(15), memberUpload.ProcessTime);
        }

        [Fact]
        public void SetProcessTime_WhenCurrentValueIsNotZero_ThrowInvalidOperationException()
        {
            // Arrange
            var memberUpload = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                A.Dummy<int>(),
                A.Dummy<int>(),
                A.Dummy<Scheme>(),
                A.Dummy<string>());

            memberUpload.SetProcessTime(TimeSpan.FromSeconds(15));

            // Act
            Action action = () => memberUpload.SetProcessTime(TimeSpan.FromSeconds(25));

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }
    }
}
