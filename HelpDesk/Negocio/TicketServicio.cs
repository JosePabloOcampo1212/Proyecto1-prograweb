using System;
using System.Collections.Generic;
using System.Linq;
using HelpDesk.Datos;
using HelpDesk.Models;
using HelpDesk.Models.Entidades;
using HelpDesk.Models.ViewModels;
using HelpDesk.Seguridad;

namespace HelpDesk.Negocio
{
   
    public class TicketServicio
    {
        private readonly TicketRepositorio _tickets = new TicketRepositorio();
        private readonly UsuarioRepositorio _usuarios = new UsuarioRepositorio();
        private readonly EstadoTicketRepositorio _estados = new EstadoTicketRepositorio();

      
        public List<Ticket> Buscar(UsuarioPrincipal usuario, string texto, int? idEstado)
        {
            int? idSolicitante = usuario.EsAdministrador ? (int?)null : usuario.Id;
            string patron = null;
            int? numero = null;

            if (!string.IsNullOrWhiteSpace(texto))
            {
                string limpio = texto.Trim();
                if (limpio.Length > 100)
                {
                    limpio = limpio.Substring(0, 100);
                }

                int valor;
                if (int.TryParse(limpio.TrimStart('#'), out valor) && valor > 0)
                {
                    numero = valor;
                }

                patron = "%" + EscaparLike(limpio) + "%";
            }

            return _tickets.Buscar(idSolicitante, idEstado, patron, numero);
        }

      
        public Ticket ObtenerConAcceso(int idTicket, UsuarioPrincipal usuario)
        {
            var ticket = _tickets.ObtenerPorId(idTicket);
            if (ticket == null)
            {
                throw new RecursoNoEncontradoException("El ticket #" + idTicket + " no existe.");
            }

            if (!usuario.EsAdministrador && ticket.IdUsuarioSolicitante != usuario.Id)
            {
                throw new AccesoDenegadoException("No tiene permiso para ver este ticket. Solo puede consultar los tickets que usted registró.");
            }

            return ticket;
        }

        public List<EstadoTicket> ListarEstados()
        {
            return _estados.Listar();
        }

        public List<Usuario> ListarPersonalSoporte()
        {
            return _usuarios.ListarPersonalSoporte();
        }

        public List<EstadoTicket> EstadosSiguientes(int idEstadoActual)
        {
            var todos = _estados.Listar();
            return FlujoEstados.Siguientes(idEstadoActual)
                .Select(id => todos.First(e => e.Id == id))
                .ToList();
        }

