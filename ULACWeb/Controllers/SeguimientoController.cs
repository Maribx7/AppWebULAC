using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;

namespace ULACWeb.Controllers
{
    public class SeguimientoController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Seguimiento", "Home");
        }
        [HttpPost]
        // Método para obtener los datos de seguimiento de forma asíncrona
        public ActionResult ObtenerDatosSeguimiento(int IDEmpresa)
        {
            if (Session["IDEmpresa"] != null)
            {
                IDEmpresa = Convert.ToInt32(Session["IDEmpresa"]);
            }
            SeguimientoModel model = new SeguimientoModel();
            List<SeguimientoModel> seguimientos = model.ObtenerSeguimientoEnvios(IDEmpresa);


            return RedirectToAction("Seguimiento", "Home", new { seguimientos = model });
        }
    }
}
