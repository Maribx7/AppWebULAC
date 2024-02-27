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
        [Required(ErrorMessage = "El campo Usuario es requerido.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido.")]
        public string Contraseñas { get; set; }
       
        public int IDEmpresa { get; set; }

        public bool VerificarCredenciales(out int idEmpresa)
        {
            idEmpresa = 0;

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
                    int resultado = (int)command.ExecuteScalar();
                    if (resultado == 1)
                    {
                        SqlCommand getIdEmpresaCommand = new SqlCommand("ObtenerIDEmpresaPorUsuario", connection);
                        getIdEmpresaCommand.CommandType = CommandType.StoredProcedure;
                        getIdEmpresaCommand.Parameters.AddWithValue("@Usuario", Usuario);
                        idEmpresa = (int)getIdEmpresaCommand.ExecuteScalar();

                    }



                    // Si el resultado es 1, las credenciales son válidas; de lo contrario, no lo son
                    return resultado == 1;
                }
            }

            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra al intentar verificar las credenciales en la base de datos
                Console.WriteLine("Error al verificar las credenciales: " + ex.Message);
                return false;
            }

        }
        public int ObtenerIDEmpresa()
        {
            
            return IDEmpresa;
        }
    }
}