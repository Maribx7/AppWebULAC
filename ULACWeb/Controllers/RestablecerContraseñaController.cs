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
        public ActionResult EnviarInstrucciones(RestablecerContraseñaModel model, string correo)
        {
            model.EnviarInstruccion();
            return RedirectToAction("Login","Home");
        }

    }
}