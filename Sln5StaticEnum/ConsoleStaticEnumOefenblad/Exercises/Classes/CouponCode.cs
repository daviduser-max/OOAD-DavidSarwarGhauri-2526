using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleStaticEnumOefenblad.Exercises.Classes
{
    internal class CouponCode
    {
        private static string _couponRegex = @"^[A-Z]{3}\d{2}-[A-Z]{2}$";
        public string Code { get; set; }
        public bool IsGeldig => Regex.IsMatch(Code, _couponRegex);

        public CouponCode(string code) 
        {
            Code = code;
        }

        public static bool ControleerCode(string code) 
        {
            return Regex.IsMatch( code, _couponRegex);   
        }

        public static string Beschrijf(string code) 
        {
            if(!ControleerCode(code))
                return "ongeldige code";
            string prefix = code.Substring(0, 3);
            string nummer = code.Substring(3, 2);
            string regio = code.Substring(6, 2);
            return $"Prefix={prefix}, Nummer={nummer}, Regio={regio}";  
        }
    }
}
