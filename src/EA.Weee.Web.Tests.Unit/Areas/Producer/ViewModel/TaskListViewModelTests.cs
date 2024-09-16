namespace EA.Weee.Web.Tests.Unit.Areas.Producer.ViewModel
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class TaskListViewModelTests
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void CheckAnswersEnabled_AllTasksComplete_ShouldBeTrue()
        {
            // Arrange
            var viewModel = fixture.Build<TaskListViewModel>()
                .With(vm => vm.ProducerTaskModels, new List<ProducerTaskModel>
                {
                    new ProducerTaskModel { Complete = true },
                    new ProducerTaskModel { Complete = true },
                    new ProducerTaskModel { Complete = true }
                })
                .Create();

            // Act & Assert
            viewModel.CheckAnswersEnabled.Should().BeTrue();
        }

        [Fact]
        public void CheckAnswersEnabled_OneTaskIncomplete_ShouldBeFalse()
        {
            // Arrange
            var viewModel = fixture.Build<TaskListViewModel>()
                .With(vm => vm.ProducerTaskModels, new List<ProducerTaskModel>
                {
                    new ProducerTaskModel { Complete = true },
                    new ProducerTaskModel { Complete = false },
                    new ProducerTaskModel { Complete = true }
                })
                .Create();

            // Act & Assert
            viewModel.CheckAnswersEnabled.Should().BeFalse();
        }

        [Fact]
        public void CheckAnswersEnabled_AllTasksIncomplete_ShouldBeFalse()
        {
            // Arrange
            var viewModel = fixture.Build<TaskListViewModel>()
                .With(vm => vm.ProducerTaskModels, new List<ProducerTaskModel>
                {
                    new ProducerTaskModel { Complete = false },
                    new ProducerTaskModel { Complete = false },
                    new ProducerTaskModel { Complete = false }
                })
                .Create();

            // Act & Assert
            viewModel.CheckAnswersEnabled.Should().BeFalse();
        }

        [Fact]
        public void CheckAnswersEnabled_EmptyTaskList_ShouldBeTrue()
        {
            // Arrange
            var viewModel = fixture.Build<TaskListViewModel>()
                .With(vm => vm.ProducerTaskModels, new List<ProducerTaskModel>())
                .Create();

            // Act & Assert
            viewModel.CheckAnswersEnabled.Should().BeTrue();
        }

        [Fact]
        public void CheckAnswersEnabled_NullTaskList_ShouldThrowNullReferenceException()
        {
            // Arrange
            var viewModel = fixture.Build<TaskListViewModel>()
                .Without(vm => vm.ProducerTaskModels)
                .Create();

            // Act & Assert
            viewModel.Invoking(vm => vm.CheckAnswersEnabled)
                .Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void OrganisationId_ShouldSetAndGetCorrectValue()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            var viewModel = fixture.Build<TaskListViewModel>()
                .With(vm => vm.OrganisationId, expectedGuid)
                .Create();

            // Act & Assert
            viewModel.OrganisationId.Should().Be(expectedGuid);
        }

        [Fact]
        public void ProducerTaskModels_ShouldAllowAddingAndRetrievingTasks()
        {
            // Arrange
            var viewModel = fixture.Create<TaskListViewModel>();
            var newTask = fixture.Create<ProducerTaskModel>();

            // Act
            viewModel.ProducerTaskModels.Add(newTask);

            // Assert
            viewModel.ProducerTaskModels.Should().Contain(newTask);
        }
    }
}