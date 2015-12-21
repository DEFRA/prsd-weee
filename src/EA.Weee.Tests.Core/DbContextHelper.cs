namespace EA.Weee.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using FakeItEasy;
    using Prsd.Core.Domain;

    public class DbContextHelper
    {
        public DbSet<T> GetFakeDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();

            var dbSet = A.Fake<DbSet<T>>(options => options.Implements(typeof(IQueryable<T>)));

            A.CallTo(() => ((IQueryable<T>)dbSet).Provider).Returns(queryable.Provider);
            A.CallTo(() => ((IQueryable<T>)dbSet).Expression).Returns(queryable.Expression);
            A.CallTo(() => ((IQueryable<T>)dbSet).ElementType).Returns(queryable.ElementType);
            A.CallTo(() => ((IQueryable<T>)dbSet).GetEnumerator()).Returns(queryable.GetEnumerator());

            return dbSet;
        }

        public DbSet<T> GetAsyncEnabledDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();

            var dbSet = A.Fake<DbSet<T>>(options => 
                {
                    options.Implements(typeof(IQueryable<T>));
                    options.Implements(typeof(IDbAsyncEnumerable<T>));
                });

            A.CallTo(() => ((IDbAsyncEnumerable<T>)dbSet).GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<T>(queryable.GetEnumerator()));
            A.CallTo(() => ((IQueryable<T>)dbSet).Provider).Returns(new TestDbAsyncQueryProvider<T>(queryable.Provider));

            A.CallTo(() => ((IQueryable<T>)dbSet).Expression).Returns(queryable.Expression);
            A.CallTo(() => ((IQueryable<T>)dbSet).ElementType).Returns(queryable.ElementType);
            A.CallTo(() => ((IQueryable<T>)dbSet).GetEnumerator()).Returns(queryable.GetEnumerator());

            return dbSet;
        }

        public T SetId<T>(T entity, Guid id) where T : Entity
        {
            // If the name ends with Proxy, this is likely to be a fake object.
            // Set it's Id value using the CallTo method as setting the value directly
            // using reflection fails.
            if (entity.GetType().Name.EndsWith("Proxy"))
            {
                A.CallTo(() => entity.Id).Returns(id);
            }
            else
            {
                typeof(Entity).GetProperty("Id").SetValue(entity, id);
            }

            return entity;
        }
    }
}
