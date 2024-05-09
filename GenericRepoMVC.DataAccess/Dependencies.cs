using GenericRepoMVC.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepoMVC.DataAccess
{
    public class Dependencies
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<ITRepository<Person>, TRepository<Person>>();
        }
    }
}
