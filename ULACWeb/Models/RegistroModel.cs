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
                string mensajeHtml = $@"
                                        <html>
                                        <head>
                                            <style>
                                                .email-container {{
                                                    max-width: 600px;
                                                    margin: auto;
                                                    background-color: #fff;
                                                    padding: 20px;
                                                    border-radius: 8px;
                                                    border: 1px solid #ccc;
                                                    font-family: 'Arial', sans-serif;
                                                }}
                                                .header {{
                                                    background-color: #d32f2f;
                                                    color: #fff;
                                                    padding: 10px;
                                                    text-align: center;
                                                }}
                                                .footer {{
                                                   
                                                    background-color: #d32f2f;
                                                    color: #fff;
                                                    padding: 20px 10px;
                                                    text-align: center;
                                                    background-image: url('https://get.wallhere.com/photo/1920x1080-px-American-Truck-Simulator-ATS-Kenworth-Peterbilt-trucks-1346089.jpg');
                                                    background-size: cover;
                                                    background-position: center; 
                                                    min-height: 150px;

                                                }}
                                                .content {{
                                                    margin: 20px 0;
                                                    color: #000;
                                                }}
                                                .btn-verify {{
                                                    background-color: #d32f2f;
                                                    color: #fff !important;
                                                    padding: 10px 20px;
                                                    text-decoration: none;
                                                    border-radius: 5px;
                                                    display: block;
                                                    width: fit-content;
                                                    margin: 20px auto;
                                                }}
                                                .disclaimer {{
                                                    font-size: 0.8em;
                                                    color: #666;
                                                    text-align: center;
                                                }}
                                            </style>
                                        </head>
                                        <body>
                                            <div class='email-container'>
                                                <div class='header'>ULAC - Verificación de Correo</div>
                                                <div class='content'>
                                                    <p>Hola {NombreContactoPrincipal},</p>
                                                    <p>Gracias por registrarte en Transportes ULAC. Estamos emocionados que seas parte de nuestros asociados. Para completar tu proceso de registro y verificar tu correo electrónico, haz clic en el botón a continuación:</p>
                                                    <a href='{enlaceVerificacion}' class='btn-verify'>Verificar Cuenta</a>
                                                    <p>Si no has solicitado un registro en ULAC, puedes ignorar este correo.</p>
                                                </div>
                                                <div class='footer'>
                                                    <p>Gracias por usar ULAC</p>
                                                </div>
                                                <p class='disclaimer'>Este es un correo automático, por favor no responder.</p>
                                            </div>
                                        </body>
                                        </html>";


                Destinatario = CorreoContactoPrincipal;
                Asunto = "Verificación de correo electrónico para ULAC";
                Mensaje = mensajeHtml;

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
                mailMessage.IsBodyHtml = true; 
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