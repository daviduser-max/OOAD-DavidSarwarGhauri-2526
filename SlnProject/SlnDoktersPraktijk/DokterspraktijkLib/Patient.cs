using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokterspraktijkLib
{
    internal class Patient
    {
        public enum NotificatieType { Geen, Mail, Sms, Biede}

        public class Patient : Gebruiker 
        {
            public NotificatieType Notificatie {  get; set; }
        }
    }
}
