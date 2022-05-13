namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.AatfReturn;
    using Prsd.Core.Domain;
    using Specification;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess;

    public class GenericDataAccess : IGenericDataAccess
    {
        private readonly WeeeContext context;

        public GenericDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : Entity
        {
            context.Set<TEntity>().Add(entity);

            await context.SaveChangesAsync();

            return entity;
        }

        public Task AddMany<TEntity>(IEnumerable<TEntity> amounts) where TEntity : Entity
        {
            context.Set<TEntity>().AddRange(amounts);

            return context.SaveChangesAsync();
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : Entity
        {
            if (entity != null)
            {
                context.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
            }
        }

        public void RemoveMany<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
        {
            foreach (var amount in entities)
            {
                context.Entry(amount).State = System.Data.Entity.EntityState.Deleted;
            }
        }

        public async Task<IEnumerable<TEntity>> GetAll<TEntity>() where TEntity : class
        {
            return await context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetById<TEntity>(Guid id) where TEntity : Entity
        {
            return await context.Set<TEntity>().SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TEntity> GetSingleByExpression<TEntity>(ISpecification<TEntity> specification) where TEntity : Entity
        {
            return await context.Set<TEntity>().SingleOrDefaultAsync(specification.ToExpression());
        }

        public async Task<List<TEntity>> GetManyByExpression<TEntity>(ISpecification<TEntity> specification) where TEntity : Entity
        {
            return await context.Set<TEntity>().Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<List<TEntity>> GetManyByReturnId<TEntity>(Guid returnId) where TEntity : Entity, IReturnOption
        {
            return await context.Set<TEntity>().Where(e => e.ReturnId == returnId).ToListAsync();
        }
    }
}
