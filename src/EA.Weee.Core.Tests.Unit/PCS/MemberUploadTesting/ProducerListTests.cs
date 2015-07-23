using EA.Weee.Core.PCS.MemberUploadTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EA.Weee.Core.Tests.Unit.PCS.MemberUploadTesting
{
    public class ProducerListTests
    {
        [Fact]
        public void ProducerListConstructor_WithNoParameters_ReturnsProducerList()
        {
            // Arrange

            // Act
            ProducerList producerList = new ProducerList();

            // Assert
            // No exception.
        }

        [Fact]
        public void ProducerListFactoryCreate_WithNullSettings_ThrowsArgumentNullException()
        {
            // Arrange
            ProducerListSettings listSettings = null;

            // Act
            Action action = () => ProducerList.Create(listSettings);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void ProducerListFactoryCreate_WithValidSettings_ReturnsProducerList()
        {
            // Arrange
            ProducerListSettings listSettings = new ProducerListSettings(SchemaVersion.Version_3_06, 1);

            // Act
            ProducerList producerList = ProducerList.Create(listSettings);

            // Assert
            // No exception.
        }
    }
}
