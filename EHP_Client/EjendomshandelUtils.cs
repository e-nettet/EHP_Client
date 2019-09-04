using System;
using EHP_Client.ServiceReferenceEjendomshandel;


namespace EHP_Client
{
    public class EjendomshandelUtils
    {
        private readonly Miljoe miljoe;
        private readonly string partyid;
        private string actas; // Agere på vegne af
        private readonly string password;
        private EjendomshandeleFPIClient client;

        public EjendomshandelUtils(Miljoe miljoe, string partyid, string actas, string password)
        {
            this.miljoe = miljoe;
            this.partyid = partyid;
            this.actas = actas;
            this.password = password;
            client = ClientFactory.GetEjendomshandeleFPIClient(partyid, password, GetEndpointAddress());
        }

        private string GetEndpointAddress()
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
            HaendelsesHistorikHentRequestHeaderType header = new HaendelsesHistorikHentRequestHeaderType()
            {
                DeltagerID = ClientFactory.ToActoerID(actas)
            };
            HaendelsesHistorikHentRequestType request = new HaendelsesHistorikHentRequestType()
            {
                DatoFra = DateTime.Now.AddDays(-14),
                DatoFraSpecified = true
            };
            HaendelsesHistorikHentResponseType response = client.HaendelsesHistorikHentV2(header, request);
            return (response);
        }
        public EjendomshandlensDataHentResponseType GetEjendomshandlensData(string procesID, RolleIDType rolleIDType)
        {
            EHPStandardRequestHeaderType header = new EHPStandardRequestHeaderType()
            {
                DeltagerID = ClientFactory.ToActoerID( actas),
                ProcesID = procesID,
                RolleID = rolleIDType
            };
            EjendomshandlensDataHentResponseType response = client.EjendomshandlensDataHent(header, new TomtElementType());
            return (response);
        }

        public SagsAktivitetListeHentResponseType GetSagsAktivitetListeHentResponseType()
        {
            SagsAktivitetListeHentRequestHeaderType header = new SagsAktivitetListeHentRequestHeaderType()
            {
                Offset = "10",
                AntalRaekker = "9999",
                Kolonne = SagsAktivitetListeHentKolonneType.Opdateret,
                KolonneSorteringsretning = KolonneSorteringsretningType.Faldende,
                DeltagerID = ClientFactory.ToActoerID( actas),
                InkluderLukket = false,
            };
            SagsAktivitetListeHentResponseType response = client.SagsAktivitetListeAktivRolleHent(header, new TomtElementType());
            return (response);
        }

        public string GetPrettyAddress(SalgsopstillingSoegAddresseType a)
        {
            string s = "";
            s += a.Vejnavn + " " + a.Vej_nr;
            if (a.Etage.Length > 0) { s += ", " + a.Etage; };
            if (a.Lejlighedsnummer.Length > 0) { s += ", lejlighedsnummer " + a.Lejlighedsnummer; };
            s += ", " + a.Postnummer + " " + a.Bynavn;
            return (s);
        }

        public ProcesSoegResponseType ProcesSoeg(string soegestreng) // This is an internal e-nettet operation and can be changed without warning
        {
            ProcesSoegRequestHeaderType header = new ProcesSoegRequestHeaderType()
            {
                DeltagerID = ClientFactory.ToActoerID( actas)
            };
            ProcesSoegRequestType request = new ProcesSoegRequestType()
            {
                CvrCprSearch = true,
                Soegestreng = soegestreng
            };
            ProcesSoegResponseType response = client.ProcesSoeg(header, request);
            return (response);
        }

        public DokumentHentResponseType DokumentHent(string procesID, RolleIDType rolle, DokumentKategoriType dokumentKategoriType)
        {
            EHPStandardRequestHeaderType header = new EHPStandardRequestHeaderType()
            {
                DeltagerID = ClientFactory.ToActoerID( actas),
                ProcesID = procesID,
                RolleID = rolle
            };
            DokumentHentRequestType request = new DokumentHentRequestType()
            {
                DokumentKategori = dokumentKategoriType,
            };
            DokumentHentResponseType response = client.DokumentHent(header, request);
            return (response);
        }
    }
}
