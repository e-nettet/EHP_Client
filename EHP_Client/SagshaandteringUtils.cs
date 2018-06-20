using System;
using EHP_Client.ServiceReferenceSagshaandtering;

namespace EHP_Client
{
    public class SagshaandteringUtils
    {
        private Miljoe miljoe;
        private string partyid;
        private string actas; // Agere på vegne af
        private string password;
        private SagshaandteringeFPIClient client;

        public SagshaandteringUtils(Miljoe miljoe, string partyid, string actas, string password)
        {
            this.miljoe = miljoe;
            this.partyid = partyid;
            this.actas = actas;
            this.password = password;
            client = ClientFactory.GetSagshaandteringeFPIClient(partyid, password, GetEndpointAddress());
        }

        private string GetEndpointAddress()
        {
            string s = "";
            switch (miljoe)
            {
                case Miljoe.Test:
                    s = "https://test-bolighandel.e-nettet.dk/efpi/sagshaandtering/Sagshaandtering.eFPI";

                    break;
                case Miljoe.Produktion:
                    s = "https://e-bolighandel.e-nettet.dk/efpi/sagshaandtering/Sagshaandtering.eFPI";
                    break;
                default:
                    break;
            }
            return (s);
        }

        public ListAktiveProcesserResponseType GetListAktiveProcesser()
        {
            ListAktiveProcesserType l = new ListAktiveProcesserType();
            l.AktoerID = ClientFactory.ToActoerID(actas);
            ListAktiveProcesserResponseType response = client.ListAktiveProcesser(l);
            return (response);
        }


    }



}
