using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraphQLDemo.Data.Domain
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Guid { get; set; }
        public string Name { get; set; }

        public int YearOfBirth { get; set; }
        public IList<BookAuthor> Books { get; set; } = new List<BookAuthor>();   

    }
}
