using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ULACWeb.Models;
using System.Security.Cryptography;
using System.Data.Entity;
using System.Collections.Generic;
using static ULACWeb.Models.RegistroModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace ULACWeb.Controllers
{
    public class RegistroController : Controller
    {



        private RegistroModel model = new RegistroModel();
        public class Persona
        {
            public string Nombre { get; set; }
            public string Identificacion { get; set; }
            public string Apellidos { get; set; }
            public string Telefono { get; set; }
            public string Pais { get; set; }
            public string Provincia { get; set; }
            public string Canton { get; set; }
            public string Distrito { get; set; }
        }
        public ActionResult Index()
        {
            return View();
        }


        //Trae segun la identificacion los datos de la persona
        [HttpPost]
        public async Task<JsonResult> BuscarPersona(string identificacion)
        {
            var baseUrl = "http://localhost:5000"; 
            var url = $"{baseUrl}/persona?identificacion={identificacion}";

            using (var client = new HttpClient()) 
            {
                try
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var persona = JsonConvert.DeserializeObject<Persona>(responseBody);

                        return Json(persona);
                    }
                    else
                    {
                      
                        return Json(new { mensaje = "Persona no encontrada" });
                    }
                }
                catch (Exception ex)
                {
                    
                    return Json(new { mensaje = ex.Message });
                }
            }
        }


                // Acción para procesar el formulario de registro
                [HttpPost]
        public ActionResult Registrar(string Correo, string Nombre, string IDEmpresa, string Identificacion, string Telefono, string NombrePais, string NombreProvincia, string NombreCanton, string NombreDistrito, string Contraseña, string Pregunta1, string Pregunta2, string Pregunta3, string Respuesta1, string Respuesta2, string Respuesta3)
        {
            {
                var wsClient = new WSPrueba1.WSSoapClient();

                var resultado = wsClient.EnviodeCorreoVerificacion(Correo, Nombre, IDEmpresa,Identificacion,Telefono,NombrePais,NombreProvincia,NombreCanton,NombreDistrito,Contraseña,Pregunta1,Pregunta2,Pregunta3,Respuesta1,Respuesta2,Respuesta3);
                if (resultado)
                {
                    return RedirectToAction("EnvioCorreo", "Home");

                }
                else
                {
                    ViewBag.ErrorRegistro = "Ya existe un usuario registrado con la información proporcionada.";
                    return View("~/Views/Home/Registro.cshtml"); ;
                }
            }
        }

        public ActionResult Verificar(string uid)
        {
            var wsClient = new WSPrueba1.WSSoapClient();
            
                if (wsClient.GuardarDatosVerificados(uid))
                {
                    ViewBag.MensajeExito = "Verificación completada con éxito. Ahora puede iniciar sesión.";
                    return RedirectToAction("Login", "Home");

                }
            
                                                            
            ViewBag.MensajeError = "Error durante la verificación. Por favor, intente de nuevo.";
            return View();
        }

        public ActionResult GetPaises()
        {
            var wsClient = new WSPrueba1.WSSoapClient();
            var paises = wsClient.GetPaises(); 
            return Json(new SelectList(paises, "IDPais", "NombrePais"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProvinciasByPaisId(int paisId)
        {
            var wsClient = new WSPrueba1.WSSoapClient();
            var provincias = wsClient.GetProvincias(paisId); 
            return Json(new SelectList(provincias, "IDProvincia", "NombreProvincia"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCantonesByProvinciaId(int provinciaId)
        {
            var wsClient = new WSPrueba1.WSSoapClient();
            var cantones = wsClient.GetCanton(provinciaId); 
            return Json(new SelectList(cantones, "IDCanton", "NombreCanton"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDistritosByCantonId(int cantonId)
        {
            var wsClient = new WSPrueba1.WSSoapClient();
            var distritos = wsClient.GetDistrito(cantonId); 
            return Json(new SelectList(distritos, "IDDistrito", "NombreDistrito"), JsonRequestBehavior.AllowGet);
        }

    }
    }
