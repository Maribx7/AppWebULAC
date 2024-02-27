using Microsoft.Win32;
using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ULACWeb.Models;

namespace ULACWeb.Controllers
{
    public class RegistroController : Controller
    {
        // Acción para mostrar el formulario de registro
        public ActionResult Index()
        {
            return View();
        }

        // Acción para procesar el formulario de registro
        [HttpPost]
        public ActionResult Index(RegistroModel model)
        {
            // Aquí puedes realizar la lógica para guardar en la base de datos
            if (model.GuardarEnBaseDeDatos())
            {
                return PartialView("_PopupRegistroExitoso");
            }

            // Siempre regresa a la vista del formulario, independientemente del resultado del guardado en la base de datos
            return View("~/Views/Home/Registro.cshtml");
        }

    }
}
