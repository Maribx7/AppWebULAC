using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace ULACWeb.Models
{
    public class LoginModel
    {
        public string Usuario { get; set; }
        public string Contraseñas { get; set; }
        public string Token { get; set; }
        public int IDEmpresa { get; set; }




        public bool VerificarCredenciales(out int idEmpresa)
        {
            idEmpresa = 0; // Inicializa idEmpresa a 0

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("VerificarCredenciales", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Usuario", Usuario);
                    command.Parameters.AddWithValue("@Contraseña", Contraseñas);
                    command.Parameters.AddWithValue("@Token", Token);

                    // Define el parámetro de salida @IDEmpresa
                    SqlParameter idEmpresaParam = new SqlParameter("@IDEmpresa", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idEmpresaParam);

                    command.ExecuteNonQuery(); // Ejecuta el procedimiento almacenado

                    // Lee el valor del parámetro de salida
                    if (idEmpresaParam.Value != DBNull.Value)
                    {
                        idEmpresa = Convert.ToInt32(idEmpresaParam.Value);
                    }
                }

                // Retorna verdadero si idEmpresa es diferente de 0, lo que indica que se encontraron las credenciales
                return idEmpresa != 0;
            }
            catch (Exception ex)
            {
                // Maneja cualquier excepción que ocurra al intentar verificar las credenciales en la base de datos
                Console.WriteLine("Error al verificar las credenciales: " + ex.Message);
                return false;
            }
        }

        public int ObtenerIDEmpresa()
        {
            
            return IDEmpresa;
        }
        public bool VerificarNOCredenciales(out int idEmpresa)
        {
            idEmpresa = 0; // Inicializa idEmpresa a 0

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("VerificarNOCredenciales", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Usuario", Usuario);
                    command.Parameters.AddWithValue("@Contraseña", Contraseñas);
                    command.Parameters.AddWithValue("@Token", Token);

                    // Define el parámetro de salida @IDEmpresa
                    SqlParameter idEmpresaParam = new SqlParameter("@IDEmpresa", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idEmpresaParam);

                    command.ExecuteNonQuery(); // Ejecuta el procedimiento almacenado

                    // Lee el valor del parámetro de salida
                    if (idEmpresaParam.Value != DBNull.Value)
                    {
                        idEmpresa = Convert.ToInt32(idEmpresaParam.Value);
                    }
                }

                // Retorna verdadero si idEmpresa es diferente de 0, lo que indica que se encontraron las credenciales
                return idEmpresa != 0;
            }
            catch (Exception ex)
            {
                // Maneja cualquier excepción que ocurra al intentar verificar las credenciales en la base de datos
                Console.WriteLine("Error al verificar las credenciales: " + ex.Message);
                return false;
            }
        }



    }
}