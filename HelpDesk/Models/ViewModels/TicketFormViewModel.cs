using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Models.ViewModels
{
  
    public class TicketFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 150 caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 4000 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Seleccione una categoría.")]
        public string Categoria { get; set; }
    }
}
