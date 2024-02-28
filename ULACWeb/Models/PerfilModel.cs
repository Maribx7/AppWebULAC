using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ULACWeb.Models
{
    public class PerfilModel
    {
        public string NombreContactoPrincipal { get; set; }
        public string NumeroContactoPrincipal { get; set; }
        public string CorreoContactoPrincipal { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public string PaisResidencia { get; set; }

        public List<PerfilModel> ObtenerDatosCliente(int IDEmpresa)
        {
            List<PerfilModel> listaClientes = new List<PerfilModel>();

            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ObtenerDatosCliente", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);
                    
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PerfilModel cliente = new PerfilModel();

                        cliente.NombreContactoPrincipal = reader["NombreContactoPrincipal"].ToString();
                        cliente.NumeroContactoPrincipal = reader["NumeroContactoPrincipal"].ToString();
                        cliente.CorreoContactoPrincipal = reader["CorreoContactoPrincipal"].ToString();
                        cliente.Usuario = reader["Usuario"].ToString();
                        cliente.Contraseña = reader["Contraseñas"].ToString();
                        cliente.PaisResidencia = reader["PaisResidencia"].ToString();

                        listaClientes.Add(cliente);
                    }

                    reader.Close();
                }
            }

            return listaClientes;
        }

    }
}
