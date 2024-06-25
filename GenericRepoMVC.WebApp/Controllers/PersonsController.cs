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
            var orderBy = ExpressionBuilder.CreateOrderBy<Person,PersonOrderByRequest?>(orderByRequest);    
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

       
    }
}
