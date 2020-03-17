using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
    public class UserModel : IdentityUser
    {
        public UserModel() : base()
        {
            this.BookLists = new HashSet<BookListModel>();
        }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "ErrorObligatorio"), MaxLength(80, ErrorMessage = "ErrorMaxLongitud")]
        public string Name { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(150, ErrorMessage = "ErrorMaxLongitud")]
        public string Surname { get; set; }

        public string Culture { get; set; }

        public virtual ICollection<BookListModel> BookLists { get; set; }

        public bool Welcomed { get; set; }
        public bool WebTrained { get; set; }

        
    }
}
