namespace EA.Weee.RequestHandlers.AatfReturn
{
    using DataAccess;
    using Domain.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Prsd.Core.Domain;
    using Specification;

    public class GenericDataAccess : IGenericDataAccess
    {
        private readonly WeeeContext context;

        public GenericDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Guid> Add<TEntity>(TEntity entity) where TEntity : Entity
        {
            context.Set<TEntity>().Add(entity);

            await context.SaveChangesAsync();

            return entity.Id;
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
    }
}
