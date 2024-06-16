using AutoMapper;
using GenericRepoMVC.Domain.Models;
using GenericRepoMVC.Servicies;
using GenericRepoMVC.WebApp.RequestModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace GenericRepoMVC.WebApp.Controllers
{
    [Route("api/persons")]
    [ApiController]
    public class PersonsController : Controller
    {
        private readonly ITServicies<Person> _personServicies;
        private readonly IMapper _mapper;
        public PersonsController(ITServicies<Person> personServicies, IMapper mapper)
        {
            _personServicies = personServicies;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetPersonRequest? getPersons = null,
            [FromQuery] PersonOrderByRequest? orderByRequest = null)
        {
            if (getPersons == null)
            {
                return Ok(await _personServicies.Get());
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var filter = createFilter(getPersons);
            var order = createOrderBy(orderByRequest);
            return Ok(await _personServicies.Get(filter: filter, orderBy: order));

        }


        [HttpGet("single")]
        public async Task<IActionResult> GetSingle([FromQuery] GetPersonRequest? getPerson = null)
        {
            if (getPerson == null)
            {
                return Ok(await _personServicies.GetSingle());
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var filter = createFilter(getPerson);
            return Ok(await _personServicies.GetSingle(filter));

        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePersonRequest personRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var person = _mapper.Map<Person>(personRequest);
            return Ok(await _personServicies.Create(person));
        }

        private Expression<Func<Person, bool>> createFilter(GetPersonRequest request)
        {
            var parameterExp = Expression.Parameter(typeof(Person), "p");
            var expressions = new List<Expression>();

            if (request.Id != null)
            {
                var idExp = Expression.Property(parameterExp, nameof(Person.Id));
                var idValue = Expression.Constant(request.Id);
                expressions.Add(Expression.Equal(idExp, idValue));
            }

            if (!string.IsNullOrEmpty(request.FirstName))
            {
                var firstNameExp = Expression.Property(parameterExp, "FirstName");
                var firstNameValue = Expression.Constant(request.FirstName);
                expressions.Add(Expression.Call(firstNameExp, "Contains", null, firstNameValue));
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                var lastNameExp = Expression.Property(parameterExp, nameof(Person.LastName));
                var lastNameValue = Expression.Constant(request.LastName);
                expressions.Add(Expression.Call(lastNameExp, "Contains", null, lastNameValue));
            }

            if (request.DateOfBirthFrom != null)
            {
                var dateOfBirthFromExp = Expression.Property(parameterExp, nameof(Person.DateOfBirth));
                var dateOfBirthFromValue = Expression.Constant(request.DateOfBirthFrom.Value);
                expressions.Add(Expression.GreaterThanOrEqual(dateOfBirthFromExp, dateOfBirthFromValue));
            }

            if (request.DateOfBirthTo != null)
            {
                var dateOfBirthToExp = Expression.Property(parameterExp, nameof(Person.DateOfBirth));
                var dateOfBirthToValue = Expression.Constant(request.DateOfBirthTo.Value);
                expressions.Add(Expression.LessThanOrEqual(dateOfBirthToExp, dateOfBirthToValue));
            }

            Expression body = expressions.Count > 0
                ? expressions.Aggregate(Expression.AndAlso)
                : Expression.Constant(true);

            var lambda = Expression.Lambda<Func<Person, bool>>(body, parameterExp);
            return lambda;
        }
        /*private Func<IQueryable<Person>, IOrderedQueryable<Person>>? createOrderBy(PersonOrderByRequest? orderByRequest)
        {
            if (orderByRequest == null)
            {
                return null;
            }
            var sortedRequestDict = getSortedRequestDict(orderByRequest);
            var orderByList = new List<Expression<Func<Person, object>>>();
            foreach (var item in sortedRequestDict)
            {
                orderByList.Add(x => x.Equals(item.ToString()));
            }
            orderByList.Add(x => x.Id);

            var resultOrderedQueryable = orderByList.Aggregate<Expression<Func<Person, object>>, IOrderedQueryable<Person>>
                (
                null, (current, orderBy)
                => current != null ?
                current.ThenBy(orderBy)
                : query.OrderBy(orderBy)
                );

            *//* if (orderByRequest.OrderById)
             {
                 orderBy = query => query.OrderBy(p => p.Id);
             }
             else if (orderByRequest.OrderByFirstName)
             {
                 orderBy = query => query.OrderBy(p => p.FirstName);
             }
             else if (orderByRequest.OrderByLastName)
             {
                 orderBy = query => query.OrderBy(p => p.LastName);
             }
             else if (orderByRequest.OrderByDateOfBirth)
             {
                 orderBy = query => query.OrderBy(p => p.DateOfBirth);
             }*//*
            //orderBy = query => query.OrderBy(p => p.DateOfBirth).ThenBy(p => p.FirstName);
            return orderBy;
        }
*/
        private Func<IQueryable<Person>, IOrderedQueryable<Person>>? createOrderBy(PersonOrderByRequest? orderByRequest)
        {
            if (orderByRequest == null)
            {
                return null;
            }

            var sortedRequestDict = getSortedRequestDict(orderByRequest);

            IOrderedQueryable<Person> ApplyOrdering(IQueryable<Person> query, string propertyName, bool isFirst)
            {
                var parameter = Expression.Parameter(typeof(Person), "p");
                var property = Expression.Property(parameter, propertyName);
                var lambda = Expression.Lambda<Func<Person, object>>(Expression.Convert(property, typeof(object)), parameter);

                if (isFirst)
                {
                    return query.OrderBy(lambda);
                }
                else
                {
                    return ((IOrderedQueryable<Person>)query).ThenBy(lambda);
                }
            }

            return query =>
            {
                IOrderedQueryable<Person> orderedQuery = null;

                foreach (var propertyName in sortedRequestDict)
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = ApplyOrdering(query, propertyName, true);
                    }
                    else
                    {
                        orderedQuery = ApplyOrdering(orderedQuery, propertyName, false);
                    }
                }

                return orderedQuery;
            };
        }

        private IEnumerable<string> getSortedRequestDict(PersonOrderByRequest orderByRequest)
        {
            // 0 0 1 2 
            //id fname lname date
            var props = orderByRequest.GetType().GetProperties(); //prop names 4 orderby
            var orderPriorityPairs = new Dictionary<string, int>();
            foreach (var prop in props)
            {
                var value = (int)prop.GetValue(orderByRequest)!;
                if (value != 0)
                {
                    orderPriorityPairs.Add(prop.Name, value);
                }
            }
            var sortedDict = orderPriorityPairs
                .OrderBy(e => e.Value)
                .ToDictionary();
            return sortedDict.Keys.ToArray();
        }
    }
}
