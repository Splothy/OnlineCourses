using Application.Abstractions.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class ConfigurePersistenceServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(connectionString, postgres => postgres.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<CourseRepository>();
        services.AddScoped<ICourseRepository>(provider => provider.GetRequiredService<CourseRepository>());
        services.AddScoped<ICourseQueries>(provider => provider.GetRequiredService<CourseRepository>());
        services.AddScoped<LessonRepository>();
        services.AddScoped<ILessonRepository>(provider => provider.GetRequiredService<LessonRepository>());
        services.AddScoped<ILessonQueries>(provider => provider.GetRequiredService<LessonRepository>());
        return services;
    }
}
