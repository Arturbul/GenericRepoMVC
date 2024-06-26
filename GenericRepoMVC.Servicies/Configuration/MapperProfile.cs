using AutoMapper;
using GenericRepoMVC.Domain.Models;
using GenericRepoMVC.ViewModels;

namespace GenericRepoMVC.WebApp.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //Person
            CreateMap<Person, CreateOrUpdatePersonRequest>().ReverseMap();
        }
    }
}
