using GraphQLDemo.Data;
using GraphQLDemo.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.Services
{
    public class AuthorService : IAuthorService, IAsyncDisposable
    {
        private readonly ApplicationDbContext dbContext;

        public AuthorService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            dbContext = dbContextFactory.CreateDbContext();
        }

        public IQueryable<Author> FetchByBookId(int bookId)
        {
            return dbContext.Authors.Where(x => x.Books.Any(b => b.BookId == bookId));
        }

        IQueryable<Author> IAuthorService.FetchByBookIds(IEnumerable<int> bookIds)
        {
            return dbContext.Authors.Where(x => x.Books.Any(b => bookIds.Contains(b.BookId)));
        }

        public async ValueTask DisposeAsync()
        {
            await dbContext.DisposeAsync();
        }
    }
}
