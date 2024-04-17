using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ULACWeb.Models;
using System.Xml.Linq;
using static ULACWeb.Models.InicioModel;
using System.Threading.Tasks;
using System.Net.Http;
using System.Data;
using System.IO;
using System.Globalization;
using System.Xml;
using Newtonsoft.Json;
using System.Text;

namespace ULACWeb.Controllers
{
    public class InicioController : Controller
    {
        private readonly HttpClient _client = new HttpClient();
        // GET: Inicio
        public ActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        public void ConfirmarServicio(int IDEmpresa, string Identificacion, string Origen, string Destino, string TiempoEstimado, float Subtotal, string MetodoPago, string NumeroTarjeta, string FechaVencimiento, string CodigoSeguridad, float Total, int IDTipoPaquete)
        {
            var wsClient = new WSPrueba1.WSSoapClient();
            wsClient.Guardar(IDEmpresa, Identificacion, Origen, Destino, TiempoEstimado, Subtotal, MetodoPago, NumeroTarjeta, FechaVencimiento, CodigoSeguridad, Total, IDTipoPaquete);

        }

        private async Task<ActionResult> GuardarContrato(int IDEmpresa, int IDTipoPaquete, string Origen, string Destino, string TiempoEstimado, string Industria, float Subtotal, string MetodoPago, string NumeroTarjeta, string FechaVencimiento, string CodigoSeguridad, float Total)
        {
            var requestData = new
            {
                IDEmpresa,
                IDTipoPaquete,
                Origen,
                Destino,
                TiempoEstimado,
                Industria,
                Subtotal,
                MetodoPago,
                NumeroTarjeta,
                FechaVencimiento,
                CodigoSeguridad,
                Total
            };
            var json = JsonConvert.SerializeObject(requestData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                // Asegúrate que la URL apunte al método Guardar de tu servicio .asmx
                var response = await client.PostAsync("http://localhost:port/NombreDelServicio.asmx/Guardar", data);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Inicio", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "No se pudo guardar el contrato.";
                    return View("Error");
                }
            }
        }

        public async Task<ActionResult> ObtenerTipoCambio()
        {
            try
            {
                string indicador = "317"; // Código para el tipo de cambio de USD a CRC
                string fecha = DateTime.Now.ToString("yyyy/MM/dd"); // Formato de fecha corregido
                string nombre = "ULAC";
                string subNiveles = "N";
                string correoElectronico = "marilyn030599@gmail.com";
                string token = "UA7AI9LAI8";

                string url = $"https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS/wsindicadoreseconomicos.asmx/ObtenerIndicadoresEconomicosXML?Indicador={indicador}&FechaInicio={fecha}&FechaFinal={fecha}&Nombre={nombre}&SubNiveles={subNiveles}&CorreoElectronico={correoElectronico}&Token={token}";

                HttpResponseMessage response = await _client.GetAsync(url); // Usando await para operaciones asincrónicas
                response.EnsureSuccessStatusCode();

                string xmlResponse = await response.Content.ReadAsStringAsync(); // Usando await para operaciones asincrónicas
                string decodedXml = System.Net.WebUtility.HtmlDecode(xmlResponse);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(decodedXml);

                // Crear un XmlNamespaceManager para poder usar el espacio de nombres en la consulta XPath
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("def", "http://ws.sdde.bccr.fi.cr"); // "def" es un prefijo arbitrario para el espacio de nombres

                // Usar el XmlNamespaceManager en la consulta XPath
                XmlNode valorNode = xmlDoc.SelectSingleNode("//def:Datos_de_INGC011_CAT_INDICADORECONOMIC/def:INGC011_CAT_INDICADORECONOMIC/def:NUM_VALOR", nsmgr);
                decimal valor = 0;
                if (valorNode != null)
                {
                    valor = decimal.Parse(valorNode.InnerText, CultureInfo.InvariantCulture);
                }

                return Json(new { success = true, tipoCambio = valor }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}