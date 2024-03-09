using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ULACWeb.Models;
using System.Security.Cryptography;

namespace ULACWeb.Controllers
{
    public class RegistroController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        // Acción para procesar el formulario de registro
        [HttpPost]
        public JsonResult Registrar(RegistroModel model)
        {
            if (ModelState.IsValid)
            {
                var resultado = model.GuardarEnBaseDeDatos();
                if (resultado)
                {
                    return Json(new { success = true, message = "Se ha enviado un correo electrónico de verificación a su dirección. Haga clic en el enlace para completar el registro." });
                }
                else
                {
                    // Si hubo un error al guardar en la base de datos o el correo ya estaba registrado
                    return Json(new { success = false, message = "Algunos campos del formulario no son válidos. Por favor, revise e intente nuevamente." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Algunos campos del formulario no son válidos. Por favor, revise e intente nuevamente." });
            }

        }
        public ActionResult VerificarCorreo(string uid)
        {
            if (uid != null && !string.IsNullOrEmpty(uid))
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("VerificarCorreo", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        // Pasamos el UID en lugar del Token directamente.
                        command.Parameters.AddWithValue("@UID", uid);

                        var reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            if (reader.GetInt32(0) == 1) // Asegúrate de que el SP devuelve 1 para éxito
                            {
                                TempData["MensajeExito"] = "Su correo ha sido verificado con éxito. Puede realizar el inicio de sesión.";
                                TempData.Keep("MensajeExito");
                                return RedirectToAction("Login", "Home"); 
                            }
                            else
                            {
                                ViewBag.MensajeError = reader.GetString(1); // Mensaje de error del SP
                            }
                        }
                    }
                 
                }
                catch (Exception ex)
                {
                    ViewBag.MensajeError = "Error al verificar el correo electrónico: " + ex.Message;
                    return Content(ViewBag.MensajeError);
                }
            }

            ViewBag.MensajeError = "El token de verificación no es válido.";
            return Content(ViewBag.MensajeError);
        }
    }
}
