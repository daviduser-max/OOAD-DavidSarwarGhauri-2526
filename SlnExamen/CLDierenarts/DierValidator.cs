using System;

namespace CLDierenarts
{
    public class DierValidator
    {
        public int MinRasLengte { get; set; } = 3;

        public bool IsGeldigNaam(string naam)
        {
            if (string.IsNullOrWhiteSpace(naam))
            {
                return false;
            }

            foreach (char c in naam)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-')
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsGeldigRas(string ras)
        {
            if (string.IsNullOrWhiteSpace(ras))
            {
                return false;
            }

            return ras.Trim().Length >= MinRasLengte;
        }
    }
}
