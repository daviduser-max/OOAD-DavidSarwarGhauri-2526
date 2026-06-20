using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.Sqlite;

namespace CLDierenarts
{
    public class Eigenaar
    {
        // Connection string loaded from App.config
        private static readonly string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        public string Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }

        public string VolledigeNaam => $"{Voornaam} {Achternaam}";

        // Parameterless constructor
        public Eigenaar() { }

        // Constructor from Reader
        public Eigenaar(SqliteDataReader reader)
        {
            Id = Convert.ToString(reader["id"]);
            Voornaam = Convert.ToString(reader["voornaam"]);
            Achternaam = Convert.ToString(reader["achternaam"]);
        }

        public static List<Eigenaar> GetAll()
        {
            List<Eigenaar> list = new List<Eigenaar>();
            using (SqliteConnection conn = new SqliteConnection(connString))
            {
                conn.Open();
                using (SqliteCommand comm = new SqliteCommand("SELECT * FROM eigenaars ORDER BY achternaam, voornaam", conn))
                {
                    using (SqliteDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Eigenaar(reader));
                        }
                    }
                }
            }
            return list;
        }

        public static Eigenaar GetById(string id)
        {
            using (SqliteConnection conn = new SqliteConnection(connString))
            {
                conn.Open();
                using (SqliteCommand comm = new SqliteCommand("SELECT * FROM eigenaars WHERE id = @id", conn))
                {
                    comm.Parameters.AddWithValue("@id", id);
                    using (SqliteDataReader reader = comm.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Eigenaar(reader);
                        }
                    }
                }
            }
            return null;
        }

        public override string ToString()
        {
            return VolledigeNaam;
        }
    }
}
