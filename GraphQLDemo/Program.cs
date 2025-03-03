using GraphQLDemo.Data;
using GraphQLDemo.Data.Domain;
using GraphQLDemo.Services;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<IAuthorService, AuthorService>();
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(@"Server =.\SQLEXPRESS;Database=MyBookStoreDB;Trusted_Connection=True;",
    provider => provider.EnableRetryOnFailure()));

builder.Services.AddGraphQLServer()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .RegisterService<IBookService>()
    .RegisterService<IAuthorService>()
    .AddQueryType<BooksQuery>()
    .AddType<BookType>()
    .AddType<AuthorType>();

var app = builder.Build();
app.UseRouting().UseEndpoints(endpoints => endpoints.MapGraphQL());


using (var context = builder.Services.BuildServiceProvider().
            GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext())
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    context.Books.AddRange(
        new Book
        {
            Title = "I love GraphQL",
            YearPublished = 2019,
            Authors = new List<BookAuthor> {
                new BookAuthor {
                    Author = new Author {
                        Name = "Brandon Minnich",
                        YearOfBirth = 1991
                    }
                }
            },
        },
        new Book
        {
            Title = "GraphQL is the future",
            YearPublished = 2019,
            Authors = new List<BookAuthor> {
                new BookAuthor {
                    Author = new Author {
                        Name = "John Willis",
                        YearOfBirth = 1991
                    }
                }
            },
        },
        new Book
        {
            Title = "I love SOAP + XML",
            YearPublished = 2019,
            Authors = new List<BookAuthor> {
                new BookAuthor {
                    Author = new Author {
                        Name = "Mark Johnson",
                        YearOfBirth = 1971
                    }
                }
            },
        });

    await context.SaveChangesAsync();
}


app.MapGet("/", () => "Hello World!");

app.Run();

public class BookResolver
{
    public async Task<Book> FetchBookById(int id, IBookService bookService) => await bookService.FetchBook(id);
    public IQueryable<Book> FetchBooks(IEnumerable<int> ids, IBookService bookService) => bookService.FetchBooks(ids);
}

public class AuthorsResolver
{
    public IQueryable<Author> FetchAuthors(int bookId, IAuthorService authorService) => authorService.FetchByBookId(bookId);
}

public class BooksQuery : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("Book")
            .UseProjection()
            .Argument("id", a => a.Type<NonNullType<IntType>>())
            .ResolveWith<BookResolver>(r => r.FetchBookById(default, default))
            .Type<BookType>();

        descriptor.Field("Books")
            .UseProjection()
            .UseFiltering()
            .UseSorting()
            .UsePaging(options: new PagingOptions { MaxPageSize = 2 } )
            .Argument("ids", a => a.Type<ListType<IntType>>())
            .ResolveWith<BookResolver>(r => r.FetchBooks(default, default))
            .Type<ListType<BookType>>();
    }
}

public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor.Field("authors")
            .UseProjection()
            .Resolve(context =>
            {
                var book = context.Parent<Book>();
                var authorService = context.Service<IAuthorService>();
                return context.BatchDataLoader<int, Author>((keys, value) =>
                {
                    var result = authorService.FetchByBookIds(keys).ToList();
                    return Task.FromResult((result.ToDictionary(t => t.Guid)) as IReadOnlyDictionary<int, Author>);
                }).LoadAsync(book.Id);
            });
    }
}

public class AuthorType : ObjectType<Author>
{
    protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
    {

    }
}

