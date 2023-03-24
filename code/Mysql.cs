using MySql.Data.MySqlClient;
using System;

namespace projet23_Station_météo_WPF.UserControls
{
    public class Serveur
    {
        static MySqlConnection srv;
        public Serveur()
        {

        }

        public void connexion(string server, string database, string uid, string pwd)
        {
            srv = new MySqlConnection($"Server={server};Database={database};Uid={uid};Pwd={pwd};");
            try
            {
                srv.Open();
                Console.WriteLine("co!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void close()
        {
            srv.Close();
        }

        public MySqlDataReader get(string table, string column = null, string where = null)
        {
            if (srv == null) return null;
            string query = "SELECT ";
            if (column != null) query += $"{column} FROM {table}";
            else query += $"* FROM {table}";
            if (where != null) query += $" WHERE {where}";
            var cmd = new MySqlCommand(query, srv);
            try
            {
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public void set()
        {
            DateTime date = DateTime.Now;
            Random r = new Random();
            date = date.AddSeconds(-date.Second);
            date = date.AddMinutes(-date.Minute);

            for (int x = 0; x < 100; x++)
            {
                string query = "INSERT INTO relevemeteo (DateHeureReleve, Temperature, Hygrometrie, VitesseVent, DirectionVent, PressionAtmospherique, Pluviometre, RayonnementSolaire) VALUES";
                for (int i = 0; i != 500; i++)
                {
                    query += $"\n('{date.ToString("yyyy-MM-dd HH:mm:")}00.000000', {r.Next(101)}, {r.Next(101)}, {r.Next(101)}, {r.Next(365)}, {r.Next(101)}, {r.Next(101)}, {r.Next(101)})";
                    if (i != 499) query += ",";
                    else query += ";";
                    date = date.AddMinutes(-30);
                }
                Console.Write(query);
                var cmd = new MySqlCommand(query, srv);
                cmd.ExecuteReader().Close();
            }
        }
    }
}