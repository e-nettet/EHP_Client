using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHP_Client.ServiceReferenceSagshaandtering;

namespace EHP_Client
{
    public class SagshaandteringUtils
    {
        private Miljoe miljoe;
        private string partyid;
        private string actas; // Agere på vegne af
        private string password;

        public SagshaandteringUtils(Miljoe miljoe, string partyid, string actas, string password)
        {
            this.miljoe = miljoe;
            this.partyid = partyid;
            this.actas = actas;
            this.password = password;
        }

        public string GetEndpointAddress(Miljoe miljoe)
        {
            string s;
            switch (miljoe)
            {
                case Miljoe.Test:
                    s = "https://test-bolighandel.e-nettet.dk/efpi/sagshaandtering/Sagshaandtering.eFPI";

                    break;
                case Miljoe.Produktion:
                    s = "https://e-bolighandel.e-nettet.dk/efpi/sagshaandtering/Sagshaandtering.eFPI";
                    break;
                default:
                    s = "";
                    break;
            }
            return (s);
        }

    }



}
