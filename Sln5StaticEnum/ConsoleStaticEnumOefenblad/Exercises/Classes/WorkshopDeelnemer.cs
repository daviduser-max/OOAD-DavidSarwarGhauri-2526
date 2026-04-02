namespace ConsoleStaticEnumOefenblad.Exercises.Classes;

internal class WorkshopDeelnemer
{
    public string Naam { get; set; }
    public bool IsAanwezig { get; private set; }
    public static int AantalAangemaakt { get; private set; }
    public static int AantalAanwezig { get; private set; }

    public WorkshopDeelnemer(string naam, bool isAanwezig)
    {
        Naam = naam;
        IsAanwezig = isAanwezig;
        AantalAangemaakt++;
        if (isAanwezig)
            AantalAanwezig++;
    }

    public void ZetAfwezig() 
    {
        if (IsAanwezig) 
        {
            IsAanwezig = false;
            AantalAanwezig--;
        }    
    }

    
}