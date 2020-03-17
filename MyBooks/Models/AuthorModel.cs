using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
    public class AuthorModel
    {
        public AuthorModel()
        {
            this.Books = new HashSet<Author_BookModel>();
        }

        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "ErrorObligatorio")]
        public string Name { get; set; }

        public virtual ICollection<Author_BookModel> Books { get; set; }
    }
}
