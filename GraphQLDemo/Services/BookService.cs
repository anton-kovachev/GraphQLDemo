using GraphQLDemo.Data;
using GraphQLDemo.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.Services
{
    public class BookService : IBookService, IAsyncDisposable
    {
        private readonly ApplicationDbContext dbContext;

        public BookService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            dbContext = dbContextFactory.CreateDbContext();
        }

        public async Task<Book> FetchBook(int id)
        {
            return await dbContext.Books.FindAsync(id);
        }

        public IQueryable<Book> FetchBooks(IEnumerable<int> ids)
        {
            return dbContext.Books.Where(x => ids.Contains(x.Id));
        }

        public async ValueTask DisposeAsync()
        {
            await dbContext.DisposeAsync();
        }
    }
}
