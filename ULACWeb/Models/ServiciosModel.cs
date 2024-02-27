using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ULACWeb.Models
{
    public class ServiciosModel
    {
        public int IDEmpresa { get; set; }
        public string NombreServicio { get; set; }
        public decimal CantidadMercancia { get; set; }
        public string MarcaCamion { get; set; }
        public DateTime FechaContrato { get; set; }
        public string EstadoServicio { get; set; }
        public string Seguro { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public int TiempoEstimado { get; set; }
        public decimal Subtotal { get; set; }
        public string MetodoPago { get; set; }
        public string NumeroTarjeta { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string CodigoSeguridad { get; set; }


        public void GuardarContrato()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GuardarContrato", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Parámetros del stored procedure
                command.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);
                command.Parameters.AddWithValue("@NombreServicio", NombreServicio);
                command.Parameters.AddWithValue("@CantidadMercancia", CantidadMercancia);
                command.Parameters.AddWithValue("@MarcaCamion", MarcaCamion);
                command.Parameters.AddWithValue("@FechaContrato", FechaContrato);
                command.Parameters.AddWithValue("@EstadoServicio", EstadoServicio);
                command.Parameters.AddWithValue("@Seguro", Seguro);
                command.Parameters.AddWithValue("@Origen", Origen);
                command.Parameters.AddWithValue("@Destino", Destino);
                command.Parameters.AddWithValue("@TiempoEstimado", TiempoEstimado);
                command.Parameters.AddWithValue("@Subtotal", Subtotal);
                command.Parameters.AddWithValue("@MetodoPago", MetodoPago);
                command.Parameters.AddWithValue("@NumeroTarjeta", NumeroTarjeta);
                command.Parameters.AddWithValue("@FechaVencimiento", FechaVencimiento);
                command.Parameters.AddWithValue("@CodigoSeguridad", CodigoSeguridad);

                // Ejecutar el comando
                command.ExecuteNonQuery();
            }
        }

        public bool VerificarContratos(int IDEmpresa)
        {
            bool tieneContratos = false;

           
            string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("VerificarContratosSP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros del stored procedure
                    command.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);

                    // Parámetro de retorno para determinar si tiene contratos
                    SqlParameter tieneContratosParam = new SqlParameter();
                    tieneContratosParam.ParameterName = "@TieneContratos";
                    tieneContratosParam.SqlDbType = SqlDbType.Bit;
                    tieneContratosParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(tieneContratosParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    // Obteniendo el valor de retorno del stored procedure
                    tieneContratos = (bool)tieneContratosParam.Value;
                }
            }

            return tieneContratos;
        }


    }
}