using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;
using Newtonsoft.Json;

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

                    // Crear la respuesta JSON
                    var response = new
                    {
                        success = true,
                        message = "Inicio de sesión exitoso"
                    };

                   

                    // Convertir la respuesta a JSON
                    var json = JsonConvert.SerializeObject(response);

                    // Devolver la respuesta JSON
                    return Content(json, "application/json");
                }
                else
                {
                    // Las credenciales son inválidas, agregar un mensaje de error y volver a mostrar el formulario de inicio de sesión
                    return Json(new { success = false, message = "El nombre de usuario o la contraseña son incorrectos." });

                }
            }
            else
            {
                // El modelo no es válido, volver a mostrar el formulario de inicio de sesión con los mensajes de validación
                return Json(new { success = false, message = "Por favor, complete todos los campos." });
            }
        }
    }
}
