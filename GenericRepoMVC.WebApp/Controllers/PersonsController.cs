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
        public async Task<IActionResult> Get([FromQuery] GetPersonRequest? getPersons = null)
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
            return Ok(await _personServicies.Get(filter: filter));

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
            return Ok(await _personServicies.GetSingle(filter: filter));

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


    }
}
