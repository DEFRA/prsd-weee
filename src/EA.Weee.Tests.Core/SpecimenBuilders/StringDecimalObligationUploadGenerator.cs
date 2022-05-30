namespace EA.Weee.Tests.Core.SpecimenBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using AutoFixture.Kernel;

    public class StringDecimalObligationUploadGenerator : ISpecimenBuilder
    {
        private readonly Random random;

        public StringDecimalObligationUploadGenerator()
        {
            random = new Random();
        }

        public object Create(object request, ISpecimenContext context)
        {
            var categories = new List<string>()
            {
                "Cat1",
                "Cat2",
                "Cat3",
                "Cat4",
                "Cat5",
                "Cat6",
                "Cat7",
                "Cat8",
                "Cat9",
                "Cat10",
                "Cat11",
                "Cat12",
                "Cat13",
                "Cat14"
            };

            if (request is PropertyInfo pi)
            {
                if (categories.Contains(pi.Name))
                {
                    return $"{random.Next(100, 999)}";
                }
            }

            return new NoSpecimen();
        }
    }
}
