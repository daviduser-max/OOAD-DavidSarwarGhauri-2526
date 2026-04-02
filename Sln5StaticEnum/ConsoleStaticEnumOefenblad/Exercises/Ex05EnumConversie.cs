using ConsoleStaticEnumOefenblad.Exercises.Classes;
using ConsoleStaticEnumOefenblad.Exercises.Enums;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex05EnumConversie
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 5: enum conversies");
        Console.WriteLine("-------------");

        // 1. Maak in "Exercises/Classes" een enum "Prioriteit" met deze waarden:
        //  - Laag
        //  - Normaal
        //  - Hoog
        //  - Kritiek
        //
        //  Test daarna:
        //  - een enumwaarde tonen als tekst
        //  - dezelfde enumwaarde omzetten naar int
        //  - een int omzetten naar enum

        // Testcode (haal uit commentaar):

        Prioriteit p1 = Prioriteit.Hoog;
        Console.WriteLine($"Enumwaarde: {p1}");

        int cijfer = (int)p1;
        Console.WriteLine($"Als int: {cijfer}");

        Prioriteit p2 = p1 + 1;
        Console.WriteLine($"Nog hogere prioriteit: {p2}");
    }
}