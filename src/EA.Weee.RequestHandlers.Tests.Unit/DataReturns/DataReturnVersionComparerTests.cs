namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.DataReturns;
    using Xunit;

    public class DataReturnVersionComparerTests
    {
        [Fact]
        public void EeeDataChanged_ThrowsArgumentNullException_WhenCurrentSubmissionIsNull()
        {
            // Arrange
            var comparer = new DataReturnVersionComparer();

            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => comparer.EeeDataChanged(null, A.Dummy<DataReturnVersion>()));
        }

        [Fact]
        public void EeeDataChanged_ReturnsFalse_WhenPreviousSubmissionIsNull()
        {
            // Arrange
            var comparer = new DataReturnVersionComparer();
            var currentSubmission = A.Dummy<DataReturnVersion>();

            // Act
            var result = comparer.EeeDataChanged(currentSubmission, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EeeDataChanged_ReturnsFalse_WhenBothSubmissionsHaveNoEeeData()
        {
            // Arrange
            var comparer = new DataReturnVersionComparer();

            var currentSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => currentSubmission.EeeOutputReturnVersion)
                .Returns(null);

            var previousSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => previousSubmission.EeeOutputReturnVersion)
                .Returns(null);

            // Act
            var result = comparer.EeeDataChanged(currentSubmission, previousSubmission);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EeeDataChanged_ReturnsTrue_WhenCurrentSubmissionHasNoEeeData_ButPreviousSubmissionHadData()
        {
            // Arrange
            var comparer = new DataReturnVersionComparer();

            var currentSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => currentSubmission.EeeOutputReturnVersion)
                .Returns(null);

            var previousSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => previousSubmission.EeeOutputReturnVersion)
                .Returns(new EeeOutputReturnVersion());

            // Act
            var result = comparer.EeeDataChanged(currentSubmission, previousSubmission);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EeeDataChanged_ReturnsTrue_WhenCurrentSubmissionHasEeeData_ButPreviousSubmissionHadNoEeeData()
        {
            // Arrange
            var comparer = new DataReturnVersionComparer();

            var currentSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => currentSubmission.EeeOutputReturnVersion)
                .Returns(new EeeOutputReturnVersion());

            var previousSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => previousSubmission.EeeOutputReturnVersion)
                .Returns(null);

            // Act
            var result = comparer.EeeDataChanged(currentSubmission, previousSubmission);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EeeDataChanged_ReturnsFalse_WhenBothSubmissionsHaveSameEeeOutputReturnVersion()
        {
            // Arrange
            var comparer = new DataReturnVersionComparer();

            var eeeOutputReturnVersionId = Guid.NewGuid();

            var eeeOutputReturnVersion = A.Fake<EeeOutputReturnVersion>();
            A.CallTo(() => eeeOutputReturnVersion.Id)
                .Returns(eeeOutputReturnVersionId);

            var currentSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => currentSubmission.EeeOutputReturnVersion)
                .Returns(eeeOutputReturnVersion);

            var previousSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => previousSubmission.EeeOutputReturnVersion)
                .Returns(eeeOutputReturnVersion);

            // Act
            var result = comparer.EeeDataChanged(currentSubmission, previousSubmission);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EeeDataChanged_ReturnsTrue_WhenSubmissionsHaveDifferentEeeOutputReturnVersion()
        {
            // Arrange
            var comparer = new DataReturnVersionComparer();

            var eeeOutputReturnVersionId1 = Guid.NewGuid();

            var eeeOutputReturnVersion1 = A.Fake<EeeOutputReturnVersion>();
            A.CallTo(() => eeeOutputReturnVersion1.Id)
                .Returns(eeeOutputReturnVersionId1);

            var currentSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => currentSubmission.EeeOutputReturnVersion)
                .Returns(eeeOutputReturnVersion1);

            var eeeOutputReturnVersionId2 = Guid.NewGuid();

            var eeeOutputReturnVersion2 = A.Fake<EeeOutputReturnVersion>();
            A.CallTo(() => eeeOutputReturnVersion2.Id)
                .Returns(eeeOutputReturnVersionId2);

            var previousSubmission = A.Fake<DataReturnVersion>();
            A.CallTo(() => previousSubmission.EeeOutputReturnVersion)
                .Returns(eeeOutputReturnVersion2);

            // Act
            var result = comparer.EeeDataChanged(currentSubmission, previousSubmission);

            // Assert
            Assert.True(result);
        }
    }
}
