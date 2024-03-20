using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Net.Mail;
using ULACWeb.Controllers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace ULACWeb.Models
{
    public class CambioContraseñaModel {
        //DATOS PARA EL MAIL
        public string Destinatario { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }

        public string IDUsuario { get; set; }
        public string NuevaContraseña { get; set; }



        public bool CambioContraseña(string IDUsuario, string NuevaContraseña, string correo)
        {
           
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString))
                {
                    using (var command = new SqlCommand("CambiarContraseña", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Asegúrate de que los nombres de los parámetros coincidan con los definidos en tu procedimiento almacenado.
                        command.Parameters.AddWithValue("@Identificacion", IDUsuario);
                        command.Parameters.AddWithValue("@NuevaContraseña", NuevaContraseña);

                        connection.Open();
                        command.ExecuteNonQuery();
                      
                    }
                }
                Random random = new Random();
                int tokenpersonal = random.Next(10000000, 100000000);
                string tokensesion = tokenpersonal.ToString();
                var nuevoToken = GenerarNuevoToken(correo);
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString))
                {
                    using (var command = new SqlCommand("spActualizarTokenUsuario", connection)) // Asume que este SP actualiza el token
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                        command.Parameters.AddWithValue("@NuevoToken", tokensesion);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                // Finalmente, enviamos el nuevo token por correo
                EnviarToken(correo, tokensesion);
                return true;
                
            }
            catch (Exception ex)
            {
                // Considera implementar una forma de registrar o manejar la excepción
                return false;
            }
        }
        private string GenerarNuevoToken(string correo)
        {
            var secretKeyString = ConfigurationManager.AppSettings["JWTSecret"];
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:52512",
                audience: "http://localhost:52512",
                claims: new List<Claim>() { new Claim(ClaimTypes.Email, correo) },
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }

      
        public void EnviarToken(string Destinatario, string tokensesion)
        {

            Asunto = "Cambio de contraseña ULAC - TOKEN";

            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("transportesulac@gmail.com", "znak dpdt xmvl phkt");
                smtpClient.EnableSsl = true;

                string mensajeHtmlToken = $@"
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
                                            <div class='header'>ULAC - Token de Inicio de Sesión</div>
                                            <div class='content'>
                                                <p>Hola,</p>
                                                <p>Gracias por verificar tu correo electrónico. A continuación, encontrarás tu token personal para iniciar sesión en Transportes ULAC. Es importante que mantengas este token seguro y no lo compartas con nadie.</p>
                                                <p>Tu token es: <strong>{tokensesion}</strong></p>
                                                <p>Para iniciar sesión, puedes utilizar el siguiente enlace y proporcionar tu token cuando se te solicite.</p>
                                                <a href='http://localhost:52512/' class='btn-verify'>Iniciar Sesión</a>
                                                <p>Si no has solicitado este token, por favor ignora este mensaje o ponte en contacto con nuestro equipo de soporte.</p>
                                            </div>
                                            <div class='footer'>
                                                <p>Gracias por usar ULAC</p>
                                            </div>
                                            <p class='disclaimer'>Este es un correo automático, por favor no responder.</p>
                                        </div>
                                    </body>
                                    </html>";




                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("transportesulac@gmail.com");
                mailMessage.To.Add(Destinatario);
                mailMessage.Subject = Asunto;
                mailMessage.Body = mensajeHtmlToken;
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);
              
            }
        }







        public string GenerarEnlaceVerificacion(string urlBase, string token)
        {

            return $"{urlBase}Verificar?uid={token}";
        }


    }
}