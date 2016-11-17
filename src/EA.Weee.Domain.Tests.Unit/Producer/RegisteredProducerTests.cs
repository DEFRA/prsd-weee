namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using System;
    using Domain.Producer;
    using FakeItEasy;
    using Prsd.Core;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class RegisteredProducerTests
    {
        [Fact]
        public void RegisteredProducer_EqualsNullParameter_ReturnsFalse()
        {
            var producer = RegisteredProducerBuilder.NewRegisteredProducer;

            Assert.NotEqual(producer, null);
        }

        [Fact]
        public void RegisteredProducer_EqualsObjectParameter_ReturnsFalse()
        {
            var producer = RegisteredProducerBuilder.NewRegisteredProducer;

            Assert.NotEqual(producer, new object());
        }

        [Fact]
        public void RegisteredProducer_EqualsSameInstance_ReturnsTrue()
        {
            var producer = RegisteredProducerBuilder.NewRegisteredProducer;

            Assert.Equal(producer, producer);
        }

        [Fact]
        public void RegisteredProducer_EqualsRegisteredProducerSameDetails_ReturnsTrue()
        {
            var producer = RegisteredProducerBuilder.NewRegisteredProducer;
            var producer2 = RegisteredProducerBuilder.NewRegisteredProducer;

            Assert.Equal(producer, producer2);
        }

        [Fact]
        public void RegisteredProducer_EqualsRegisteredProducerDifferentComplianceYear_ReturnsFalse()
        {
            var producer = RegisteredProducerBuilder.WithComplianceYear(2015);
            var producer2 = RegisteredProducerBuilder.WithComplianceYear(2016);

            Assert.NotEqual(producer, producer2);
        }

        [Fact]
        public void RegisteredProducer_EqualsRegisteredProducerDifferentPrn_ReturnsFalse()
        {
            var producer = RegisteredProducerBuilder.WithProducerRegistrationNumber("AAAA");
            var producer2 = RegisteredProducerBuilder.WithProducerRegistrationNumber("BBBB");

            Assert.NotEqual(producer, producer2);
        }

        [Fact]
        public void RegisteredProducer_EqualsRegisteredProducerDifferentSchemeApprovalNumber_ReturnsFalse()
        {
            var producer = RegisteredProducerBuilder.WithSchemeApprovalNumber("AAAA");
            var producer2 = RegisteredProducerBuilder.WithSchemeApprovalNumber("BBBB");

            Assert.NotEqual(producer, producer2);
        }

        [Fact]
        public void RegisteredProducer_EqualsRemovedRegisteredProducer_ReturnsFalse()
        {
            var producer = RegisteredProducerBuilder.NewRegisteredProducer;
            var producer2 = RegisteredProducerBuilder.NewRegisteredProducer;
            producer2.Remove();

            Assert.NotEqual(producer, producer2);
        }

        [Fact]
        public void RegisteredProducer_EqualsRemovedRegisteredProducerWithSameRemovedDate_ReturnsTrue()
        {
            var producer = RegisteredProducerBuilder.NewRegisteredProducer;
            var producer2 = RegisteredProducerBuilder.NewRegisteredProducer;

            SystemTime.Freeze(new DateTime(2016, 1, 1));
            producer.Remove();
            producer2.Remove();

            SystemTime.Unfreeze();

            Assert.Equal(producer, producer2);
        }

        [Fact]
        public void RegisteredProducer_EqualsRemovedRegisteredProducerWithDifferentRemovedDate_ReturnsFalse()
        {
            var producer = RegisteredProducerBuilder.NewRegisteredProducer;
            var producer2 = RegisteredProducerBuilder.NewRegisteredProducer;

            SystemTime.Freeze(new DateTime(2016, 1, 1));
            producer.Remove();

            SystemTime.Freeze(new DateTime(2016, 10, 1));
            producer2.Remove();

            SystemTime.Unfreeze();

            Assert.NotEqual(producer, producer2);
        }

        private class RegisteredProducerBuilder
        {
            private int complianceYear;
            private string producerRegistrationNumber;
            private string schemeApprovalNumber;

            public RegisteredProducerBuilder()
            {
                complianceYear = 2016;
                producerRegistrationNumber = "WEE/PR1234";
                schemeApprovalNumber = "WEE/SC1234SC";
            }

            public RegisteredProducer Build()
            {
                var scheme = A.Fake<Scheme>();
                A.CallTo(() => scheme.ApprovalNumber)
                    .Returns(schemeApprovalNumber);

                return new RegisteredProducer(producerRegistrationNumber, complianceYear, scheme);
            }

            public static RegisteredProducer NewRegisteredProducer
            {
                get { return new RegisteredProducerBuilder().Build(); }
            }

            public static RegisteredProducer WithComplianceYear(int complianceYear)
            {
                var builder = new RegisteredProducerBuilder();
                builder.complianceYear = complianceYear;

                return builder.Build();
            }

            public static RegisteredProducer WithProducerRegistrationNumber(string producerRegistrationNumber)
            {
                var builder = new RegisteredProducerBuilder();
                builder.producerRegistrationNumber = producerRegistrationNumber;

                return builder.Build();
            }

            public static RegisteredProducer WithSchemeApprovalNumber(string schemeApprovalNumber)
            {
                var builder = new RegisteredProducerBuilder();
                builder.schemeApprovalNumber = schemeApprovalNumber;

                return builder.Build();
            }
        }
    }
}
