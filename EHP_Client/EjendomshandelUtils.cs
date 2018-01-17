using System;
using System.Linq;
using EHP_Client.ServiceReferenceEjendomshandel;


namespace EHP_Client
{
    public static class EjendomshandelUtils
    {
        public static string GetEndpointAddress(Miljoe miljoe)
        {
            string s = "";
            switch (miljoe)
            {
                case Miljoe.Test:
                    s = "https://test-bolighandel.e-nettet.dk/efpi/ejendomshandel/Ejendomshandel.eFPI";
                    break;
                case Miljoe.Staging:
                    s = "https://staging-bolighandel.e-nettet.dk/efpi/ejendomshandel/Ejendomshandel.eFPI";
                    break;
                case Miljoe.Produktion:
                    s = "https://e-bolighandel.e-nettet.dk/efpi/ejendomshandel/Ejendomshandel.eFPI";
                    break;
                default:
                    break;
            }
            return (s);
        }
    }
}
