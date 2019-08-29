using System;
using System.ComponentModel.DataAnnotations.Schema;
using EA.Prsd.Core.Domain;

namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain
{
    public class EntityWithChildren : Entity
    {
        public virtual SimpleEntity SimpleEntityA { get; private set; }

        public Guid SimpleEntityBId { get; private set; }

        public virtual SimpleEntity SimpleEntityB { get; private set; }

        public EntityWithChildren() : base()
        {
        }

        public EntityWithChildren(SimpleEntity entityA, SimpleEntity entityB) : base()
        {
            // Entity referenced only as an entity
            SimpleEntityA = entityA;

            // Entity referenced both by Id and entity, but only setting entity
            SimpleEntityB = entityB;
        }

        public void UpdateSimpleEntityB(SimpleEntity entityB)
        {
            SimpleEntityB = entityB;
        }
    }
}