using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.Sqlite;

namespace CLDierenarts
{
    public abstract class Dier
    {
        protected static readonly string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        public int Id { get; set; }
        public string Naam { get; set; }
        public string EigenaarId { get; set; }
        public DateTime Geboortedatum { get; set; }
        public double Gewicht { get; set; }
        public Urgentie Urgentie { get; set; }
        public bool IsOpgenomen { get; set; }
        public DateTime? DatumOpgenomen { get; set; }

        public Eigenaar Eigenaar => Eigenaar.GetById(EigenaarId);

        public Dier() { }

        public Dier(SqliteDataReader reader)
        {
            Id = Convert.ToInt32(reader["id"]);
            Naam = Convert.ToString(reader["naam"]);
            EigenaarId = Convert.ToString(reader["eigenaarId"]);
            Geboortedatum = Convert.ToDateTime(reader["geboortedatum"]);
            Gewicht = Convert.ToDouble(reader["gewicht"]);
            Urgentie = (Urgentie)Enum.Parse(typeof(Urgentie), Convert.ToString(reader["urgentie"]));
            IsOpgenomen = Convert.ToInt32(reader["isOpgenomen"]) == 1;
            
            int datumOpgenomenOrdinal = reader.GetOrdinal("datumOpgenomen");
            DatumOpgenomen = reader.IsDBNull(datumOpgenomenOrdinal) || string.IsNullOrEmpty(reader.GetString(datumOpgenomenOrdinal))
                ? (DateTime?)null
                : (DateTime?)Convert.ToDateTime(reader.GetString(datumOpgenomenOrdinal));
        }

        public virtual string GeefInfo()
        {
            string opnameStatus = IsOpgenomen 
                ? $"Admitted on: {DatumOpgenomen?.ToString("dd-MM-yyyy HH:mm")}" 
                : "Not currently admitted";

            return $"ID: {Id}\n" +
                   $"Name: {Naam}\n" +
                   $"Owner: {Eigenaar?.VolledigeNaam ?? "Unknown"} ({EigenaarId})\n" +
                   $"Birthdate: {Geboortedatum.ToString("dd-MM-yyyy")}\n" +
                   $"Weight: {Gewicht} kg\n" +
                   $"Urgency: {Urgentie}\n" +
                   $"Status: {opnameStatus}";
        }

        public override string ToString()
        {
            string type = this is Kat ? "Kat" : "Hond";
            string opnameIndicator = IsOpgenomen ? " [OPGENOMEN]" : "";
            return $"{Naam} ({type}){opnameIndicator}";
        }

        public void Opnemen()
        {
            IsOpgenomen = true;
            DatumOpgenomen = DateTime.Now;
            UpdateInDb();
        }

        public int InsertToDb()
        {
            using (SqliteConnection conn = new SqliteConnection(connString))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO dieren (naam, eigenaarId, geboortedatum, gewicht, urgentie, type, ras, isGevaccineerd, isOpgenomen, datumOpgenomen)
                    VALUES (@naam, @eigenaarId, @geboortedatum, @gewicht, @urgentie, @type, @ras, @isGevaccineerd, @isOpgenomen, @datumOpgenomen);
                    SELECT last_insert_rowid();";

                using (SqliteCommand comm = new SqliteCommand(sql, conn))
                {
                    comm.Parameters.AddWithValue("@naam", Naam);
                    comm.Parameters.AddWithValue("@eigenaarId", EigenaarId);
                    comm.Parameters.AddWithValue("@geboortedatum", Geboortedatum.ToString("yyyy-MM-dd"));
                    comm.Parameters.AddWithValue("@gewicht", Gewicht);
                    comm.Parameters.AddWithValue("@urgentie", Urgentie.ToString());
                    comm.Parameters.AddWithValue("@isOpgenomen", IsOpgenomen ? 1 : 0);
                    comm.Parameters.AddWithValue("@datumOpgenomen", DatumOpgenomen.HasValue ? DatumOpgenomen.Value.ToString("yyyy-MM-ddTHH:mm:ss") : DBNull.Value);

                    if (this is Kat kat)
                    {
                        comm.Parameters.AddWithValue("@type", "Kat");
                        comm.Parameters.AddWithValue("@ras", DBNull.Value);
                        comm.Parameters.AddWithValue("@isGevaccineerd", kat.IsGevaccineerd ? 1 : 0);
                    }
                    else if (this is Hond hond)
                    {
                        comm.Parameters.AddWithValue("@type", "Hond");
                        comm.Parameters.AddWithValue("@ras", hond.Ras ?? (object)DBNull.Value);
                        comm.Parameters.AddWithValue("@isGevaccineerd", DBNull.Value);
                    }

                    Id = Convert.ToInt32(comm.ExecuteScalar());
                    return Id;
                }
            }
        }

