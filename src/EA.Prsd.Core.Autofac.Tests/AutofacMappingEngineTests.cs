namespace EA.Prsd.Core.Autofac.Tests
{
    using System;
    using global::Autofac;
    using Mapper;
    using Xunit;

    public class AutofacMappingEngineTests
    {
        private readonly AutofacMappingEngine engine;

        public AutofacMappingEngineTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DecimalMapper>().AsImplementedInterfaces();
            var container = builder.Build();
            engine = new AutofacMappingEngine(container.BeginLifetimeScope());
        }

        [Fact]
        public void CanMap_Generic()
        {
            var result = engine.Map<string, decimal>("1.0");
            Assert.Equal(1.0m, result);
        }

        [Fact]
        public void CanMap_NonGeneric()
        {
            var result = engine.Map<decimal>("1.0");
            Assert.Equal(1.0m, result);
        }

        [Fact]
        public void CanMapWithParameter_Generic()
        {
            var result = engine.Map<string, bool, decimal>("1.0", true);
            Assert.Equal(1.0m, result);
        }

        [Fact]
        public void CanMapWithParameter_NonGeneric()
        {
            var result = engine.Map<decimal>("1.0", true);
            Assert.Equal(1.0m, result);
        }

        [Fact]
        public void Map_Generic_CanHandleNull()
        {
            string nullString = null;
            var result = engine.Map<string, decimal>(nullString);
            Assert.Equal(default(decimal), result);
        }

        [Fact]
        public void Map_NonGeneric_CanHandleNull()
        {
            string nullString = null;
            var result = engine.Map<decimal>(nullString);
            Assert.Equal(default(decimal), result);
        }

        [Fact]
        public void MapWithParameter_Generic_CanHandleNull()
        {
            string nullString = null;
            var result = engine.Map<string, bool, decimal>(nullString, true);
            Assert.Equal(default(decimal), result);
        }

        [Fact]
        public void MapWithParameter_NonGeneric_CanHandleNull()
        {
            string nullString = null;
            var result = engine.Map<decimal>(nullString, true);
            Assert.Equal(default(decimal), result);
        }

        private class DecimalMapper : IMap<string, decimal>, IMapWithParameter<string, bool, decimal>
        {
            public decimal Map(string source)
            {
                return Convert.ToDecimal(source);
            }

            public decimal Map(string source, bool parameter)
            {
                return Convert.ToDecimal(source);
            }
        }
    }
}