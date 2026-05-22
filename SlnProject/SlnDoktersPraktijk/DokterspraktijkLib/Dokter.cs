using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DokterspraktijkLib
{
    public class Dokter : Gebruiker
    {
        public int RizivNummer { get; set; }
        public bool IsGeconventioneerd { get; set; }

        public Dokter()
        {
        }

        public static Dokter ValideerInloggen(string email, string password)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd FROM Dokter WHERE email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && WachtwoordKlopt(password, reader["paswoord"].ToString()))
                    {
                        Dokter dokter = new Dokter();
                        dokter.Id = Convert.ToInt32(reader["id"]);
                        dokter.Voornaam = reader["voornaam"].ToString();
                        dokter.Achternaam = reader["achternaam"].ToString();
                        dokter.Email = reader["email"].ToString();
                        dokter.Passwoord = reader["paswoord"].ToString();

                        if (reader["gsm"] != DBNull.Value)
                        {
                            dokter.Gsm = reader["gsm"].ToString().Trim();
                        }
                        else
                        {
                            dokter.Gsm = "";
                        }

                        if (reader["profielfotodata"] != DBNull.Value)
                        {
                            dokter.Profielfotodata = (byte[])reader["profielfotodata"];
                        }
                        else
                        {
                            dokter.Profielfotodata = null;
                        }

                        dokter.RizivNummer = Convert.ToInt32(reader["rizivnummer"]);
                        dokter.IsGeconventioneerd = Convert.ToInt32(reader["isgeconventioneerd"]) == 1;
                        return dokter;
                    }
                }
            }

            return null;
        }

        public static List<Dokter> GetDokters()
        {
            List<Dokter> dokters = new List<Dokter>();

            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd FROM Dokter ORDER BY achternaam, voornaam";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dokter dokter = new Dokter();
                        dokter.Id = Convert.ToInt32(reader["id"]);
                        dokter.Voornaam = reader["voornaam"].ToString();
                        dokter.Achternaam = reader["achternaam"].ToString();
                        dokter.Email = reader["email"].ToString();
                        dokter.Passwoord = reader["paswoord"].ToString();

                        if (reader["gsm"] != DBNull.Value)
                        {
                            dokter.Gsm = reader["gsm"].ToString().Trim();
                        }
                        else
                        {
                            dokter.Gsm = "";
                        }

                        if (reader["profielfotodata"] != DBNull.Value)
                        {
                            dokter.Profielfotodata = (byte[])reader["profielfotodata"];
                        }
                        else
                        {
                            dokter.Profielfotodata = null;
                        }

                        dokter.RizivNummer = Convert.ToInt32(reader["rizivnummer"]);
                        dokter.IsGeconventioneerd = Convert.ToInt32(reader["isgeconventioneerd"]) == 1;
                        dokters.Add(dokter);
                    }
                }
            }

            return dokters;
        }

        public static Dokter Read(int id)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd FROM Dokter WHERE id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Dokter dokter = new Dokter();
                        dokter.Id = Convert.ToInt32(reader["id"]);
                        dokter.Voornaam = reader["voornaam"].ToString();
                        dokter.Achternaam = reader["achternaam"].ToString();
                        dokter.Email = reader["email"].ToString();
                        dokter.Passwoord = reader["paswoord"].ToString();

                        if (reader["gsm"] != DBNull.Value)
                        {
                            dokter.Gsm = reader["gsm"].ToString().Trim();
                        }
                        else
                        {
                            dokter.Gsm = "";
                        }

                        if (reader["profielfotodata"] != DBNull.Value)
                        {
                            dokter.Profielfotodata = (byte[])reader["profielfotodata"];
                        }
                        else
                        {
                            dokter.Profielfotodata = null;
                        }

                        dokter.RizivNummer = Convert.ToInt32(reader["rizivnummer"]);
                        dokter.IsGeconventioneerd = Convert.ToInt32(reader["isgeconventioneerd"]) == 1;
                        return dokter;
                    }
                }
            }

            throw new KeyNotFoundException("Dokter met ID " + id + " is niet gevonden.");
        }
    }
}
