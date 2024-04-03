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
        public async Task<ActionResult> ConfirmarServicio(int IDEmpresa, string Identificacion, string Origen, string Destino, string TiempoEstimado, string Industria, float Subtotal, string MetodoPago, string NumeroTarjeta, string FechaVencimiento, string CodigoSeguridad, float Total, int IDTipoPaquete)
        {
            // Preparar los datos para enviar en la solicitud POST
            var requestData = new
            {
                IDEmpresa,
                Identificacion,
                Origen,
                Destino,
                TiempoEstimado,
                Industria,
                Subtotal,
                MetodoPago,
                NumeroTarjeta,
                FechaVencimiento,
                CodigoSeguridad,
                Total,
                IDTipoPaquete
            };
            var json = JsonConvert.SerializeObject(requestData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Crear el HttpClient y hacer la solicitud al servicio web
            using (var client = new HttpClient())
            {
                // Asegúrate de cambiar la URL al endpoint correcto de tu servicio web Python
                var response = await client.PostAsync("http://localhost:5000/verificar_y_cargar_tarjeta", data);
                string resultContent = await response.Content.ReadAsStringAsync();

                // Aquí puedes manejar la respuesta. Por ejemplo, verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Operación exitosa, haz algo aquí, como redirigir al usuario o mostrar un mensaje
                    return RedirectToAction("Inicio", "Home");
                }
                else
                {
                    // Manejar el caso de error
                    ViewBag.ErrorMessage = "Hubo un problema al procesar el pago.";
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