using ConsoleStaticEnumOefenblad.Exercises.Classes;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex03StaticClass
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 3: tekst analyseren");
        Console.WriteLine("-------------");

        //  1. Maak in "Exercises/Classes" een statische class "TekstAnalyse":
        //  - private variabele verbodenWoorden: een array van strings met de woorden "delete", "drop", "truncate"
        //  - private variabele verbodenKarakters: een array van chars met de tekens "!", "@", "#", "$", "%"
        // 
        //  2. Voeg deze statische methodes toe:
        //  - AantalWoorden(string tekst): telt woorden op basis van spaties
        //  - BevatVerbodenWoord(string tekst): geeft true terug als de tekst verboden woorden bevat
        //  - BevatVerbodenKarakter(string tekst): geeft true terug als de tekst verboden karakters bevat
        //  - IsGeschiktVoorTitel(string tekst): geeft true terug als:
        //   * de tekst niet leeg is
        //   * de tekst minimum 5 en maximum 30 tekens lang is
        //   * de tekst geen verboden woorden of karakters bevat
        //
        //  Zorg ervoor dat de methodes ook veilig omgaan met null of lege strings.

        // Testcode (haal uit commentaar):

        Console.WriteLine();
        string[] teksten = new string[]
        {
            "dit is een test",
            "gratis truncate aanbieding",
            "Workshop C# basis",
            "delete titel",
            ""
        };
        foreach (string tekst in teksten) 
        { 
            int aantalWoorden = TekstAnalyse.AantalWoorden(tekst);
            bool bevatVerbodenWoord = TekstAnalyse.BevatVerbodenWoord(tekst);
            bool bevatVerbodenKarakter = TekstAnalyse.BevatVerbodenKarakter(tekst);
            bool isGeschiktVoorTitel = TekstAnalyse.IsGeschiktVoorTitel(tekst);
            Console.WriteLine($"'{tekst}':");
            Console.WriteLine($" - bevat {aantalWoorden} woorden");
            if (bevatVerbodenWoord) Console.WriteLine(" - bevat verboden woord");
            if (bevatVerbodenKarakter) Console.WriteLine(" - bevat verboden karakter");
            Console.WriteLine($" - is {(isGeschiktVoorTitel ? "geschikt" : "niet geschikt")} voor een titel");
        }
    }
}