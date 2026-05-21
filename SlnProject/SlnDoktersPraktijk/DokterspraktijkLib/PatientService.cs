using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace DokterspraktijkLib
{
    public static  class PatientService
    {
        public static Patient ValideerInloggen(string email, string wachtwoord)
        {
            using (SqlConnection conn = Database.GetConnection()) {
                string query = "SELECT * FROM Patient WHERE email = @Email";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                string gehashedWachtwoord = HashWachtwoord(wachtwoord);
                System.Diagnostics.Debug.WriteLine(gehashedWachtwoord);
                cmd.Parameters.AddWithValue("@Passwoord", gehashedWachtwoord);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("Email gevonden");
                        Patient patient = new Patient
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Voornaam = reader["Voornaam"].ToString(),
                            Achternaam = reader["achternaam"].ToString(),
                            Email = reader["email"].ToString(),
                            Gsm = reader["gsm"].ToString()
                        };
                        return patient;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Database fout: " + ex.Message);
                }
                return null;
             }
       }

        private static string HashWachtwoord(string wachtwoord)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(wachtwoord);

            byte[] hash = sha256.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();

            for(int i =0; i <hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString().ToUpper();

        }
    }
}
