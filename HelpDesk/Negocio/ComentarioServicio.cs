using System;
using System.Collections.Generic;
using HelpDesk.Datos;
using HelpDesk.Models;
using HelpDesk.Models.Entidades;
using HelpDesk.Seguridad;

namespace HelpDesk.Negocio
{

    public class ComentarioServicio
    {
        public const int LargoMaximo = 1000;

        private readonly ComentarioRepositorio _comentarios = new ComentarioRepositorio();
        private readonly TicketServicio _tickets = new TicketServicio();

      
        public List<Comentario> Listar(int idTicket, UsuarioPrincipal usuario)
        {
            _tickets.ObtenerConAcceso(idTicket, usuario);
            return _comentarios.ListarPorTicket(idTicket);
        }

     
        public Comentario Agregar(int idTicket, string texto, UsuarioPrincipal usuario)
        {
            var ticket = _tickets.ObtenerConAcceso(idTicket, usuario);

            if (!PuedeComentar(ticket))
            {
                throw new ReglaNegocioException("El ticket está cerrado y ya no admite comentarios.");
            }

            string limpio = (texto ?? string.Empty).Trim();
            if (limpio.Length == 0)
            {
                throw new ReglaNegocioException("Escriba el comentario antes de enviarlo.");
            }
            if (limpio.Length > LargoMaximo)
            {
                throw new ReglaNegocioException("El comentario no puede superar los " + LargoMaximo + " caracteres.");
            }

            var comentario = new Comentario
            {
                IdTicket = idTicket,
                IdUsuario = usuario.Id,
                Texto = limpio,
                Fecha = DateTime.Now
            };

            int id = _comentarios.Insertar(comentario);
            return _comentarios.ObtenerPorId(id);
        }

        public static bool PuedeComentar(Ticket ticket)
        {
            return ticket.IdEstado != Estados.Cerrado;
        }
    }
}
