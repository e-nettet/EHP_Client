using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHP_Client.ServiceReferenceGaranti;

namespace EHP_Client
{
    public class GarantiUtils
    {
        private Miljoe miljoe;
        private string partyid;
        private string actas; // Agere på vegne af
        private string password;
        private GarantieFPIClient client;

        public GarantiUtils(Miljoe miljoe, string partyid, string actas, string password)
        {
            this.miljoe = miljoe;
            this.partyid = partyid;
            this.actas = actas;
            this.password = password;
            client = ClientFactory.GetGarantieFPIClient(partyid, password, GetEndpointAddress());
        }

        public string GetEndpointAddress()
        {
            string s;
            switch (miljoe)
            {
                case Miljoe.Test:
                    s = "https://test-bolighandel.e-nettet.dk/efpi/garanti/Garanti.eFPI";

                    break;
                case Miljoe.Produktion:
                    s = "https://e-bolighandel.e-nettet.dk/efpi/garanti/Garanti.eFPI";
                    break;
                default:
                    s = "";
                    break;
            }
            return (s);
        }

        public HaendelsesHistorikHentResponseType GetHentAlleAktoerInformationer()
        {
            HaendelsesHistorikHentRequestHeaderType header = new HaendelsesHistorikHentRequestHeaderType()
            {
                DeltagerID = ClientFactory.ToActoerID(actas)
            };

            HaendelsesHistorikHentRequestType request = new HaendelsesHistorikHentRequestType()
            {
                DatoFra = DateTime.Now.AddDays(-14),
                DatoFraSpecified = true,
                DatoTil = DateTime.Now,
                DatoTilSpecified = true
            };
            HaendelsesHistorikHentResponseType response = client.HaendelsesHistorikHent(header, request);
            return (response);
        }

        public GarantierListeHentResponseType GetGarantierListeHentResponseType()
        {
            GarantierListeHentRequestHeaderType header = new GarantierListeHentRequestHeaderType()
            {
                AntalRaekker = ((int)100).ToString(),
                DeltagerID = ClientFactory.ToActoerID(actas),
                GarantiSelectionStatus = GarantiSelectionStatusType.All,
                Kolonne = GarantierListeHentKolonneType.Status,
                KolonneSorteringsretning = KolonneSorteringsretningType.Faldende,
                Offset = "100"
            };
            GarantierListeHentResponseType response = client.GarantierListeHent(header, new TomtElementType());
            return (response);
        }
    }
}
