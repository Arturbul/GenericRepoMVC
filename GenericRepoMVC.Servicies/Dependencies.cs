using GenericRepoMVC.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepoMVC.Servicies
{
    public class Dependencies
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<ITServicies<Person>, PersonServicies>();

            DataAccess.Dependencies.Register(services);
        }
    }
}
