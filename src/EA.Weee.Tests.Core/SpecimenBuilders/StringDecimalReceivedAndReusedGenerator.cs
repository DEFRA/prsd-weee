namespace EA.Weee.Tests.Core.SpecimenBuilders
{
    using System;
    using System.Reflection;
    using AutoFixture.Kernel;

    public class StringDecimalReceivedAndReusedGenerator : ISpecimenBuilder
    {
        private readonly Random random;

        public StringDecimalReceivedAndReusedGenerator()
        {
            random = new Random();
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (request is PropertyInfo pi)
            {
                if (pi.Name.Equals("Received") || pi.Name.Equals("Reused"))
                {
                    return $"{random.Next(100, 999)}";
                }
            }

            return new NoSpecimen();
        }
    }
}
