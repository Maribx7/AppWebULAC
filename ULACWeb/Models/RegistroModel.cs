using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace ULACWeb.Models
{
    public class RegistroModel
    {
        public int IDEmpresa { get; set; }
        public string NombreContactoPrincipal { get; set; }
        public string NumeroContactoPrincipal { get; set; }
        public string PaisResidencia { get; set; }
        public string Contraseñas { get; set; }
        public string Usuario { get; set; }
        public string CorreoContactoPrincipal { get; set; }

        public bool GuardarEnBaseDeDatos()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("InsertarCliente", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);
                    command.Parameters.AddWithValue("@NombreContactoPrincipal", NombreContactoPrincipal);
                    command.Parameters.AddWithValue("@NumeroContactoPrincipal", NumeroContactoPrincipal);
                    command.Parameters.AddWithValue("@PaisResidencia", PaisResidencia);
                    command.Parameters.AddWithValue("@Contraseñas", Contraseñas);
                    command.Parameters.AddWithValue("@Usuario", Usuario);
                    command.Parameters.AddWithValue("@CorreoContactoPrincipal", CorreoContactoPrincipal);

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