using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokterspraktijkLib
{
    public enum NotificatieType 
    {
        Geen,
        Mail,
        Sms,
        Beide
    }

    public class Patient : Gebruiker 
    {
    public NotificatieType Notificatie { get; set; }

        public Patient() {
            
        }
    
    }
}
