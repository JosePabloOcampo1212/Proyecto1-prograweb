using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese el correo electrónico.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ingrese la contraseña.")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Contrasena { get; set; }

        public string ReturnUrl { get; set; }
    }
}
