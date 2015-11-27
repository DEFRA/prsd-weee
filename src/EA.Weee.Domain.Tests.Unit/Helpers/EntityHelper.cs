namespace EA.Weee.Domain.Tests.Unit.Helpers
{
    using System;
    using Prsd.Core.Domain;
    using Prsd.Core.Identity;

    public static class EntityHelper
    {
        /// <summary>
        /// Set all Entity Ids to a new Comb Guid.
        /// </summary>
        /// <param name="entities"></param>
        public static void SetEntityIds(params Entity[] entities)
        {
            foreach (var entity in entities)
            {
                typeof(Entity).GetProperty("Id").SetValue(entity, GuidCombGenerator.GenerateComb());
            }
        }

        /// <summary>
        /// Set the Entity Id to the given Guid.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        public static void SetEntityId(Entity entity, Guid id)
        {
            typeof(Entity).GetProperty("Id").SetValue(entity, id);
        }
    }
}