using GraphQLDemo.Data.Domain;

namespace GraphQLDemo.Services
{
    public interface IBookService
    {
        Task<Book> FetchBook(int id);
        IQueryable<Book> FetchBooks(IEnumerable<int> ids);
    }
}
