using System;
using Microsoft.Data.Sqlite;

namespace CLDierenarts
{
    public class Hond : Dier
    {
        public string Ras { get; set; }

        public Hond() : base() { }

        public Hond(SqliteDataReader reader) : base(reader)
        {
            int rasOrdinal = reader.GetOrdinal("ras");
            Ras = reader.IsDBNull(rasOrdinal) ? null : reader.GetString(rasOrdinal);
        }

        public override string GeefInfo()
        {
            return base.GeefInfo() + $"\nBreed (Ras): {Ras ?? "Unknown"}";
        }
    }
}
