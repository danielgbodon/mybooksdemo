using MyBooks.Core.Validations;
using MyBooks.Models;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.ViewModels
{
    public class RegisterUserViewModel
    {

        [Required(ErrorMessage = "ErrorObligatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ErrorObligatorio"), EmailAddress(ErrorMessage = "ErrorEmail")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "ErrorObligatorio"), DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "ErrorObligatorio"), DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "ErrorContrasenasCoincidencia")]
        public string RepeatPassword { get; set; }

        [Required(ErrorMessage = "ErrorObligatorio"), UserProfile(ErrorMessage = "ErrorPerfil")]
        public string Profile { get; set; }

        public string Culture { get; set; }

        public string Role
        {
            get
            {
                return Profile.ToLower();
            }
        }

        public UserModel getUser()
        {
            return new UserModel() { 
                Name = Name,
                UserName = Email,
                Email = Email,
                Culture = Culture
            };
        }
        
    }
}
