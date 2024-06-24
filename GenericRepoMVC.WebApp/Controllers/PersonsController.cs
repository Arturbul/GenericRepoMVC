using AutoMapper;
using GenericRepoMVC.Domain.Models;
using GenericRepoMVC.Servicies;
using GenericRepoMVC.ViewModels;
using GenericRepoMVC.WebApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
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
        public async Task<IActionResult> Get([FromQuery] GetPersonRequest? getPersonsRequest = null,
            [FromQuery] PersonOrderByRequest? orderByRequest=null)
        {
            if (getPersonsRequest == null)
            {
                return Ok(await _personServicies.Get());
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var filter = ExpressionBuilder.CreateFilter<Person, GetPersonRequest?>(getPersonsRequest);
            var orderBy = createOrderBy(orderByRequest);
            return Ok(await _personServicies.Get(filter: filter,orderBy: orderBy));

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
            var filter = ExpressionBuilder.CreateFilter<Person, GetPersonRequest?>(getPerson);
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

        private Func<IQueryable<Person>, IOrderedQueryable<Person>>? createOrderBy(PersonOrderByRequest? orderByRequest)
        {
            if (orderByRequest == null || areAllPropertiesDefault(orderByRequest))
            {
                return null;
            }

            var sortedRequestDict = getPropertiesNameInOrder(orderByRequest);

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

        private bool areAllPropertiesDefault(PersonOrderByRequest orderByRequest)
        {
            var props = orderByRequest.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(orderByRequest);
                var defaultValue = GetDefaultValue(prop.PropertyType);

                if (!Equals(value, defaultValue))
                {
                    return false;
                }
            }
            return true;
        }

        private object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private IEnumerable<string> getPropertiesNameInOrder(PersonOrderByRequest orderByRequest)
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
                    orderPriorityPairs.Add(prop.Name.Replace("OrderBy",""), value);
                }
            }
            var sortedDict = orderPriorityPairs
                .OrderBy(e => e.Value)
                .ToDictionary();
            return sortedDict.Keys.ToArray();
        }
    }
}
