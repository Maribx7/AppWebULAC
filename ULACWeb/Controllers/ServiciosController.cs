using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;

namespace ULACWeb.Controllers
{
    public class ServiciosController : Controller
    {
        // GET: Servicios
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inicio()
        {// Verificar si el usuario tiene al menos un contrato asociado con su IDEmpresa
            int IDEmpresa = (int)Session["IDEmpresa"];
            ServiciosModel model = new ServiciosModel();
            bool tieneContratos = model.VerificarContratos(IDEmpresa);

            if (!tieneContratos)
            {
                // Redirigir al usuario a la página para contratar un servicio
                return RedirectToAction("Index", "Inicio");
            }

            // Pasar las variables IDEmpresa y tieneContratos a la página de Seguimiento
            TempData["IDEmpresa"] = IDEmpresa;
            TempData["tieneContratos"] = tieneContratos;

            // El usuario tiene contratos, mostrar la página de seguimiento
            return RedirectToAction("Seguimiento", "Home");
        }



    }
}