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
using System.Data.Entity;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities.Encoders;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace ULACWeb.Models
{
    public class RegistroModel
    {



        [Required(ErrorMessage = "El ID de su empresa es obligatorio")]
        public int IDEmpresa { get; set; }

        [Required(ErrorMessage = "La Identificación es obligatorio")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Número de Teléfono es obligatorio")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "El  Correo es obligatorio")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El Contraseña es obligatorio")]
        public string Contraseña { get; set; }

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

        public string AsuntoToken { get; set; }
        public string MensajeToken { get; set; }

        [Required(ErrorMessage = "Elegir el pais es obligatoria")]
        public string NombrePais { get; set; }
        [Required(ErrorMessage = "Elegir la provincia es obligatoria")]
        public string NombreProvincia { get; set; }
        [Required(ErrorMessage = "Elegir el canton es obligatoria")]
        public string NombreCanton { get; set; }
        [Required(ErrorMessage = "Elegir el distrito es obligatoria")]
        public string NombreDistrito { get; set; }

        public class Pais
        {
            public int IDPais { get; set; }
            public string NombrePais { get; set; } 
            public virtual ICollection<Provincia> Provincias { get; set; }
        }

        public class Provincia
        {
            public int IDProvincia { get; set; }
            public int IDPais { get; set; }
            public string NombreProvincia { get; set; }
            public virtual Pais Pais { get; set; }
            public virtual ICollection<Canton> Cantones { get; set; }
        }

        public class Canton
        {
            public int IDCanton { get; set; }
            public string NombreCanton { get; set; }
            public int IDProvincia { get; set; }
            public virtual Provincia Provincia { get; set; }
            public virtual ICollection<Distrito> Distritos { get; set; }
        }

        public class Distrito
        {
            public int IDDistrito { get; set; }
            public string NombreDistrito { get; set; }
            public int IDCanton { get; set; }
            public virtual Canton Canton { get; set; }
        }

        public static string GenerarClaveSecreta()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }



        public bool EnviodeCorreoVerificacion()
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
                        new Claim(ClaimTypes.Name, Nombre),
                        new Claim("IDEmpresa" ,IDEmpresa.ToString()),
                        new Claim("Identificacion",Identificacion),
                        new Claim("Telefono", Telefono),
                        new Claim("PaisResidencia", NombrePais),
                        new Claim("Provincia", NombreProvincia),
                        new Claim("Canton", NombreCanton),
                        new Claim("Distrito", NombreDistrito),
                        new Claim("Contraseña", Contraseña), 
                        new Claim("Pregunta1", Pregunta1),
                        new Claim("Pregunta2", Pregunta2),
                        new Claim("Pregunta3", Pregunta3),
                        new Claim("Respuesta1", Respuesta1),
                        new Claim("Respuesta2", Respuesta2),
                        new Claim("Respuesta3", Respuesta3)

                    },
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                        );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                string enlaceVerificacion = GenerarEnlaceVerificacion("http://localhost:52512/Registro/", tokenString);

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
                                                    <p>Hola {Nombre},</p>
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


                Destinatario = Correo;
                Asunto = "Verificación de correo electrónico para ULAC";
                Mensaje = mensajeHtml;

                EnviarCorreoElectronico();
                MensajeGeneral = "Se ha enviado un correo electrónico de verificación a su dirección. Haga clic en el enlace para completar el registro."; ;

                return true;
            }
            catch (Exception ex)
            {
                MensajeGeneral = "Error al enviar el correo de verificación: " + ex.Message;
                return false;
            }
        }

        public bool GuardarDatosVerificados(string token)
        {
            try
            {
                Random random = new Random();
                int tokenpersonal = random.Next(10000000, 100000000);
                string tokensesion = tokenpersonal.ToString();
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;

                if (tokenS != null)
                {
                    var IDEmpresa = tokenS.Claims.First(claim => claim.Type == "IDEmpresa").Value;
                    var correo = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
                    var nombre = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                    var identificacion = tokenS.Claims.First(claim => claim.Type == "Identificacion").Value;
                    var telefono = tokenS.Claims.First(claim => claim.Type == "Telefono").Value;
                    var paisResidencia = tokenS.Claims.First(claim => claim.Type == "PaisResidencia").Value;
                    var provincia = tokenS.Claims.First(claim => claim.Type == "Provincia").Value;
                    var canton = tokenS.Claims.First(claim => claim.Type == "Canton").Value;
                    var distrito = tokenS.Claims.First(claim => claim.Type == "Distrito").Value;
                    var contraseña = tokenS.Claims.First(claim => claim.Type == "Contraseña").Value;
                    var pregunta1 = tokenS.Claims.First(claim => claim.Type == "Pregunta1").Value;
                    var pregunta2 = tokenS.Claims.First(claim => claim.Type == "Pregunta2").Value;
                    var pregunta3 = tokenS.Claims.First(claim => claim.Type == "Pregunta3").Value;
                    var respuesta1 = tokenS.Claims.First(claim => claim.Type == "Respuesta1").Value;
                    var respuesta2 = tokenS.Claims.First(claim => claim.Type == "Respuesta2").Value;
                    var respuesta3 = tokenS.Claims.First(claim => claim.Type == "Respuesta3").Value;
                    int.TryParse(paisResidencia, out int IDPaisResidencia);
                    int.TryParse(provincia, out int IDProvincia);
                    int.TryParse(canton, out int IDcanton);
                    int.TryParse(distrito, out int IDdistrito);

                    string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("InsertarUsuario", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);
                        command.Parameters.AddWithValue("@Correo", correo);
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@Identificacion", identificacion);
                        command.Parameters.AddWithValue("@Telefono", telefono);
                        command.Parameters.AddWithValue("@IDPaisResidencia", IDPaisResidencia);
                        command.Parameters.AddWithValue("@IDProvincia", IDProvincia);
                        command.Parameters.AddWithValue("@IDCanton", IDcanton);
                        command.Parameters.AddWithValue("@IDDistrito", IDdistrito);
                        
                        command.Parameters.AddWithValue("@Contraseña", contraseña);
                        command.Parameters.AddWithValue("@Pregunta1", pregunta1);
                        command.Parameters.AddWithValue("@Pregunta2", pregunta2);
                        command.Parameters.AddWithValue("@Pregunta3", pregunta3);
                        command.Parameters.AddWithValue("@Respuesta1", respuesta1);
                        command.Parameters.AddWithValue("@Respuesta2", respuesta2);
                        command.Parameters.AddWithValue("@Respuesta3", respuesta3);
                        command.Parameters.AddWithValue("@Token", tokensesion);
                        


                        command.ExecuteNonQuery();
                    }
                    EnviarToken(correo, tokensesion);
                    return true;
                }
                else
                {
                    Console.WriteLine("Token inválido.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra al guardar los datos verificados en la base de datos
                Console.WriteLine($"Error al guardar los datos verificados en la base de datos: {ex.Message}");
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

        public void EnviarToken(string Destinatario, string Token)
        {
            AsuntoToken = "Registro ULAC - TOKEN";
        
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
                                                <p>Tu token es: <strong>{Token}</strong></p>
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
                mailMessage.Subject = AsuntoToken;
                mailMessage.Body = mensajeHtmlToken; 
                mailMessage.IsBodyHtml = true;

                smtpClient.Send(mailMessage);
                MensajeGeneral = "Se ha enviado un correo electrónico de verificación a su dirección. Haga clic en el enlace para completar el registro."; ;
            }
        }




       


        public string GenerarEnlaceVerificacion(string urlBase, string token)
        {
           
            return $"{urlBase}Verificar?uid={token}";
        }

        public List<Pais> GetPaises()
        {
            List<Pais> paises = new List<Pais>();
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT IDPais, NombrePais FROM Pais", connection);
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Pais pais = new Pais()
                    {
                        IDPais = (int)reader["IDPais"],
                        NombrePais = reader["NombrePais"].ToString()
                    };
                    paises.Add(pais);
                }
            }

            return paises;
        }

        public List<Provincia> GetProvincias(int idPais)
        {
            List<Provincia> provincias = new List<Provincia>();
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("ExtraerProvincia", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idPais", idPais); 

                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Provincia provincia = new Provincia()
                    {
                        IDProvincia = (int)reader["IDProvincia"],
                        NombreProvincia = reader["NombreProvincia"].ToString(),
                        IDPais = idPais 
                    };
                    provincias.Add(provincia);
                }
            }

            return provincias;
        }
        public List<Canton> GetCanton(int IDProvincia)
        {
            List<Canton> cantones = new List<Canton>();
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("ExtraerCanton", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IDProvincia", IDProvincia); // Asegúrate que el parámetro se llame así en tu SP

                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Canton canton = new Canton()
                    {
                        IDCanton = (int)reader["IDCanton"],
                        NombreCanton = reader["NombreCanton"].ToString(),
                        IDProvincia = IDProvincia // Asumiendo que quieras mantener esta información aquí
                    };
                    cantones.Add(canton); // Añade 'canton', no 'cantones'
                }
            }

            return cantones;
        }
        public List<Distrito> GetDistrito(int IDCanton)
        {
            List<Distrito> distritos = new List<Distrito>();
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("ExtraerDistrito", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IDCanton", IDCanton); // Asegúrate que el parámetro se llame así en tu SP

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Distrito distrito = new Distrito()
                            {
                                IDDistrito = (int)reader["IDDistrito"],
                                NombreDistrito = reader["NombreDistrito"].ToString(),
                                IDCanton = IDCanton // Asumiendo que quieras mantener esta información aquí
                            };
                            distritos.Add(distrito);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejo específico de excepciones de SQL
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                // Considera registrar este error en un archivo de logs o manejarlo según las necesidades de tu aplicación.
            }
            catch (Exception ex)
            {
                // Manejo de otras excepciones
                Console.WriteLine($"Error: {ex.Message}");
                // Considera registrar este error en un archivo de logs o manejarlo según las necesidades de tu aplicación.
            }

            return distritos;
        }


    }
}