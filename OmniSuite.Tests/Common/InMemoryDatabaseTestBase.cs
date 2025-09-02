using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniSuite.Persistence;

namespace OmniSuite.Tests.Common
{
    public abstract class InMemoryDatabaseTestBase : TestBase
    {
        protected ApplicationDbContext Context { get; private set; } = null!;
        protected IServiceProvider ServiceProvider { get; private set; } = null!;

        protected virtual void SetupDatabase()
        {
            var services = CreateServiceCollection();

            // Add in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            });

            ServiceProvider = services.BuildServiceProvider();
            Context = ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Context.Database.EnsureCreated();
            
            // Setup default authenticated user for tests
            SetupAuthenticatedUser(Guid.NewGuid());
        }

        protected virtual void CleanupDatabase()
        {
            Context?.Database.EnsureDeleted();
            Context?.Dispose();
        }

        protected virtual async Task SeedDatabaseAsync()
        {
            // Override in derived classes to seed test data
            await Task.CompletedTask;
        }

        protected virtual async Task<T> SaveEntityAsync<T>(T entity) where T : class
        {
            Context.Add(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        protected virtual async Task<List<T>> SaveEntitiesAsync<T>(IEnumerable<T> entities) where T : class
        {
            Context.AddRange(entities);
            await Context.SaveChangesAsync();
            return entities.ToList();
        }
    }
}
