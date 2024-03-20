using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ULACWeb.Models
{
    public class PreguntasSeguridadModel
    {
        public string Correo { get; set; }
        public PreguntaSeguridad Pregunta { get; set; }

        public PreguntaSeguridad ObtenerPreguntaSeguridadAleatoria(string uid)
        {


            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(uid) as JwtSecurityToken;
            PreguntaSeguridad pregunta = null;

            if (tokenS != null)
            {
                var correo = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString))
            {
                using (var command = new SqlCommand("spObtenerPreguntasSeguridad", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Correo", correo);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pregunta = new PreguntaSeguridad
                            {
                                IDPregunta = reader.GetInt32(reader.GetOrdinal("IDPregunta")),
                                Pregunta = reader.GetString(reader.GetOrdinal("Pregunta"))
                            };
                        }
                    }
                }
            }
            }


            return pregunta;
        }
        public bool ValidarRespuestaSeguridad(string IDUsuario, int IDPregunta, string Respuesta)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString))
            {
                using (var command = new SqlCommand("spValidarRespuestaSeguridad", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                    command.Parameters.AddWithValue("@IDPregunta", IDPregunta);
                    command.Parameters.AddWithValue("@Respuesta", Respuesta);
                    var EsCorrectaParam = command.Parameters.Add("@EsCorrecta", SqlDbType.Bit);
                    EsCorrectaParam.Direction = ParameterDirection.Output;

                    connection.Open();
                    command.ExecuteNonQuery();

                    return Convert.ToBoolean(EsCorrectaParam.Value);
                }
            }
        }

        public class PreguntaSeguridad
    {
        public int IDPregunta { get; set; }
        public string Pregunta { get; set; }
    }




    }
}