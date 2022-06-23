namespace EA.Weee.Tests.Core.SpecimenBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using AutoFixture.Kernel;
    using Weee.Core.AatfReturn;

    public class AatfFacilityTypeGenerator : ISpecimenBuilder
    {
        public AatfFacilityTypeGenerator()
        {
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (request is PropertyInfo pi)
            {
                if (pi.Name == "FacilityType")
                {
                    return FacilityType.Aatf;
                }
            }

            return new NoSpecimen();
        }
    }
}
