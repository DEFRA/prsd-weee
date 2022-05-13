namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Prsd.Core.Domain;
    using Specification;

    public interface IGenericDataAccess
    {
        Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : Entity;

        Task AddMany<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity;

        Task<IEnumerable<TEntity>> GetAll<TEntity>() where TEntity : class;

        Task<TEntity> GetById<TEntity>(Guid id) where TEntity : Entity;

        Task<TEntity> GetSingleByExpression<TEntity>(ISpecification<TEntity> specification) where TEntity : Entity;

        Task<List<TEntity>> GetManyByExpression<TEntity>(ISpecification<TEntity> specification) where TEntity : Entity;

        Task<List<TEntity>> GetManyByReturnId<TEntity>(Guid returnId) where TEntity : Entity, IReturnOption;

        void Remove<TEntity>(TEntity entity) where TEntity : Entity;

        void RemoveMany<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity;
    }
}