using HelpDesk.Datos;
using HelpDesk.Models.Entidades;
using HelpDesk.Seguridad;

namespace HelpDesk.Negocio
{
   
    public class AutenticacionServicio
    {
     
        private const string MensajeCredenciales = "El correo o la contraseña no son correctos.";

        private readonly UsuarioRepositorio _usuarios = new UsuarioRepositorio();

        public Usuario ValidarCredenciales(string email, string contrasena, out string error)
        {
            error = null;
            Usuario usuario = string.IsNullOrWhiteSpace(email) ? null : _usuarios.ObtenerPorEmail(email.Trim());

            if (usuario == null)
            {
               
                HashContrasena.SimularVerificacion(contrasena);
                error = MensajeCredenciales;
                return null;
            }

            if (!HashContrasena.Verificar(contrasena, usuario.ContrasenaHash))
            {
                error = MensajeCredenciales;
                return null;
            }

            if (!usuario.Estado)
            {
                error = "El usuario está inactivo. Comuníquese con el área de soporte.";
                return null;
            }

            return usuario;
        }
    }
}
