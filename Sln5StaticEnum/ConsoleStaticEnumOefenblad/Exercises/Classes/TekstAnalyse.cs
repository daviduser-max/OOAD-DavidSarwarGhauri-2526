using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace ConsoleStaticEnumOefenblad.Exercises.Classes
{
    internal class TekstAnalyse
    {
        private static string[] verbodenWoorden = { "delete", "drop", "truncate"};
        private static char[] verbodenKarakters = { '!', '@', '#', '$', '%' };

        public static int AantalWoorden(string tekst)
        {
            if(string.IsNullOrWhiteSpace(tekst))
                return 0;
            return tekst.Split(' ').Length;

        }

        public static bool BevatVerbodenWoord(string tekst) 
        {
            if(string.IsNullOrWhiteSpace(tekst))
                return false;
            foreach(string woord in verbodenWoorden) 
            {
                if(tekst.Contains(woord))  
                       return true;
            }
            return false;
        }


        public static bool BevatVerbodenKarakter(string tekst) 
        {  if(string.IsNullOrWhiteSpace(tekst))
                return false;
        foreach(char karakter in verbodenKarakters)
            {
                if(tekst.Contains(karakter))
                    return true;
            }
        return false;
            
                    
        }
         
            

        public static bool IsGeschiktVoorTitel(string tekst) 
        {
            if(string .IsNullOrWhiteSpace(tekst))
                return false;
            if(tekst.Length < 5 || tekst.Length > 30)
                return false;
            if(BevatVerbodenWoord(tekst))
                return false;
            if(BevatVerbodenKarakter(tekst))
                return false;
            return true;
        }


    }
}
