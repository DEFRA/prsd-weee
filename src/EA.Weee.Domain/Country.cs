namespace EA.Weee.Domain
{
    using System;

    public class Country
    {
        protected Country()
        {
        }

        public Guid Id { get; protected set; }

        public string Name { get; private set; }
    }
}
