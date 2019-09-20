using System;
using EA.Prsd.Core.Domain;

namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain
{
    public class EntityWithForeignId : Entity
    {
        public Guid SimpleEntityId { get; private set; }

        public EntityWithForeignId()
        {
        }

        public EntityWithForeignId(SimpleEntity simpleEntity)
        {
            // Entity referenced only as an entity
            SimpleEntityId = simpleEntity.Id;
        }
    }
}