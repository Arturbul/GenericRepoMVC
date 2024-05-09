using AutoMapper;
using GenericRepoMVC.Domain.Models;
using GenericRepoMVC.WebApp.RequestModels;

namespace GenericRepoMVC.WebApp.Mappers
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<Person, CreatePersonRequest>().ReverseMap();
        }
    }
}
