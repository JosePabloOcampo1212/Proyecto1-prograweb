using System;
using System.Security.Cryptography;

namespace HelpDesk.Seguridad
{
   
    public static class HashContrasena
    {
        private const string Prefijo = "PBKDF2-SHA256";
        private const int Iteraciones = 600000;
        private const int BytesSal = 16;
        private const int BytesHash = 32;

        public static string Generar(string contrasena)
        {
            if (contrasena == null)
            {
                throw new ArgumentNullException("contrasena");
            }

            var sal = new byte[BytesSal];
            using (var generador = RandomNumberGenerator.Create())
            {
                generador.GetBytes(sal);
            }

            byte[] hash = Derivar(contrasena, sal, Iteraciones, BytesHash);
            return Prefijo + "$" + Iteraciones + "$" + Convert.ToBase64String(sal) + "$" + Convert.ToBase64String(hash);
        }

        public static bool Verificar(string contrasena, string valorGuardado)
        {
            if (contrasena == null || string.IsNullOrEmpty(valorGuardado))
            {
                return false;
            }

            string[] partes = valorGuardado.Split('$');
            if (partes.Length != 4 || partes[0] != Prefijo)
            {
                return false;
            }

            int iteraciones;
            if (!int.TryParse(partes[1], out iteraciones) || iteraciones <= 0)
            {
                return false;
            }

            byte[] sal;
            byte[] esperado;
            try
            {
                sal = Convert.FromBase64String(partes[2]);
                esperado = Convert.FromBase64String(partes[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] calculado = Derivar(contrasena, sal, iteraciones, esperado.Length);
            return SonIguales(calculado, esperado);
        }

      
        public static void SimularVerificacion(string contrasena)
        {
            Derivar(contrasena ?? string.Empty, new byte[BytesSal], Iteraciones, BytesHash);
        }

        private static byte[] Derivar(string contrasena, byte[] sal, int iteraciones, int largo)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(contrasena, sal, iteraciones, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(largo);
            }
        }

       
        private static bool SonIguales(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int diferencia = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }
            return diferencia == 0;
        }
    }
}
