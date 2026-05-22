using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DokterspraktijkLib
{
    public class Afspraak
    {
        public int Id { get; set; }
        public DateTime Moment { get; set; }
        public string Klacht { get; set; }
        public int PatientId { get; set; }
        public int DokterId { get; set; }

        public Patient Patient { get; set; }
        public Dokter Dokter { get; set; }

        public Afspraak()
        {
        }

        public Afspraak(DateTime moment, string klacht, int patientId, int dokterId)
        {
            Moment = moment;
            Klacht = klacht;
            PatientId = patientId;
            DokterId = dokterId;
        }

        public static List<Afspraak> GetByDokterAndDate(int dokterId, DateTime datum)
        {
            List<Afspraak> afspraken = new List<Afspraak>();

            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, moment, klacht, patient_id, dokter_id FROM Afspraak WHERE dokter_id = @DokterId AND CAST(moment AS Date) = CAST(@Datum AS Date) ORDER BY moment";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DokterId", dokterId);
                cmd.Parameters.AddWithValue("@Datum", datum.Date);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Afspraak afspraak = new Afspraak();
                        afspraak.Id = Convert.ToInt32(reader["id"]);
                        afspraak.Moment = Convert.ToDateTime(reader["moment"]);
                        afspraak.Klacht = reader["klacht"].ToString();
                        afspraak.PatientId = Convert.ToInt32(reader["patient_id"]);
                        afspraak.DokterId = Convert.ToInt32(reader["dokter_id"]);
                        afspraken.Add(afspraak);
                    }
                }
            }

            foreach (Afspraak afspraak in afspraken)
            {
                afspraak.Patient = Patient.Read(afspraak.PatientId);
                afspraak.Dokter = Dokter.Read(afspraak.DokterId);
            }

            return afspraken;
        }

        public static List<Afspraak> GetByPatient(int patientId)
        {
            List<Afspraak> afspraken = new List<Afspraak>();

            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT id, moment, klacht, patient_id, dokter_id FROM Afspraak WHERE patient_id = @PatientId ORDER BY moment DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Afspraak afspraak = new Afspraak();
                        afspraak.Id = Convert.ToInt32(reader["id"]);
                        afspraak.Moment = Convert.ToDateTime(reader["moment"]);
                        afspraak.Klacht = reader["klacht"].ToString();
                        afspraak.PatientId = Convert.ToInt32(reader["patient_id"]);
                        afspraak.DokterId = Convert.ToInt32(reader["dokter_id"]);
                        afspraken.Add(afspraak);
                    }
                }
            }

            foreach (Afspraak afspraak in afspraken)
            {
                afspraak.Patient = Patient.Read(afspraak.PatientId);
                afspraak.Dokter = Dokter.Read(afspraak.DokterId);
            }

            return afspraken;
        }

        public bool Create()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "INSERT INTO Afspraak (moment, klacht, patient_id, dokter_id) " +
                               "VALUES (@Moment, @Klacht, @PatientId, @DokterId); " +
                               "SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Moment", Moment);
                cmd.Parameters.AddWithValue("@Klacht", Klacht);
                cmd.Parameters.AddWithValue("@PatientId", PatientId);
                cmd.Parameters.AddWithValue("@DokterId", DokterId);

                conn.Open();
                object newId = cmd.ExecuteScalar();

                if (newId != null && newId != DBNull.Value)
                {
                    Id = Convert.ToInt32(newId);
                    return true;
                }
            }

            return false;
        }

        public bool Delete()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                string query = "DELETE FROM Afspraak WHERE id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", Id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
