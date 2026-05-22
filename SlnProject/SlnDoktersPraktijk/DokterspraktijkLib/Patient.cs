using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DokterspraktijkLib
{
    public enum NotificatieType
    {
        Geen = 0,
        Mail = 1,
        Sms = 2,
        Beide = 3
    }

    public class Patient : Gebruiker
    {
        public int Geslacht { get; set; }
        public DateTime Geboortedatum { get; set; }
        public NotificatieType Notificatie { get; set; }

        public Patient()
        {
        }

        public static Patient ValideerInloggen(string email, string password)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties FROM Patient WHERE email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && WachtwoordKlopt(password, reader["paswoord"].ToString()))
                    {
                        Patient patient = new Patient();
                        patient.LeesVanReader(reader);
                        return patient;
                    }
                }
            }

            return null;
        }

        public static List<Patient> GetPatients()
        {
            List<Patient> patienten = new List<Patient>();

            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties FROM Patient ORDER BY achternaam, voornaam";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Patient patient = new Patient();
                        patient.LeesVanReader(reader);
                        patienten.Add(patient);
                    }
                }
            }

            return patienten;
        }

        public static List<Patient> ZoekOpNaam(string zoekterm)
        {
            if (string.IsNullOrWhiteSpace(zoekterm))
            {
                return GetPatients();
            }

            List<Patient> patienten = new List<Patient>();

            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties FROM Patient WHERE voornaam LIKE @Zoek OR achternaam LIKE @Zoek ORDER BY achternaam, voornaam";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Zoek", "%" + zoekterm.Trim() + "%");

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Patient patient = new Patient();
                        patient.LeesVanReader(reader);
                        patienten.Add(patient);
                    }
                }
            }

            return patienten;
        }

        public static Patient Read(int id)
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties FROM Patient WHERE id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Patient patient = new Patient();
                        patient.LeesVanReader(reader);
                        return patient;
                    }
                }
            }

            throw new KeyNotFoundException("Patient met ID " + id + " is niet gevonden.");
        }

        public bool Create()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "INSERT INTO Patient (voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties) " +
                               "VALUES (@Voornaam, @Achternaam, @Geslacht, @Gsm, @Email, @Paswoord, @Geboortedatum, @Profielfotodata, @Notificaties)";

                SqlCommand cmd = new SqlCommand(query, conn);
                VulParameters(cmd, true);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "UPDATE Patient SET voornaam = @Voornaam, achternaam = @Achternaam, geslacht = @Geslacht, " +
                               "gsm = @Gsm, email = @Email, paswoord = @Paswoord, geboortedatum = @Geboortedatum, " +
                               "profielfotodata = @Profielfotodata, notificaties = @Notificaties WHERE id = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);

                bool isAlGehashed = false;
                if (Passwoord != null && Passwoord.Length == 64)
                {
                    isAlGehashed = true;
                    foreach (char c in Passwoord)
                    {
                        if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                        {
                            isAlGehashed = false;
                            break;
                        }
                    }
                }

                VulParameters(cmd, !isAlGehashed);
                cmd.Parameters.AddWithValue("@Id", Id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                string queryAfspraken = "DELETE FROM Afspraak WHERE patient_id = @PatientId";
                SqlCommand cmdAfspraken = new SqlCommand(queryAfspraken, conn);
                cmdAfspraken.Parameters.AddWithValue("@PatientId", Id);
                cmdAfspraken.ExecuteNonQuery();

                string queryPatient = "DELETE FROM Patient WHERE id = @Id";
                SqlCommand cmdPatient = new SqlCommand(queryPatient, conn);
                cmdPatient.Parameters.AddWithValue("@Id", Id);

                int rowsAffected = cmdPatient.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        private void LeesVanReader(SqlDataReader reader)
        {
            Id = Convert.ToInt32(reader["id"]);
            Voornaam = reader["voornaam"].ToString();
            Achternaam = reader["achternaam"].ToString();
            Geslacht = Convert.ToInt32(reader["geslacht"]);
            Email = reader["email"].ToString();
            Passwoord = reader["paswoord"].ToString();
            Geboortedatum = Convert.ToDateTime(reader["geboortedatum"]);
            Notificatie = (NotificatieType)Convert.ToInt32(reader["notificaties"]);

            if (reader["gsm"] != DBNull.Value)
            {
                Gsm = reader["gsm"].ToString().Trim();
            }
            else
            {
                Gsm = "";
            }

            if (reader["profielfotodata"] != DBNull.Value)
            {
                Profielfotodata = (byte[])reader["profielfotodata"];
            }
            else
            {
                Profielfotodata = null;
            }
        }

        private void VulParameters(SqlCommand cmd, bool hashPaswoord)
        {
            cmd.Parameters.AddWithValue("@Voornaam", Voornaam);
            cmd.Parameters.AddWithValue("@Achternaam", Achternaam);
            cmd.Parameters.AddWithValue("@Geslacht", Geslacht);
            cmd.Parameters.AddWithValue("@Gsm", Gsm);
            cmd.Parameters.AddWithValue("@Email", Email);

            if (hashPaswoord)
            {
                cmd.Parameters.AddWithValue("@Paswoord", HashWachtwoord(Passwoord));
            }
            else
            {
                cmd.Parameters.AddWithValue("@Paswoord", Passwoord);
            }

            cmd.Parameters.AddWithValue("@Geboortedatum", Geboortedatum);
            cmd.Parameters.AddWithValue("@Notificaties", (int)Notificatie);

            if (Profielfotodata != null)
            {
                cmd.Parameters.Add("@Profielfotodata", System.Data.SqlDbType.Image).Value = Profielfotodata;
            }
            else
            {
                cmd.Parameters.Add("@Profielfotodata", System.Data.SqlDbType.Image).Value = DBNull.Value;
            }
        }
    }
}
