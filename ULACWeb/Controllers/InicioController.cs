using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;

namespace ULACWeb.Controllers
{
    public class InicioController : Controller
    {
        // GET: Inicio
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ConfirmarServicio(int IDEmpresa, string Origen, string Destino, string TiempoEstimado, decimal Subtotal, string MetodoPago, string NumeroTarjeta, string FechaVencimiento, string CodigoSeguridad, decimal Total, int IDTipoPaquete)
        {
            // Crear un objeto de contrato con los datos recibidos del formulario
            InicioModel.Contrato contrato = new InicioModel.Contrato
            {
                IDEmpresa = IDEmpresa,
                IDTipoPaquete = IDTipoPaquete,
                Origen = Origen,
                Destino = Destino,
                TiempoEstimado = TiempoEstimado,
                Subtotal = Subtotal,
                MetodoPago = MetodoPago,
                NumeroTarjeta = NumeroTarjeta,
                FechaVencimiento = FechaVencimiento,
                CodigoSeguridad = CodigoSeguridad,
                Total = Total
            };

            // Guardar el contrato en la base de datos
            contrato.Guardar();

            // Redirigir a la página de pago
            return RedirectToAction("Inicio");
        }

    }
}