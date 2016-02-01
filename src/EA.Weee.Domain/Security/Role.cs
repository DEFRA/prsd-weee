namespace EA.Weee.Domain.Security
{
    using System;

    public class Role
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        protected Role()
        {
        }

        public Role(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
