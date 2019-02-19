namespace EA.Weee.Core.AatfReturn
{
    using System;

    public class Aatf
    {
        public Aatf(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
