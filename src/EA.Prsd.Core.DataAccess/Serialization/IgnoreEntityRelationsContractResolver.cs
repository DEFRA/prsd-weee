﻿namespace EA.Prsd.Core.DataAccess.Serialization
{
    using EA.Prsd.Core.Domain;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///     A contract resolver which ignores properties that are of type Entity or IEnumerable&lt;Entity&gt;.
    /// </summary>
    internal class IgnoreEntityRelationsContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var isDefaultValueIgnored =
                ((property.DefaultValueHandling ?? DefaultValueHandling.Ignore)
                 & DefaultValueHandling.Ignore) != 0;
            if (isDefaultValueIgnored
                && (typeof(Entity).IsAssignableFrom(property.PropertyType)
                    || typeof(IEnumerable<Entity>).IsAssignableFrom(property.PropertyType)))
            {
                Predicate<object> shouldSerialize = obj => false;
                var oldShouldSerialize = property.ShouldSerialize;
                property.ShouldSerialize = oldShouldSerialize != null
                    ? o => oldShouldSerialize(o) && shouldSerialize(o)
                    : shouldSerialize;
            }

            return property;
        }
    }
}