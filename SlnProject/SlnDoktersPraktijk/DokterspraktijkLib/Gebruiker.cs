using System;
using System.Security.Cryptography;
using System.Text;

namespace DokterspraktijkLib
{
    /// <summary>
    /// Superklasse voor patiënt en dokter met gemeenschappelijke gegevens.
    /// </summary>
    public class Gebruiker
    {
        public int Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Email { get; set; }
        public string Passwoord { get; set; }
        public string Gsm { get; set; }
        public byte[] Profielfotodata { get; set; }

        public string VolledigeNaam
        {
            get { return Voornaam + " " + Achternaam; }
        }

        public override string ToString()
        {
            return VolledigeNaam;
        }

        /// <summary>
        /// Maakt een SHA256-hash van een wachtwoord (zoals in de databank opgeslagen).
        /// </summary>
        protected static string HashWachtwoord(string wachtwoord)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(wachtwoord);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();

                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Vergelijkt ingevoerd wachtwoord met de hash uit de databank.
        /// </summary>
        protected static bool WachtwoordKlopt(string ingevoerdWachtwoord, string opgeslagenHash)
        {
            string ingegevenHash = HashWachtwoord(ingevoerdWachtwoord).Trim();
            string hashUitDb = opgeslagenHash.Trim();
            return string.Equals(hashUitDb, ingegevenHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
