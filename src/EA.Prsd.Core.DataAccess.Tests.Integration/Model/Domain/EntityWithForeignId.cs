﻿namespace EA.Prsd.Core.DataAccess.Tests.Integration.Model.Domain
{
    using EA.Prsd.Core.Domain;
    using System;

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