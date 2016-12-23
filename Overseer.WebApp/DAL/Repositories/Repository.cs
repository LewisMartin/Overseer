using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Data.Entity;
using Overseer.WebApp.DAL.Core;

namespace Overseer.WebApp.DAL.Repositories
{
    // implementation of IRepository interface - a generic repository (using generic class notation, hence the '<TEntity>')
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        // I've changed this from a generic dbcontext to our dbcontext (since we're likely to have only one DBContext in the application)
        // if we had multiple dbcontexts we could make this generic and cast it within our custom repositories (i.e. within a 'SpecificContextClass' property: 'return dbContext as SpecificContextClass')
        protected OverseerDBContext dbContext;

        // constructor - takes an instantiation of our custom dbcontext
        public Repository(OverseerDBContext context)
        {
            dbContext = context;
        }

        // add an entity
        public void Add(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
        }

        // add multiple entities
        public void AddRange(ICollection<TEntity> entities)
        {
            dbContext.Set<TEntity>().AddRange(entities);
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

        // count total rows
        public int CountRows()
        {
            return dbContext.Set<TEntity>().Count();
        }
    }
}