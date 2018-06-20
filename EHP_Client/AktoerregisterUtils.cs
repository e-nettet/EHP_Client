using System;
using EHP_Client.ServiceReferenceAktoerregister;

namespace EHP_Client

{
    public class AktoerregisterUtils
    {
        private Miljoe miljoe;
        private string partyid;
        private string actas; // Agere på vegne af
        private string password;

        public AktoerregisterUtils(Miljoe miljoe, string partyid, string actas, string password)
        {
            this.miljoe = miljoe;
            this.partyid = partyid;
            this.actas = actas;
            this.password = password;
        }


        public string GetEndpointAddress()
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
        public HentAlleAktoerInformationerResponseType GetHentAlleAktoerInformationer()
        {
            AktoerregistereFPIClient client = ClientFactory.GetAktoerregistereFPIClient(partyid, password, GetEndpointAddress());
            HentAlleAktoerInformationerResponseType response = client.HentAlleAktoerInformationer(new HentAlleAktoerInformationerType());
            return (response);
        }

        public string GetPrettyRoller(ProcesDeltagelseType[] p)
        {
            string s = "";
            for (int i = 0; i < p.Length; i++)
            {
                for (int j = 0; j < p[i].Rolle.Length; j++)
                {
                    if (s.Length > 0) { s += ", "; }
                    s += p[i].Rolle[j] + " (" + p[i].Procesversion.ProcesNavn + ")";
                }
            }
            if (s.Length == 0) { s = "Ingen roller"; }
            return (s);
        }
    }
}
