using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using ULACWeb.Models;
using System.Web.Mvc;
using System.Linq;

namespace ULACWeb.Controllers
{
    public class DetallesOperacionController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("DetallesOperacion", "Home");
        }

        [HttpPost]
        // Método para obtener los datos de seguimiento de forma asíncrona
        public ActionResult ObtenerDetalleOperacion(int IDEmpresa)
        {
            var wsClient = new WSPrueba1.WSSoapClient();
            if (Session["IDEmpresa"] != null)
            {
                IDEmpresa = Convert.ToInt32(Session["IDEmpresa"]);
            }
            
            List<WSPrueba1.DetalleOperacionModel> Detalles = wsClient.ObtenerDetalles(IDEmpresa).ToList();


            return View("~/Views/Home/DetallesOperacion.cshtml", Detalles);
        }
    }
}
