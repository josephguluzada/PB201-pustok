using Microsoft.Extensions.DependencyInjection;
using Pustok.Core.Repositories;
using Pustok.Data.Repositories;

namespace Pustok.Data;

public static class ServiceRegistration
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookImageRepository, BookImageRepository>();
    }
}
