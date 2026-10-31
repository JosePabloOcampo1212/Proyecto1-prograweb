using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HelpDesk.Models.ViewModels;
using HelpDesk.Negocio;
using HelpDesk.Seguridad;

namespace HelpDesk.Controllers
{

    public class CuentaController : BaseController
    {
        private readonly AutenticacionServicio _autenticacion = new AutenticacionServicio();

        // GET: /Cuenta/Login
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (UsuarioActual != null)
            {
                return RedirectToAction("Index", "Tickets");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Cuenta/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            string error;
            var usuario = _autenticacion.ValidarCredenciales(modelo.Email, modelo.Contrasena, out error);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(modelo);
            }

            
            var ticket = new FormsAuthenticationTicket(
                1,
                usuario.Email,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                UsuarioPrincipal.CrearDatosCookie(usuario),
                FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };
            Response.Cookies.Add(cookie);

            
            if (!string.IsNullOrEmpty(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
            {
                return Redirect(modelo.ReturnUrl);
            }
            return RedirectToAction("Index", "Tickets");
        }

        // POST: /Cuenta/Salir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salir()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
