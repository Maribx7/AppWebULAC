using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace ULACWeb.Models
{
    public class RestablecerContraseñaController : Controller
    {

        // GET: RecuperarContraseña
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EnviarInstrucciones(string correo)
        {
            var wsClient = new WSPrueba1.WSSoapClient();

            if (wsClient.EnviarInstruccion(correo))
            {
                ViewBag.Instrucciones = "Hemos enviado un correo electrónico a la dirección proporcionada. Si no lo recibes en breve, es posible que no estés registrado(a) o que haya algún error en la dirección de correo electrónico ingresada. Por favor, verifica tus datos y vuelve a intentarlo.\r\n\r\n";

            }
            else
            {
                ViewBag.Instrucciones = "Hubo un error al enviar el correo. Por favor verifica que el correo ingresado es correcto y vuelve a intentarlo.";
            }
            return View("~/Views/Home/RestablecerContraseña.cshtml");

        }
    }
}