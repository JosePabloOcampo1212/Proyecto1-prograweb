using System;
using System.Collections.Generic;
using HelpDesk.Models;

namespace HelpDesk.Negocio
{
   
    public static class FlujoEstados
    {
        private static readonly Dictionary<int, int[]> Transiciones = new Dictionary<int, int[]>
        {
            { Estados.Abierto,   new[] { Estados.EnProceso } },
            { Estados.EnProceso, new[] { Estados.Resuelto, Estados.Abierto } },
            { Estados.Resuelto,  new[] { Estados.Cerrado, Estados.EnProceso } },
            { Estados.Cerrado,   new int[0] }
        };

        public static int[] Siguientes(int idEstadoActual)
        {
            int[] siguientes;
            return Transiciones.TryGetValue(idEstadoActual, out siguientes) ? siguientes : new int[0];
        }

        public static bool EsPermitida(int idEstadoActual, int idEstadoNuevo)
        {
            return Array.IndexOf(Siguientes(idEstadoActual), idEstadoNuevo) >= 0;
        }
    }
}
