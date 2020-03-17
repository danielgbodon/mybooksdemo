using System.ComponentModel.DataAnnotations;

namespace MyBooks.ViewModels
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "ErrorObligatorio"), EmailAddress(ErrorMessage = "ErrorEmail")]
        public string Username { get; set; }

        [Required(ErrorMessage = "ErrorObligatorio"), DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
