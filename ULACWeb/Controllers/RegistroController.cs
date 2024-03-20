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

namespace ULACWeb.Controllers
{
    public class RegistroController : Controller
    {

        private RegistroModel model = new RegistroModel();

        public ActionResult Index()
        {

            
            return View();
        }

        // Acción para procesar el formulario de registro
        [HttpPost]
        public ActionResult Registrar(RegistroModel model)
        {
            {
               

                var resultado = model.EnviodeCorreoVerificacion();
                if (resultado)
                {
                    return RedirectToAction("EnvioCorreo", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
             

        }
        public ActionResult Verificar(string uid, RegistroModel model)
        {

            if (model != null)
            {
                if (model.GuardarDatosVerificados(uid))
                {
                    ViewBag.MensajeExito = "Verificación completada con éxito. Ahora puede iniciar sesión.";
                    return RedirectToAction("Login", "Home");

                }
            }

            ViewBag.MensajeError = "Error durante la verificación. Por favor, intente de nuevo.";
            return View(); // Asegúrate de manejar correctamente esta situación, posiblemente mostrando un mensaje de error.
        }

        public ActionResult GetPaises()
        {
            var paises = model.GetPaises(); 
            return Json(new SelectList(paises, "IDPais", "NombrePais"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProvinciasByPaisId(int paisId)
        {
            var provincias = model.GetProvincias(paisId); 
            return Json(new SelectList(provincias, "IDProvincia", "NombreProvincia"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCantonesByProvinciaId(int provinciaId)
        {
            var cantones = model.GetCanton(provinciaId); 
            return Json(new SelectList(cantones, "IDCanton", "NombreCanton"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDistritosByCantonId(int cantonId)
        {
            var distritos = model.GetDistrito(cantonId); 
            return Json(new SelectList(distritos, "IDDistrito", "NombreDistrito"), JsonRequestBehavior.AllowGet);
        }

    }
    }
