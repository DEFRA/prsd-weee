namespace EA.Weee.RequestHandlers.AatfReturn
{
    using DataAccess;
    using Domain.AatfReturn;
    using System;
    using System.Data.Entity;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Prsd.Core.Domain;

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

        public async Task<TEntity> GetById<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : Entity
        {
            return await context.Set<TEntity>().SingleOrDefaultAsync(expression);
        }
    }
}
