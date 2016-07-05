using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace envSeer.DAL.Core
{
    // a generic repository (a generic interface hence the '<TEntity>') of type TEntity, where TEntity is a class (our domain models)
    // Note - we return IEnumerable collections over IQueryable as a best practise - we want all query logic to be WITHIN the repository
    public interface IRepository<TEntity> where TEntity : class
    {
        // method to get an entity by it's Id (primary key)
        TEntity Get(int id);
        // method to get a range of entities - returns an IEnumerable
        IEnumerable<TEntity> GetAll();

        // method that will allow us to find entities using lambda expressions
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        // method to add an entity to the collection
        void Add(TEntity entity);

        // method to delete an entity
        void Delete(TEntity entity);
    }
}