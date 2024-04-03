using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Tokens;
using ULACWeb.Models;
using static ULACWeb.Models.PreguntasSeguridadModel;

namespace ULACWeb.Controllers
{
    public class PreguntasSeguridadController : Controller
    {

        PreguntasSeguridadModel model = new PreguntasSeguridadModel();
        // GET: PreguntasSeguridad
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PreguntasSeguridad(string uid)
        {
            var wsClient = new WSPrueba1.WSSoapClient();

            var handler = new JwtSecurityTokenHandler();
            string correo = null;
            var tokenS = handler.ReadToken(uid) as JwtSecurityToken;
            if (tokenS != null)
            {
                correo = tokenS.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            }

            var pregunta = wsClient.ObtenerPreguntaSeguridadAleatoria(uid);
            if (pregunta == null)
            {
                return RedirectToAction("Login", "Home");
            }
         
            return View("~/Views/Home/PreguntasSeguridad.cshtml");

        }


        [HttpPost]
        public ActionResult ValidarRespuestas(string IDUsuario, int IDPregunta, string respuesta, string correo)
        {

            
            TempData["IDUsuario"] = IDUsuario;
            TempData["Correo"] = correo;
            bool esCorrecta = model.ValidarRespuestaSeguridad(IDUsuario, IDPregunta, respuesta);
            
            if (esCorrecta)
            {

                return RedirectToAction("CambioContraseña", "Home", new { IDUsuario = IDUsuario, Correo = correo });

            }
            else
            {
              
                TempData["Error"] = "La respuesta ingresada no es correcta. Por favor, intente nuevamente.";
                return RedirectToAction("PreguntasSeguridad", "Home");
            }
        }


    }
}