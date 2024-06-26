﻿using GenericRepoMVC.DataAccess;
using GenericRepoMVC.Domain.Models;
using System.Linq.Expressions;

namespace GenericRepoMVC.Servicies
{
    public class PersonServicies : ITServicies<Person>
    {
        private readonly ITRepository<Person> _personRepo;
        public PersonServicies(ITRepository<Person> personRepo)
        {
            _personRepo = personRepo;
        }
        public async Task<IEnumerable<Person>> Get(Expression<Func<Person, bool>>? filter = null,
            Func<IQueryable<Person>, IOrderedQueryable<Person>>? orderBy = null)
        {
            return await _personRepo.Get(filter: filter, orderBy: orderBy);

        }
        public async Task<Person?> GetSingle(Expression<Func<Person, bool>>? filter = null)
        {
            return await _personRepo.GetSingle(filter);
        }
        public async Task<Person> CreateOrUpdate(Person entity)
        {
            return entity.Id ==0 ? 
                await _personRepo.Create(entity) 
                : await _personRepo.Update(entity);
        }
     
        public Task<int> Delete(Person entity)
        {
            return _personRepo.Delete(entity);
        }
    }
}
