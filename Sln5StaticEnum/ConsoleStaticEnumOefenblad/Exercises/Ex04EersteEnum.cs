using ConsoleStaticEnumOefenblad.Exercises.Classes;
using ConsoleStaticEnumOefenblad.Exercises.Enums;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex04EersteEnum
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 4: bestelstatus");
        Console.WriteLine("-------------");

        // 1. Maak in "Exercises/Classes" een enum "BestelStatus" met deze waarden:
        //  - Nieuw
        //  - InBehandeling
        //  - Verzonden
        //  - Geleverd
        //  - Geannuleerd
        //
        // 2. Maak daarna een klasse "Bestelling" met:
        //  - KlantNaam
        //  - ProductNaam
        //  - Status
        //
        // 3. Voeg ook een property KanNogGewijzigdWorden toe:
        //  - niet elke status laat nog wijzigingen toe; zorg dus dat de methode enkel true teruggeeft wanneer dat logisch is.
        //
        // 4. Voeg een override van ToString() toe die de klantnaam, productnaam en bestelstatus netjes weergeeft (zie screenshot).

        // Tescode (haal uit commentaar):

        Bestelling b1 = new Bestelling
        {
            KlantNaam = "Sara",
            ProductNaam = "Laptop",
            Status = BestelStatus.Nieuw
        };
        Bestelling b2 = new Bestelling
        {
            KlantNaam = "Imran",
            ProductNaam = "Muis",
            Status = BestelStatus.Verzonden
        };
        Console.WriteLine(b1);
        Console.WriteLine(b2);
    }
}