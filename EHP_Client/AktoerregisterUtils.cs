using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHP_Client.ServiceReferenceAktoerregister;

namespace EHP_Client

{
    public static class AktoerregisterUtils
    {
        public static string GetEndpointAddress(Miljoe miljoe)
        {
            string s;
            switch (miljoe)
            {
                case Miljoe.Test:
                    s = "https://test-bolighandel.e-nettet.dk/efpi/aktoerregister/Aktoerregister.eFPI";

                    break;
                case Miljoe.Produktion:
                    s = "https://e-bolighandel.e-nettet.dk/efpi/aktoerregister/Aktoerregister.eFPI";
                    break;
                default:
                    s = "";
                    break;
            }
            return (s);
        }
    }
}
