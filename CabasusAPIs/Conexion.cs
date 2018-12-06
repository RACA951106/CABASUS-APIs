using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace CabasusAPIs
{
    public class Conexion
    {
        MySqlConnection conexion = null;
        public Conexion()
        {
            var cadena = new MySqlConnectionStringBuilder();
            cadena.UserID = "root";
            cadena.Password = "password";
            cadena.Database = "susabac";
            cadena.Server = "localhost";

            conexion = new MySqlConnection(cadena.ToString());
        }

        public DataTable Consultar(string Query)
        {
            conexion.Open();
            MySqlCommand command = new MySqlCommand(Query, conexion);
            command.ExecuteNonQuery();

            var dataset = new DataTable();

            var adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataset);

            conexion.Close();

            return dataset;
        }

        public bool Insertar(string Query)
        {
            try
            {
                conexion.Open();
                MySqlCommand command = new MySqlCommand(Query, conexion);
                command.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch(Exception ex) 
            {
                Console.WriteLine("//////////////////////////////////" + ex.Message + "//////////////////////////////////");
                return false;
            }
        }

        public List<object> Procedimiento(string nombre, List<ProcedureParameters> parametros, List<string> campos)
        {
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader reader;

            cmd.CommandText = nombre;
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (var item in parametros)
            {
                cmd.Parameters.AddWithValue(item.parametro, item.valor);
            }

            cmd.Connection = conexion;

            conexion.Open();

            reader = cmd.ExecuteReader();

            List<object> Datos = new List<object>();

            while (reader.Read())
            {
                Dictionary<object, object> Row = new Dictionary<object, object>();
                foreach (var item in campos)
                {
                    Row.Add(item, reader[item]);
                }
                Datos.Add(Row);
            }

            conexion.Close();

            return Datos;
        }


    }

    public class ProcedureParameters
    {
        public string parametro
        {
            get;
            set;
        }
        public object valor
        {
            get;
            set;
        }
    }
}
