using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
    public class BookCategoryModel
    {
        public BookCategoryModel()
        {
            this.Books = new HashSet<Book_CategoryModel>();
        }

        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "ErrorObligatorio")]
        public string Name { get; set; }

        public virtual ICollection<Book_CategoryModel> Books { get; set; }
    }
}
