using MyBooks.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MyBooks.Models
{
    public class BookListModel
    {
        public BookListModel()
        {
           this.Book_BookLists = new HashSet<Book_BookListModel>();
        }

        public int Id { get; set; }


        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "ErrorObligatorio"), MaxLength(150, ErrorMessage = "ErrorMaxLongitud")]
        public string Name { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "ErrorObligatorio"), RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "ErrorColorHexadecimal")]
        public string Color { get; set; }

        [Display(Name = "Usuario")]
        public UserModel User { get; set; }

        public string UserId { get; set; }

        public virtual ICollection<Book_BookListModel> Book_BookLists { get; set; }
        [NotMapped]
        public virtual ICollection<BookModel> Books
        {
            get
            {
                if (Book_BookLists == null) return new List<BookModel>();
                return Book_BookLists.Select(bbl => bbl.Book).ToList();
            }
            set
            {
                if (value == null)
                    Book_BookLists = null;
                else
                    Book_BookLists = value.Select(b => new Book_BookListModel() { BookList = this, Book = b }).ToList();
            }
        }

        public bool Removable { get; set; }

        public int Order { get; set; }

        public BookListViewModel getViewModel()
        {
            return new BookListViewModel()
            {
                Id = Id,
                Name = Name,
                Color = Color,
                Removable = (Removable) ? 1 : 0,
                Books = Books.Select(b => b.getViewModel()).ToList()
            };
        }
    }
}
