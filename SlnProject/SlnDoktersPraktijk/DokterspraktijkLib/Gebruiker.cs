using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokterspraktijkLib
{
     public class Gebruiker
    {
        public int Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
       
        public string Email {  get; set; }

        public string Passwoord { get; set; }

        public string Gsm {  get; set; }

        public Byte[] Profielfotodata {  get; set; }

        public string VolledigeNaam => $"{Voornaam} {Achternaam}";



    }
}
