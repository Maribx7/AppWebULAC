using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using static ULACWeb.Models.RegistroModel;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;

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
        public string CorreoContactoPrincipal { get; set; }
        public bool IsVerified { get; set; }
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

        public string VerificationToken { get; set; }
        public Guid VerificationUID { get; set; }
        public string MensajeGeneral { get; set; }

        //DATOS PARA EL MAIL
        public string Destinatario { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }


        public bool GuardarEnBaseDeDatos()
        {
            try
            {
                if (UsuarioYaRegistrado(CorreoContactoPrincipal))
                {
                    MensajeGeneral = "El correo electrónico ya está registrado.";
                    return false;
                    
                }

                // Genera un token de verificación único y un UID
                VerificationToken = Guid.NewGuid().ToString();
                VerificationUID = Guid.NewGuid(); // Genera el UID

                // Genera un token de verificación único
                
                var protectedToken = Convert.ToBase64String(
                  ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(VerificationToken),
                    null,
                    DataProtectionScope.LocalMachine
                  )
                );

                string enlaceVerificacion = GenerarEnlaceVerificacion("http://localhost:52512/Registro/", VerificationUID);


                Destinatario = CorreoContactoPrincipal;
                Asunto = "Verificación de correo electrónico para ULAC";
                Mensaje = $"Hola {NombreContactoPrincipal},<br>Para completar tu registro en ULAC, haz clic en el siguiente enlace para verificar tu correo electrónico: <br>{enlaceVerificacion}";

                EnviarCorreoElectronico();
                IsVerified = false;
                MensajeGeneral = "Se ha enviado un correo electrónico de verificación a su dirección. Haga clic en el enlace para completar el registro."; ;

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
                    command.Parameters.AddWithValue("@IsVerified", false);
                    command.Parameters.AddWithValue("@VerificationToken", protectedToken);
                    command.Parameters.AddWithValue("@VerificationUID", VerificationUID);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra al intentar guardar en la base de datos
                MensajeGeneral = "Error al guardar en la base de datos: " + ex.Message;
                return false;
            }
        }

        public void EnviarCorreoElectronico()
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("transportesulac@gmail.com", "znak dpdt xmvl phkt");
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("marilyn030599@gmail.com");
                mailMessage.To.Add(Destinatario);
                mailMessage.Subject = Asunto;
                mailMessage.Body = Mensaje;

                smtpClient.Send(mailMessage);
            }
        }


        private bool UsuarioYaRegistrado(string CorreoContactoPrincipal)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("UsuarioYaRegistrado", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CorreoContactoPrincipal", CorreoContactoPrincipal);

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetInt32(0) == 1;
                }
                return false;
            }

        }
        public string GenerarEnlaceVerificacion(string urlBase, Guid uid)
        {
            // Usa el UID en lugar del token para el enlace de verificación
            return $"{urlBase}VerificarCorreo?uid={uid}";
        }

    }
}