        public bool PuedeEditar(Ticket ticket, UsuarioPrincipal usuario)
        {
            return ticket.IdUsuarioSolicitante == usuario.Id && ticket.IdEstado == Estados.Abierto;
        }

       
        public int Crear(TicketFormViewModel datos, UsuarioPrincipal usuario)
        {
            ValidarDatos(datos);
            var ahora = DateTime.Now;

            var ticket = new Ticket
            {
                Titulo = datos.Titulo.Trim(),
                Descripcion = datos.Descripcion.Trim(),
                Categoria = datos.Categoria,
                IdUsuarioSolicitante = usuario.Id,
                IdEstado = Estados.Abierto,
                FechaCreacion = ahora,
                FechaActualizacion = ahora
            };

            return _tickets.Insertar(ticket);
        }

      
        public void Editar(int idTicket, TicketFormViewModel datos, UsuarioPrincipal usuario)
        {
            var ticket = ObtenerConAcceso(idTicket, usuario);

            if (ticket.IdUsuarioSolicitante != usuario.Id)
            {
                throw new AccesoDenegadoException("Solo la persona que registró el ticket puede editarlo.");
            }
            if (ticket.IdEstado != Estados.Abierto)
            {
                throw new ReglaNegocioException("Solo se pueden editar tickets en estado Abierto.");
            }

            ValidarDatos(datos);
            ticket.Titulo = datos.Titulo.Trim();
            ticket.Descripcion = datos.Descripcion.Trim();
            ticket.Categoria = datos.Categoria;
            ticket.FechaActualizacion = DateTime.Now;

            if (_tickets.ActualizarDatos(ticket) == 0)
            {
                throw new ReglaNegocioException("El ticket cambió de estado mientras se editaba. Recargue la página.");
            }
        }

        
        public Ticket Asignar(int idTicket, int idUsuarioAsignado, UsuarioPrincipal administrador)
        {
            ExigirAdministrador(administrador);
            var ticket = ObtenerConAcceso(idTicket, administrador);

            if (ticket.IdEstado == Estados.Cerrado)
            {
                throw new ReglaNegocioException("No se puede reasignar un ticket cerrado.");
            }
            if (ticket.IdUsuarioAsignado == idUsuarioAsignado)
            {
                throw new ReglaNegocioException("El ticket ya está asignado a esa persona.");
            }

            var tecnico = _usuarios.ObtenerPorId(idUsuarioAsignado);
            if (tecnico == null || !tecnico.Estado || !tecnico.EsAdministrador)
            {
                throw new ReglaNegocioException("Solo se puede asignar el ticket a personal de soporte activo.");
            }

            if (_tickets.ActualizarAsignacion(idTicket, idUsuarioAsignado, DateTime.Now) == 0)
            {
                throw new ReglaNegocioException("No se pudo asignar el ticket. Recargue la página e intente de nuevo.");
            }

            return _tickets.ObtenerPorId(idTicket);
        }

      
        public Ticket CambiarEstado(int idTicket, int idEstadoNuevo, UsuarioPrincipal administrador)
        {
            ExigirAdministrador(administrador);
            var ticket = ObtenerConAcceso(idTicket, administrador);

            if (ticket.IdEstado == Estados.Cerrado)
            {
                throw new ReglaNegocioException("El ticket está cerrado y ya no admite cambios de estado.");
            }
            if (!FlujoEstados.EsPermitida(ticket.IdEstado, idEstadoNuevo))
            {
                throw new ReglaNegocioException("No se permite ese cambio desde el estado " + ticket.NombreEstado + ".");
            }
            if (idEstadoNuevo == Estados.EnProceso && !ticket.IdUsuarioAsignado.HasValue)
            {
                throw new ReglaNegocioException("Asigne el ticket a una persona de soporte antes de pasarlo a En Proceso.");
            }

            var ahora = DateTime.Now;
            DateTime? fechaResolucion = ticket.FechaResolucion;

            if (idEstadoNuevo == Estados.Resuelto)
            {
                fechaResolucion = ahora;
            }
            else if (idEstadoNuevo == Estados.EnProceso || idEstadoNuevo == Estados.Abierto)
            {
               
                fechaResolucion = null;
            }
           

            if (_tickets.ActualizarEstado(idTicket, ticket.IdEstado, idEstadoNuevo, fechaResolucion, ahora) == 0)
            {
                throw new ReglaNegocioException("Otra persona modificó el ticket hace un momento. Recargue la página.");
            }

            return _tickets.ObtenerPorId(idTicket);
        }

       
        public void Eliminar(int idTicket, UsuarioPrincipal administrador)
        {
            ExigirAdministrador(administrador);
            ObtenerConAcceso(idTicket, administrador);
            _tickets.Eliminar(idTicket);
        }

     

        private static void ValidarDatos(TicketFormViewModel datos)
        {
            string titulo = (datos.Titulo ?? string.Empty).Trim();
            string descripcion = (datos.Descripcion ?? string.Empty).Trim();

            if (titulo.Length < 5 || titulo.Length > 150)
            {
                throw new ReglaNegocioException("El título debe tener entre 5 y 150 caracteres.");
            }
            if (descripcion.Length < 10 || descripcion.Length > 4000)
            {
                throw new ReglaNegocioException("La descripción debe tener entre 10 y 4000 caracteres.");
            }
            if (Array.IndexOf(Categorias.Lista, datos.Categoria) < 0)
            {
                throw new ReglaNegocioException("La categoría seleccionada no es válida.");
            }
        }

        internal static void ExigirAdministrador(UsuarioPrincipal usuario)
        {
            if (usuario == null || !usuario.EsAdministrador)
            {
                throw new AccesoDenegadoException("Esta acción es exclusiva del rol Administrador.");
            }
        }

       
        private static string EscaparLike(string texto)
        {
            return texto.Replace("\\", "\\\\")
                        .Replace("%", "\\%")
                        .Replace("_", "\\_")
                        .Replace("[", "\\[");
        }
    }
}
