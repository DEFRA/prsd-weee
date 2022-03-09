namespace EA.Weee.Integration.Tests.Builders.Base
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using Autofac;
    using AutoFixture;
    using DataAccess;
    using Prsd.Core;
    using Prsd.Core.Autofac;
    using Prsd.Core.Domain;

    public abstract class DbTestDataBuilder<T, TK> : InstanceBuilder<T, TK>
        where T : class
        where TK : DbTestDataBuilder<T, TK>
    {
        public WeeeContext DbContext;
        public Fixture Fixture;

        protected DbTestDataBuilder()
        {
            Container = ServiceLocator.Container;

            var userContext = Container.Resolve<IUserContext>();
            var eventDispatcher = Container.Resolve<IEventDispatcher>();

            DbContext = new WeeeContext(userContext, eventDispatcher);
            
            Fixture = new Fixture();
        }

        public override T Create()
        {
            if (instance == null)
            {
                throw new Exception("Failed to create instance of " + typeof(T));
            }
                
            Guard.ArgumentNotNull(() => DbContext, DbContext);

            try
            {
                DbContext.Set<T>().Add(instance);
            }
            catch (Exception)
            {
                EnsureAttachedToEf(instance);
            }

            DbContext.SaveChanges(); // N.B. may only work if we have a connected db context

            Console.WriteLine("Created test " + typeof(T) + ": " + instance);

            DbContext.Dispose();

            return instance;
        }

        public DbEntityEntry<T> EnsureAttachedToEf(T entity)
        {
            var e = DbContext.Entry(entity);
            if (e.State == EntityState.Detached)
            {
                DbContext.Set<T>().Attach(entity);
                e = DbContext.Entry(entity);
            }

            return e;
        }
    }
}
