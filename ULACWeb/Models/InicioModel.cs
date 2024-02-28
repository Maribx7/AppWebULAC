using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace ULACWeb.Models
{
    public class InicioModel
    {
        public class Contrato
        {
            public int IDContrato { get; set; }
            public int IDEmpresa { get; set; }
            public int IDTipoPaquete { get; set; }
            public string Origen { get; set; }
            public string Destino { get; set; }
            public string Industria { get; set; }
            public string TiempoEstimado { get; set; }
            public decimal Subtotal { get; set; }
            public string MetodoPago { get; set; }
            public string NumeroTarjeta { get; set; }
            public string FechaVencimiento { get; set; }
            public string CodigoSeguridad { get; set; }
            public decimal Total { get; set; }


            public void Guardar()
            {
                // Cadena de conexión a la base de datos (debes configurarla según tu entorno)
                string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;

                // Crear conexión
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Crear comando para llamar al procedimiento almacenado
                    using (SqlCommand command = new SqlCommand("GuardarContrato", connection))
                    {
                        // Especificar que el comando es un procedimiento almacenado
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Agregar parámetros al comando
                        command.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);
                        command.Parameters.AddWithValue("@IDTipoPaquete", IDTipoPaquete);
                        command.Parameters.AddWithValue("@Origen", Origen);
                        command.Parameters.AddWithValue("@Destino", Destino);
                        command.Parameters.AddWithValue("@TiempoEstimado", TiempoEstimado);
                        command.Parameters.AddWithValue("@Industria", Industria);
                        command.Parameters.AddWithValue("@Subtotal", Subtotal);
                        command.Parameters.AddWithValue("@MetodoPago", MetodoPago);
                        command.Parameters.AddWithValue("@NumeroTarjeta", NumeroTarjeta);
                        command.Parameters.AddWithValue("@FechaVencimiento", FechaVencimiento);
                        command.Parameters.AddWithValue("@CodigoSeguridad", CodigoSeguridad);
                        command.Parameters.AddWithValue("@Total", Total);

                        // Abrir conexión
                        connection.Open();

                        // Ejecutar el comando
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
