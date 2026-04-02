using System;
using System.Collections.Generic;
using System.Text;
using ConsoleStaticEnumOefenblad.Exercises.Enums;
namespace ConsoleStaticEnumOefenblad.Exercises.Classes
{
    internal class Bestelling
    {
        public string KlantNaam { get; set; }
        public string ProductNaam {  get; set; }
        public BestelStatus Status { get; set; }

        public bool KanNogGewijzigdWorden =>
            Status != BestelStatus.Geleverd && Status != BestelStatus.Geannuleerd;

        public override string ToString()
        {
            return $"{ProductNaam} - {Status} - wijzigbaar: {(KanNogGewijzigdWorden ? "ja" : "nee")}";
        }
    }
}
