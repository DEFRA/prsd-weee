namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class SICCode : Entity
    {
         public SICCode(string name)
         {
             Name = name;
         }

        protected SICCode()
        {
        }

        public string Name { get; private set; }

        public virtual Guid ProducerId { get; private set; }

        public virtual Producer Producer { get; private set; }
    }
}
