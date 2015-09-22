namespace EA.Weee.Web.Tests.Unit.App_Start
{
    using EA.Weee.Web.App_Start;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ReturnUrlMappingTests
    {
        /// <summary>
        /// This test ensures that the IsMapped returns false for a path that is not mapped.
        /// </summary>
        [Fact]
        public void IsMapped_WithNoMapping_ReturnsFalse()
        {
            // Arrange
            ReturnUrlMapping mapping = new ReturnUrlMapping("/");

            // Act
            bool result = mapping.IsMapped("/controller1/action1");

            // Assert
            Assert.Equal(false, result);
        }

        /// <summary>
        /// This test ensures that the IsMapped returns true for a path that is mapped.
        /// </summary>
        [Fact]
        public void IsMapped_WithMapping_ReturnsTrue()
        {
            // Arrange
            ReturnUrlMapping mapping = new ReturnUrlMapping("/");
            mapping.Add("/controller1/action1", "/controller2/action2");

            // Act
            bool result = mapping.IsMapped("/controller1/action1");

            // Assert
            Assert.Equal(true, result);
        }

        /// <summary>
        /// This test ensures that the IsMapped returns true for a path that is mapped
        /// and that the mapping correctly translates the path when an application
        /// virtual path is being used.
        /// </summary>
        [Fact]
        public void IsMapped_WithMappingAndApplicationVirtualPath_ReturnsTrue()
        {
            // Arrange
            ReturnUrlMapping mapping = new ReturnUrlMapping("/weee");
            mapping.Add("/controller1/action1", "/controller2/action2");

            // Act
            bool result = mapping.IsMapped("/weee/controller1/action1");

            // Assert
            Assert.Equal(true, result);
        }

        /// <summary>
        /// This test ensures that the ApplyMap returns the original path
        /// for a path that is not mapped.
        /// </summary>
        [Fact]
        public void ApplyMap_WithNoMapping_ReturnsPathUnchanged()
        {
            // Arrange
            ReturnUrlMapping mapping = new ReturnUrlMapping("/");

            // Act
            string result = mapping.ApplyMap("/controller1/action1");

            // Assert
            Assert.Equal("/controller1/action1", result);
        }

        /// <summary>
        /// This test ensures that the ApplyMap returns the mapped path
        /// for a path that is mapped.
        /// </summary>
        [Fact]
        public void ApplyMap_WithMapping_ReturnsMappedPath()
        {
            // Arrange
            ReturnUrlMapping mapping = new ReturnUrlMapping("/");
            mapping.Add("/controller1/action1", "/controller2/action2");

            // Act
            string result = mapping.ApplyMap("/controller1/action1");

            // Assert
            Assert.Equal("/controller2/action2", result);
        }

        /// <summary>
        /// This test ensures that the ApplyMap returns the mapped path
        /// for a path that is mapped and that the mapping correctly
        /// translates the path when an application virtual path is being used.
        /// </summary>
        [Fact]
        public void ApplyMap_WithMappingAndApplicationVirtualPath_ReturnsMappedPath()
        {
            // Arrange
            ReturnUrlMapping mapping = new ReturnUrlMapping("/weee");
            mapping.Add("/controller1/action1", "/controller2/action2");

            // Act
            string result = mapping.ApplyMap("/weee/controller1/action1");

            // Assert
            Assert.Equal("/weee/controller2/action2", result);
        }
    }
}
