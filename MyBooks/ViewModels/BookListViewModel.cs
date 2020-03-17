using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.ViewModels
{
    public class BookListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        public int Removable { get; set; }

        public virtual ICollection<BookViewModel> Books { get; set; }
    }
}
