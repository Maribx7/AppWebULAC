using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;

namespace ULACWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                int idEmpresa;
                if (model.VerificarCredenciales(out idEmpresa))
                {
                    // Almacenar el IDEmpresa en la sesión
                    Session["IDEmpresa"] = idEmpresa;
                    

                    return RedirectToAction("Inicio", "Home");
                }
                else
                {
                    // Las credenciales son inválidas, agregar un mensaje de error y volver a mostrar el formulario de inicio de sesión
                    ModelState.AddModelError("", "El nombre de usuario o la contraseña son incorrectos.");
                    return View(model);
                }
            }
            else
            {
                // El modelo no es válido, volver a mostrar el formulario de inicio de sesión con los mensajes de validación
                return View(model);
            }
        }
    }
}