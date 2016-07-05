using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Data.Entity;
using envSeer.DAL.Core;

namespace envSeer.DAL.Repositories
{
    // implementation of IRepository interface - a generic repository (using generic class notation, hence the '<TEntity>')
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        // I've changed this from a generic dbcontext to our dbcontext (since we're likely to have only one DBContext in the application)
        // if we had multiple dbcontexts we could make this generic and cast it within our custom repositories (i.e. within a 'SpecificContextClass' property: 'return dbContext as SpecificContextClass')
        protected envSeerDBContext dbContext;

        // constructor - takes an instantiation of our custom dbcontext
        public Repository(envSeerDBContext context)
        {
            dbContext = context;
        }

        // add an entity
        public void Add(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
        }

        // delete an entity
        public void Delete(TEntity entity)
        {
            dbContext.Set<TEntity>().Remove(entity);
        }

        // gets an entity by an integer id value (primary key)
        public TEntity Get(int id)
        {
            return dbContext.Set<TEntity>().Find(id);
        }

        // gets all the entities in a set
        public IEnumerable<TEntity> GetAll()
        {
            return dbContext.Set<TEntity>().ToList();
        }

        // get entities via a predicate (a LINQ lambda expression passed to this method)
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return dbContext.Set<TEntity>().Where(predicate);
        }
    }
}