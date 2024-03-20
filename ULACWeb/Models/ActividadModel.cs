using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ULACWeb.Models
{
    public class ActividadModel
    {

        public class Actividad
        {
            public int IDEmpresa { get; set; }
            public string Usuario { get; set; }
            public string TipoActividad { get; set; }
            public string Detalles { get; set; }
            public DateTime FechaHora { get; set; } = DateTime.Now; // Predeterminado al momento actual
            public string IP { get; set; }
        }
        public static class RegistroActividadesHelper
        {
            public static void RegistrarActividad(Actividad actividad)
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                    INSERT INTO RegistroActividades (IDEmpresa, TipoActividad, Detalles, FechaHora, IP)
                    VALUES (@IDEmpresa, @TipoActividad, @Detalles, @FechaHora, @IP)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IDEmpresa", actividad.IDEmpresa);

                            command.Parameters.AddWithValue("@TipoActividad", actividad.TipoActividad);
                            command.Parameters.AddWithValue("@Detalles", actividad.Detalles ?? string.Empty);
                            command.Parameters.AddWithValue("@FechaHora", actividad.FechaHora);
                            command.Parameters.AddWithValue("@IP", actividad.IP ?? string.Empty);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Considera loguear el error con algún mecanismo de logging
                    Console.WriteLine("Error al registrar la actividad: " + ex.Message);
                }
            }
            public static void actividadLoginFallido(string identificacion, int intentosMaximosFallidos = 4)
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Primero, actualizamos el número de intentos fallidos
                        string queryActualizarIntentos = @"
                UPDATE Usuario
                SET IntentosFallidos = IntentosFallidos + 1
                WHERE IDEmpresa = @IDEmpresa";

                        using (SqlCommand command = new SqlCommand(queryActualizarIntentos, connection))
                        {
                            command.Parameters.AddWithValue("@IDEmpresa", identificacion);
                            command.ExecuteNonQuery();
                        }

                        // Luego, verificamos si se alcanzó el número máximo de intentos fallidos
                        string queryVerificarIntentos = @"
                SELECT IntentosFallidos
                FROM Usuario
                WHERE IDEmpresa = @IDEmpresa";

                        using (SqlCommand command = new SqlCommand(queryVerificarIntentos, connection))
                        {
                            command.Parameters.AddWithValue("@IDEmpresa", identificacion);
                            int intentosFallidos = (int)command.ExecuteScalar();

                            if (intentosFallidos >= intentosMaximosFallidos)
                            {
                                // Si se supera el número máximo de intentos, bloqueamos la cuenta
                                string queryBloquearCuenta = @"
                        UPDATE Usuario
                        SET Estado = 'BL' 
                        WHERE IDEmpresa = @IDEmpresa";

                                using (SqlCommand commandBloquear = new SqlCommand(queryBloquearCuenta, connection))
                                {
                                    commandBloquear.Parameters.AddWithValue("@IDEmpresa", identificacion);
                                    commandBloquear.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al actualizar los intentos fallidos y verificar el bloqueo: " + ex.Message);
                }
            }
            public static void RegistrarUltimoIngreso(string IDEmpresa, DateTime ultimoIngreso)
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                            UPDATE Usuario
                            SET UltimoIntento = @UltimoIntento
                            WHERE IDEmpresa = @IDEmpresa";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IDEmpresa", IDEmpresa);
                            command.Parameters.AddWithValue("@UltimoIntento", ultimoIngreso);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al registrar el último ingreso: " + ex.Message);
                }
            }
            public static bool EstaUsuarioBloqueado(int idEmpresa)
            {
                bool estaBloqueado = false;
                int intentosFallidos = 0;

                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["SqlConexion"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                SELECT IntentosFallidos
                FROM Usuario
                WHERE IDEmpresa = @IDEmpresa";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@IDEmpresa", idEmpresa);
                            object result = command.ExecuteScalar();

                            if (result != null)
                            {
                                intentosFallidos = Convert.ToInt32(result);
                            }
                        }

                        // Mueve la lógica de actualización aquí, dentro del mismo bloque 'using' donde la conexión está abierta
                        if (intentosFallidos >= 3)
                        {
                            estaBloqueado = true;
                            string queryActualizarEstado = @"
                UPDATE Usuario
                SET Estado = 'BL'
                WHERE IDEmpresa = @IDEmpresa";

                            using (SqlCommand commandUpdate = new SqlCommand(queryActualizarEstado, connection)) // Usa la misma conexión abierta
                            {
                                commandUpdate.Parameters.AddWithValue("@IDEmpresa", idEmpresa);
                                commandUpdate.ExecuteNonQuery(); // Ahora la conexión ya está abierta
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al verificar si el usuario está bloqueado: " + ex.Message);
                }

                return estaBloqueado;
            }

        }
    }

    }

   