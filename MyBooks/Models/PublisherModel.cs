using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
    public class PublisherModel
    {
        public PublisherModel()
        {
            this.Books = new HashSet<BookModel>();
        }

        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "ErrorObligatorio")]
        public string Name { get; set; }

        public virtual ICollection<BookModel> Books { get; set; }
    }
}
