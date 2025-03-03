using GraphQLDemo.Data.Domain;

namespace GraphQLDemo.Services
{
    public interface IAuthorService
    {
        IQueryable<Author> FetchByBookId(int bookId);
        IQueryable<Author> FetchByBookIds(IEnumerable<int> bookIds);
    }
}
