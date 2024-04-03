using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;

namespace ULACWeb.Controllers
{
    public class PerfilController : Controller
    {
        // GET: Perfil
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerDatosCliente(int IDEmpresa)
        {
            var wsClient = new WSPrueba1.WSSoapClient();
            List<WSPrueba1.PerfilModel> listaClientes = wsClient.ObtenerDatosCliente(IDEmpresa).ToList();

            // Devolver los datos del cliente a la vista correspondiente
            return View("~/Views/Home/Perfil.cshtml", listaClientes);
        }
    }
}
