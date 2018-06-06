using System;
using EHP_Client.ServiceReferenceEjendomshandel;


namespace EHP_Client
{
    public class EjendomshandelUtils
    {
        private Miljoe miljoe;
        private string partyid;
        private string actas; // Agere på vegne af
        private string password;

        public EjendomshandelUtils(Miljoe miljoe, string partyid, string actas, string password)
        {
            this.miljoe = miljoe;
            this.partyid = partyid;
            this.actas = actas;
            this.password = password;
        }

        public string GetEndpointAddress()
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

        public HaendelsesHistorikHentResponseType GetHaendelsesHistorikHentResponseType()
        {
            EjendomshandeleFPIClient client = ClientFactory.GetEjendomshandeleFPIClient(partyid, password, GetEndpointAddress());
            HaendelsesHistorikHentRequestHeaderType header = new HaendelsesHistorikHentRequestHeaderType();
            header.DeltagerID = ClientFactory.ToActoerID(actas);
            HaendelsesHistorikHentRequestType request = new HaendelsesHistorikHentRequestType();
            request.DatoFra = DateTime.Now.AddDays(-14);
            request.DatoFraSpecified = true;
            HaendelsesHistorikHentResponseType response = client.HaendelsesHistorikHentV2(header, request);
            return (response);
        }
    }
}
