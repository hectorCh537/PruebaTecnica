using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace Prueba_2
{
    class Program
    {
        static string connectionString = "server=localhost;user=root;database=empresa;port=3306;password=";

        static void Main(string[] args)
        {
            while (true)
            {
                MostrarMenu();
                string? opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        ListarTopUsuarios();
                        break;
                    case "2":
                        GenerarCSV();
                        break;
                    case "3":
                        ActualizarSalarioMenu();
                        break;
                    case "4":
                        AgregarNuevoUsuarioMenu();
                        break;
                    case "5":
                        Console.WriteLine("Saliendo del sistema...");
                        return;
                    default:
                        Console.WriteLine("Opción no válida, por favor intente de nuevo.");
                        break;
                }
            }
        }

        static void MostrarMenu()
        {
            Console.WriteLine("Menú Principal:");
            Console.WriteLine("1. Listar top 10 usuarios");
            Console.WriteLine("2. Generar archivo CSV");
            Console.WriteLine("3. Actualizar salario de un usuario");
            Console.WriteLine("4. Agregar nuevo usuario");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
        }

        static void ListarTopUsuarios()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM usuarios LIMIT 10";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                Console.WriteLine("Top 10 Usuarios:");
                while (rdr.Read())
                {
                    Console.WriteLine($"{rdr["userId"]}, {rdr["Login"]}, {rdr["Nombre"]}, {rdr["Paterno"]}, {rdr["Materno"]}");
                }
                rdr.Close();
            }
        }

        static void GenerarCSV()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT u.Login, CONCAT(u.Nombre, ' ', u.Paterno, ' ', u.Materno) AS NombreCompleto, e.Sueldo, e.FechaIngreso 
                    FROM usuarios u 
                    JOIN empleados e ON u.userId = e.userId";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                using (StreamWriter sw = new StreamWriter("usuarios.csv"))
                {
                    sw.WriteLine("Login,Nombre Completo,Sueldo,Fecha Ingreso");
                    while (rdr.Read())
                    {
                        sw.WriteLine($"{rdr["Login"]},{rdr["NombreCompleto"]},{rdr["Sueldo"]},{rdr["FechaIngreso"]}");
                    }
                }
                rdr.Close();
            }

            Console.WriteLine("Archivo CSV generado exitosamente.");
        }

        static void ActualizarSalarioMenu()
        {
            Console.Write("Ingrese el ID del usuario: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("ID de usuario no válido.");
                return;
            }

            Console.Write("Ingrese el nuevo salario: ");
            if (!double.TryParse(Console.ReadLine(), out double nuevoSueldo))
            {
                Console.WriteLine("Salario no válido.");
                return;
            }

            ActualizarSalario(userId, nuevoSueldo);
        }

        static void ActualizarSalario(int userId, double nuevoSueldo)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE empleados SET Sueldo = @sueldo WHERE userId = @userId";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@sueldo", nuevoSueldo);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Salario actualizado exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("No se encontró el usuario especificado.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el salario: " + ex.Message);
            }
        }

        static void AgregarNuevoUsuarioMenu()
        {
            Console.Write("Ingrese el nombre: ");
            string? nombre = Console.ReadLine();

            Console.Write("Ingrese el apellido paterno: ");
            string? paterno = Console.ReadLine();

            Console.Write("Ingrese el apellido materno: ");
            string? materno = Console.ReadLine();

            Console.Write("Ingrese el sueldo: ");
            if (!double.TryParse(Console.ReadLine(), out double sueldo))
            {
                Console.WriteLine("Sueldo no válido.");
                return;
            }

            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(paterno) && !string.IsNullOrEmpty(materno))
            {
                AgregarNuevoUsuario(nombre, paterno, materno, sueldo);
            }
            else
            {
                Console.WriteLine("Nombre, apellido paterno y apellido materno no pueden ser nulos.");
            }
        }

        static void AgregarNuevoUsuario(string nombre, string paterno, string materno, double sueldo)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

              
                string sqlUltimoLogin = "SELECT Login FROM usuarios ORDER BY userId DESC LIMIT 1";
                MySqlCommand cmdUltimoLogin = new MySqlCommand(sqlUltimoLogin, conn);
                string? ultimoLogin = cmdUltimoLogin.ExecuteScalar() as string;

 
                if (string.IsNullOrEmpty(ultimoLogin) || !ultimoLogin.StartsWith("user"))
                {
                    throw new FormatException("El último login no tiene el formato esperado 'userXX'.");
                }


                int ultimoNumero;
                try
                {
                    ultimoNumero = int.Parse(ultimoLogin.Substring(4)); 
                }
                catch (FormatException)
                {
                    throw new FormatException("El último login no contiene un número válido.");
                }


                int nuevoNumero = ultimoNumero + 1;
                string nuevoLogin = "user" + nuevoNumero.ToString("D2");


                string sqlUsuario = "INSERT INTO usuarios (Login, Nombre, Paterno, Materno) VALUES (@login, @nombre, @paterno, @materno)";
                MySqlCommand cmdUsuario = new MySqlCommand(sqlUsuario, conn);
                cmdUsuario.Parameters.AddWithValue("@login", nuevoLogin);
                cmdUsuario.Parameters.AddWithValue("@nombre", nombre);
                cmdUsuario.Parameters.AddWithValue("@paterno", paterno);
                cmdUsuario.Parameters.AddWithValue("@materno", materno);
                cmdUsuario.ExecuteNonQuery();

                long userId = cmdUsuario.LastInsertedId;


                string sqlEmpleado = "INSERT INTO empleados (userId, Sueldo, FechaIngreso) VALUES (@userId, @sueldo, @fechaIngreso)";
                MySqlCommand cmdEmpleado = new MySqlCommand(sqlEmpleado, conn);
                cmdEmpleado.Parameters.AddWithValue("@userId", userId);
                cmdEmpleado.Parameters.AddWithValue("@sueldo", sueldo);
                cmdEmpleado.Parameters.AddWithValue("@fechaIngreso", DateTime.Now);
                cmdEmpleado.ExecuteNonQuery();

                Console.WriteLine("Nuevo usuario agregado exitosamente.");
            }
        }
    }
}
