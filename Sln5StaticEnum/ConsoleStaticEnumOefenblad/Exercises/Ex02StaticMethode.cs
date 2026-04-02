using ConsoleStaticEnumOefenblad.Exercises.Classes;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex02StaticMethode
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 2: couponcodes controleren");
        Console.WriteLine("-------------");

        // 1. Maak in "Exercises/Classes" een klasse "CouponCode":
        //   - private static string _couponRegex = @"^[A-Z]{3}\d{2}-[A-Z]{2}$";
        //   - property "Code" van type string.
        //   - property "IsGeldig" van type bool met alleen getter:
        //     gebruik Regex.IsMatch(...) om te controleren of Code geldig is
        //   - constructor met één parameter
        //
        // 2. Voeg vervolgens statische methode ControleerCode(string code) toe, die toelaat een gegeven code te controleren
        //
        // 3. Voeg tenslotte nog een statische methode Beschrijf(string code) toe: 
        //   - als de code geldig is, geef je een tekst terug in dit formaat:
        //     "Prefix=ABC, Nummer=12, Regio=DE"
        //   - als de code ongeldig is, geef je "ongeldige code" terug.

        // Testcode (haal uit commentaar):

        string[] codes = { "ABC12-DE", "AB12-DE", "XYZ99-BE" };
        Console.WriteLine("\ntesten IsGeldig() methode:\n");
        foreach (string code in codes)
        {
            Console.WriteLine($"Code {code} is {(CouponCode.ControleerCode(code) ? "geldig" : "ongeldig")}");
        }
        Console.WriteLine("\ntesten Beschrijf() methode:\n");
        foreach (string code in codes) 
        {
            Console.WriteLine($"Code {code}: {CouponCode.Beschrijf(code)}");
        }

    }
}