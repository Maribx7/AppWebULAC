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
        public ActionResult Index(string Usuario,string Contraseña, string Token)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    
                    int idEmpresa = 0;
                    var wsClient = new WSPrueba1.WSSoapClient();

                    if (wsClient.VerificarCredenciales(Usuario, Contraseña, Token, out idEmpresa))
                    {

                        // Almacenar el IDEmpresa en la sesión
                        Session["IDEmpresa"] = idEmpresa;
                        WSPrueba1.Actividad actividadLogin = new WSPrueba1.Actividad
                        {
                            Identificacion = Usuario,
                            TipoActividad = "Inicio de sesión",
                            Detalles = "Inicio de sesión exitoso",
                            FechaHora = DateTime.Now,
                            IP = Request.UserHostAddress 
                        };
                        var IDEmpresa = idEmpresa.ToString();
                        wsClient.RegistrarActividad(actividadLogin);
                        wsClient.RegistrarUltimoIngreso(IDEmpresa, DateTime.Now);
                        return RedirectToAction("Inicio", "Home");
                    }
                    else
                    {
                        wsClient.VerificarNOCredenciales(Usuario, Contraseña, Token, out idEmpresa);
                        WSPrueba1.Actividad actividadLogin = new WSPrueba1.Actividad
                        {
                            Identificacion = Usuario, // Necesitarás implementar este método
                            FechaHora = DateTime.Now,
                            TipoActividad = "Inicio de sesión fallido",
                            Detalles = "Inicio de sesión fallido. El nombre de usuario o la contraseña son incorrectos.",
                            IP = Request.UserHostAddress
                        };

                        var IDEmpresa = idEmpresa.ToString();
                        wsClient.actividadLoginFallido(Usuario.ToString(), 4);
                        wsClient.RegistrarActividad(actividadLogin);
                        if (wsClient.EstaUsuarioBloqueado(idEmpresa))
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
            catch (Exception)
            {
              
                return Json(new { success = false, message = "Ocurrió un error inesperado. Por favor, intente de nuevo más tarde." });
            }
        }


    }


}
