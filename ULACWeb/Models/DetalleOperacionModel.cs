using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ULACWeb.Models
{
    public class DetalleOperacionModel
    {
        public int IDOperacion { get; set; }
        public string Industria { get; set; }
        public string Ruta { get; set; }
        public string Contacto { get; set; }
        public string TiempoEstimado { get; set; }
        public string Estado { get; set; }

        public List<DetalleOperacionModel> ObtenerDetalles(int IDEmpresa)
        {
            List<DetalleOperacionModel> listaDetalleOperacion = new List<DetalleOperacionModel>();

            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("TraerDetalles", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        DetalleOperacionModel detalleOperacion = new DetalleOperacionModel();

                        detalleOperacion.IDOperacion = Convert.ToInt32(reader["IDOperacion"]);
                        detalleOperacion.Industria = reader["Industria"].ToString();
                        detalleOperacion.Ruta = reader["Ruta"].ToString();
                        detalleOperacion.Contacto = reader["Contacto"].ToString();
                        detalleOperacion.TiempoEstimado = reader["TiempoEstimado"].ToString();
                        detalleOperacion.Estado = reader["Estado"].ToString();

                        listaDetalleOperacion.Add(detalleOperacion);
                    }

                    reader.Close();
                }
            }

            return listaDetalleOperacion;
        }
    }
}
