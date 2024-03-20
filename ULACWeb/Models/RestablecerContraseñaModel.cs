using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services.Description;

namespace ULACWeb.Models
{
    public class RestablecerContraseñaModel
    {
        [Required(ErrorMessage = "El  Correo es obligatorio")]
        public string Correo { get; set; }

        public string Destinatario { get; set; }
        public string Asunto{ get; set; }
        public string Mensaje { get; set; }


        public static string GenerarClaveSecreta()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public bool EnviarInstruccion()
        {
            try
            {
                var secretKeyString = ConfigurationManager.AppSettings["JWTSecret"];
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
             

                var tokenOptions = new JwtSecurityToken(
                    issuer: "http://localhost:52512",
                    audience: "http://localhost:52512",
                    claims: new List<Claim>()
                    {

                        new Claim(ClaimTypes.Email, Correo),


                    },
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                        );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                string enlaceVerificacion = GenerarEnlaceVerificacion("http://localhost:52512/PreguntasSeguridad/", tokenString);


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
                                                <div class='header'>ULAC - Cambio de contraseña</div>
                                                <div class='content'>
                                                    <p>Hola</p>
                                                    <p>Has solicitado recuperar tu contraseña en Transportes ULAC.
                                                        Como parte del proceso de recuperación, serás redirigido a una página donde deberás responder ciertas preguntas de seguridad para verificar tu identidad. 
                                                           Esto es una medida de seguridad para proteger tu cuenta. 
                                                            Para proceder, por favor sigue el enlace a continuación.</p>                                                    
                                                    <a href={enlaceVerificacion} class='btn-verify'> Preguntas Seguridad</a>        
                                                        <p>Si no has solicitado una recuperacion de contrasña en ULAC, puedes ignorar este correo.</p>
                                                          
                                                </div>
                                                <div class='footer'>
                                                    <p>Gracias por usar ULAC</p>
                                                </div>
                                                <p class='disclaimer'>Este es un correo automático, por favor no responder.</p>
                                            </div>
                                        </body>
                                        </html>";


                Destinatario = Correo;
                Asunto = "Cambio de contraseña para ULAC";
                Mensaje = mensajeHtml;

                EnviarCorreoElectronico();
                

                return true;
            }
            catch (Exception ex)
            {
               
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
                mailMessage.From = new MailAddress("transportesulac@gmail.com");
                mailMessage.To.Add(Destinatario);
                mailMessage.Subject = Asunto;
                mailMessage.Body = Mensaje;
                mailMessage.IsBodyHtml = true;
                smtpClient.Send(mailMessage);
            }
        }

        public string GenerarEnlaceVerificacion(string urlBase, string token)
        {

            return $"{urlBase}PreguntasSeguridad?uid={token}";
        }

    }
}