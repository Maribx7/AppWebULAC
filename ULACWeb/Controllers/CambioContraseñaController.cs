using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ULACWeb.Models
{
    public class CambioContraseñaController : Controller
    {
        // GET: CambioContraseña
        public ActionResult Index()
        {
            return View();
        }

        // POST: CambioContraseña/Cambiar
        [HttpPost]
        public ActionResult CambioContraseña(string IDUsuario, string NuevaContraseña, string correo)
        {
            try
            {
                if (string.IsNullOrEmpty(IDUsuario) || string.IsNullOrEmpty(NuevaContraseña) || string.IsNullOrEmpty(correo))
                {
                    ViewBag.Error = "El usuario y la nueva contraseña son requeridos.";
                    return RedirectToAction("Index", "Home");
                }

                CambioContraseñaModel cambioModel = new CambioContraseñaModel();
                bool resultado = cambioModel.CambioContraseña(IDUsuario, NuevaContraseña, correo);

                if (resultado)
                {
                    ViewBag.Message = "La contraseña ha sido cambiada con éxito.";
                }
                else
                {
                    ViewBag.Error = "Ha ocurrido un error al intentar cambiar la contraseña.";
                }
            }
            catch (Exception ex)
            {
                // Log ex o manejo de excepciones según sea necesario.
                ViewBag.Error = "Ha ocurrido un error interno.";
            }

            return View("Index");
        }
    }
}