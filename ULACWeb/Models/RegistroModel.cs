using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using static ULACWeb.Models.RegistroModel;

namespace ULACWeb.Models
{
    public class RegistroModel
    {
        [Required(ErrorMessage = "El ID de su empresa es obligatorio")]
        public int IDEmpresa { get; set; }
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        public string NombreContactoPrincipal { get; set; }

        [Required(ErrorMessage = "El Número de Teléfono es obligatorio")]
        public string NumeroContactoPrincipal { get; set; }
        [Required(ErrorMessage = "El País de Residencia es obligatorio")]
        public string PaisResidencia { get; set; }
        [Required(ErrorMessage = "El Contraseña es obligatorio")]
        public string Contraseña { get; set; }
        [Required(ErrorMessage = "La Usuario es obligatorio")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "El Correo Electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El Correo Electrónico no es válido")]
        public string CorreoContactoPrincipal { get; set; }
        [Required(ErrorMessage = "La selección de la pregunta es obligatoria")]
        public string Pregunta1 { get; set; }
        [Required(ErrorMessage = "La selección de la pregunta es obligatoria")]
        public string Pregunta2 { get; set; }
        [Required(ErrorMessage = "La selección de la pregunta es obligatoria")]
        public string Pregunta3 { get; set; }
        [Required(ErrorMessage = "La respuesta de la pregunta es obligatoria")]
        public string Respuesta1 { get; set; }
        [Required(ErrorMessage = "La respuesta de la pregunta es obligatoria")]
        public string Respuesta2 { get; set; }
        [Required(ErrorMessage = "La respuesta de la pregunta es obligatoria")]
        public string Respuesta3 { get; set; }

        public bool GuardarEnBaseDeDatos()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("InsertarUsuario", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);
                    command.Parameters.AddWithValue("@NombreContactoPrincipal", NombreContactoPrincipal);
                    command.Parameters.AddWithValue("@NumeroContactoPrincipal", NumeroContactoPrincipal);
                    command.Parameters.AddWithValue("@PaisResidencia", PaisResidencia);
                    command.Parameters.AddWithValue("@Contraseña", Contraseña);
                    command.Parameters.AddWithValue("@Usuario", Usuario);
                    command.Parameters.AddWithValue("@CorreoContactoPrincipal", CorreoContactoPrincipal);
                    command.Parameters.AddWithValue("@PreguntaSeguridad1", Pregunta1);
                    command.Parameters.AddWithValue("@PreguntaSeguridad2", Pregunta2);
                    command.Parameters.AddWithValue("@PreguntaSeguridad3", Pregunta3);
                    command.Parameters.AddWithValue("@RespuestaSeguridad1", Respuesta1);
                    command.Parameters.AddWithValue("@RespuestaSeguridad2", Respuesta2);
                    command.Parameters.AddWithValue("@RespuestaSeguridad3", Respuesta3);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra al intentar guardar en la base de datos
                Console.WriteLine("Error al guardar en la base de datos: " + ex.Message);
                return false;
            }
        }
    }

}