using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;
using Newtonsoft.Json;
using static ULACWeb.Models.ActividadModel;

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

            try
            {
                if (ModelState.IsValid)
                {
                    int idEmpresa = 0;

                    if (model.VerificarCredenciales(out idEmpresa))
                    {

                        // Almacenar el IDEmpresa en la sesión
                        Session["IDEmpresa"] = idEmpresa;
                        Actividad actividadLogin = new Actividad
                        {
                            IDEmpresa = idEmpresa, // Necesitarás implementar este método
                           
                            TipoActividad = "Inicio de sesión",
                            Detalles = "Inicio de sesión exitoso",
                            IP = Request.UserHostAddress 
                        };
                        var IDEmpresa = idEmpresa.ToString();
                        RegistroActividadesHelper.RegistrarActividad(actividadLogin);
                        RegistroActividadesHelper.RegistrarUltimoIngreso(IDEmpresa, DateTime.Now);
                        return RedirectToAction("Inicio", "Home");
                    }
                    else
                    {
                        model.VerificarNOCredenciales(out idEmpresa);
                        Actividad actividadLogin = new Actividad
                        {
                            IDEmpresa = idEmpresa, // Necesitarás implementar este método

                            TipoActividad = "Inicio de sesión fallido",
                            Detalles = "Inicio de sesión fallido. El nombre de usuario o la contraseña son incorrectos.",
                            IP = Request.UserHostAddress
                        };

                        var IDEmpresa = idEmpresa.ToString();
                        RegistroActividadesHelper.actividadLoginFallido(IDEmpresa);
                        RegistroActividadesHelper.RegistrarActividad(actividadLogin);
                        if (RegistroActividadesHelper.EstaUsuarioBloqueado(idEmpresa))
                        {
                            return RedirectToAction("ErrorBloqueado", "Home");
                        }
                        return Json(new { success = false, message = "El nombre de usuario o la contraseña son incorrectos." });

                    }
                }
                else
                {
                    
                    return Json(new { success = false, message = "Por favor, complete todos los campos." });
                }
            }
            catch (Exception ex)
            {
              
                return Json(new { success = false, message = "Ocurrió un error inesperado. Por favor, intente de nuevo más tarde." });
            }
        }


    }


}
