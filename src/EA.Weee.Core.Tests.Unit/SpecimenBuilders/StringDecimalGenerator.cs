namespace EA.Weee.Core.Tests.Unit.SpecimenBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture.Kernel;

    public class StringDecimalGenerator : ISpecimenBuilder
    {
        private readonly Random random;

        public StringDecimalGenerator()
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
