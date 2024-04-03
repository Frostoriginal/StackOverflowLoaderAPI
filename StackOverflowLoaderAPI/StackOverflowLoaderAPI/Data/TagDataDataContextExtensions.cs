using Microsoft.EntityFrameworkCore; // UseSqlServer
using Microsoft.Extensions.DependencyInjection;
using StackOverflowLoaderAPI.Data; // IServiceCollection
namespace StackOverflowLoaderAPI.Data;
public static class TagDataDataContextExtensions
{
    /// <summary>
    /// Adds TagDataDataContext to the specified IServiceCollection. Uses the    SqlServer database provider.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString">Set to override the default.</param>
    /// <returns>An IServiceCollection that can be used to add more  services.</returns>
 public static IServiceCollection AddTagDataDataContext(
 this IServiceCollection services,
 string connectionString /*= "Data Source = host.docker.internal,1433; User iD = sa; Password=Pass@word; Initial Catalog = SOTags; TrustServerCertificate=True;"*/)
    {
        services.AddDbContext<TagDataDataContext>(options =>
        {
        options.UseSqlServer(connectionString);
          //  options.LogTo(Console.WriteLine, LogLevel.None);  // Console
 //           new[] { Microsoft.EntityFrameworkCore
 //.Diagnostics.RelationalEventId.CommandExecuting });
        });
        return services;
    }
}
