using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration; // Asegúrate de importar este espacio de nombres

namespace ULACWeb.Models
{
    public class SeguimientoModel
    {
        public int IDViaje { get; set; }
        public int IDEmpresa { get; set; }
        public string Chofer { get; set; }
        public string Estado { get; set; }
        public string CodigoEntrega { get; set; }
        public string CompaniaContratante { get; set; }
        public string Destino { get; set; }
        public string Origen { get; set; }

        public List<SeguimientoModel> ObtenerSeguimientoEnvios(int IDEmpresa)
        {
            List<SeguimientoModel> seguimientos = new List<SeguimientoModel>();

            // Aquí se invoca el procedimiento almacenado en lugar de construir una consulta directa
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ObtenerSeguimientoEnvios", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Especificar que es un SP
                    cmd.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SeguimientoModel seguimiento = new SeguimientoModel();
                        seguimiento.IDViaje = Convert.ToInt32(reader["IDViaje"]);
                        seguimiento.IDEmpresa = Convert.ToInt32(reader["IDEmpresa"]);
                        seguimiento.Chofer = reader["Chofer"].ToString();
                        seguimiento.Estado = reader["Estado"].ToString();
                        seguimiento.CodigoEntrega = reader["CodigoEntrega"].ToString();
                        seguimiento.CompaniaContratante = reader["CompaniaContratante"].ToString();
                        seguimiento.Destino = reader["Destino"].ToString();
                        seguimiento.Origen = reader["Origen"].ToString();

                        seguimientos.Add(seguimiento);
                    }
                }
            }

            return seguimientos;
        }
    }
}

