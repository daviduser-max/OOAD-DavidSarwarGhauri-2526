using System;
using Microsoft.Data.Sqlite;

namespace CLDierenarts
{
    public class Kat : Dier
    {
        public bool IsGevaccineerd { get; set; }

        public Kat() : base() { }

        public Kat(SqliteDataReader reader) : base(reader)
        {
            int isGevaccineerdOrdinal = reader.GetOrdinal("isGevaccineerd");
            IsGevaccineerd = !reader.IsDBNull(isGevaccineerdOrdinal) && Convert.ToInt32(reader.GetValue(isGevaccineerdOrdinal)) == 1;
        }

        public override string GeefInfo()
        {
            string vaccStatus = IsGevaccineerd ? "Yes" : "No";
            return base.GeefInfo() + $"\nVaccinated: {vaccStatus}";
        }
    }
}