        public void UpdateInDb()
        {
            using (SqliteConnection conn = new SqliteConnection(connString))
            {
                conn.Open();
                string sql = @"
                    UPDATE dieren
                    SET naam = @naam, 
                        eigenaarId = @eigenaarId, 
                        geboortedatum = @geboortedatum, 
                        gewicht = @gewicht, 
                        urgentie = @urgentie, 
                        type = @type, 
                        ras = @ras, 
                        isGevaccineerd = @isGevaccineerd, 
                        isOpgenomen = @isOpgenomen, 
                        datumOpgenomen = @datumOpgenomen
                    WHERE id = @id";

                using (SqliteCommand comm = new SqliteCommand(sql, conn))
                {
                    comm.Parameters.AddWithValue("@naam", Naam);
                    comm.Parameters.AddWithValue("@eigenaarId", EigenaarId);
                    comm.Parameters.AddWithValue("@geboortedatum", Geboortedatum.ToString("yyyy-MM-dd"));
                    comm.Parameters.AddWithValue("@gewicht", Gewicht);
                    comm.Parameters.AddWithValue("@urgentie", Urgentie.ToString());
                    comm.Parameters.AddWithValue("@isOpgenomen", IsOpgenomen ? 1 : 0);
                    comm.Parameters.AddWithValue("@datumOpgenomen", DatumOpgenomen.HasValue ? DatumOpgenomen.Value.ToString("yyyy-MM-ddTHH:mm:ss") : DBNull.Value);
                    comm.Parameters.AddWithValue("@id", Id);

                    if (this is Kat kat)
                    {
                        comm.Parameters.AddWithValue("@type", "Kat");
                        comm.Parameters.AddWithValue("@ras", DBNull.Value);
                        comm.Parameters.AddWithValue("@isGevaccineerd", kat.IsGevaccineerd ? 1 : 0);
                    }
                    else if (this is Hond hond)
                    {
                        comm.Parameters.AddWithValue("@type", "Hond");
                        comm.Parameters.AddWithValue("@ras", hond.Ras ?? (object)DBNull.Value);
                        comm.Parameters.AddWithValue("@isGevaccineerd", DBNull.Value);
                    }

                    comm.ExecuteNonQuery();
                }
            }
        }

        public static List<Dier> GetAll()
        {
            List<Dier> list = new List<Dier>();
            using (SqliteConnection conn = new SqliteConnection(connString))
            {
                conn.Open();
                using (SqliteCommand comm = new SqliteCommand("SELECT * FROM dieren ORDER BY naam", conn))
                {
                    using (SqliteDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string type = Convert.ToString(reader["type"]);
                            if (type == "Kat")
                            {
                                list.Add(new Kat(reader));
                            }
                            else if (type == "Hond")
                            {
                                list.Add(new Hond(reader));
                            }
                        }
                    }
                }
            }
            return list;
        }

        public static Dier GetById(int id)
        {
            using (SqliteConnection conn = new SqliteConnection(connString))
            {
                conn.Open();
                using (SqliteCommand comm = new SqliteCommand("SELECT * FROM dieren WHERE id = @id", conn))
                {
                    comm.Parameters.AddWithValue("@id", id);
                    using (SqliteDataReader reader = comm.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string type = Convert.ToString(reader["type"]);
                            if (type == "Kat") return new Kat(reader);
                            if (type == "Hond") return new Hond(reader);
                        }
                    }
                }
            }
            return null;
        }
    }
}
