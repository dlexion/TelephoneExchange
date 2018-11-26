﻿using System.Collections.Generic;

namespace BillingSystem.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly IList<T> _entitiesList = new List<T>();

        public GenericRepository(IList<T> entities)
        {
            _entitiesList = entities;
        }

        public ICollection<T> GetAll()
        {
            return _entitiesList;
        }

        public void Add(T entity)
        {
            _entitiesList.Add(entity);
        }

        public void Remove(T entity)
        {
            _entitiesList.Remove(entity);
        }
    }
}