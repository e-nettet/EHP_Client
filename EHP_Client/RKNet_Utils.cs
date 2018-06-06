using System;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using EHP_Client.ServiceReferenceTBIS;


namespace EHP_Client
{
    public enum DataType { Restgaeld, Omregningskurstabller, EgnePapirer, AllePapirer, Satser, Priser }

    public class RKNet_Utils
    {
        private Miljoe miljoe;
        private string partyid;
        private string jnummer;
        private string password;

        public Miljoe Miljoe { get => miljoe; set => miljoe = value; }
        public string PartyID { get => partyid; set => partyid = value; }
        public string Jnummer { get => jnummer; set => jnummer = value; }
        public string Password { get => password; set => password= value; }

        public RKNet_Utils(Miljoe miljoe, string partyid, string jnummer, string password)
        {
            this.miljoe = miljoe;
            this.partyid = partyid;
            this.jnummer = jnummer;
            this.password = password;


        }

        private string GetEndpointAddress()
        {
            string s;
            switch (miljoe)
            {
                case Miljoe.Produktion:
                    s = "https://tbisws.e-nettet.dk/TBISWS/Main";
                    break;
                case Miljoe.Test:
                    s = "https://test-tbisws.e-nettet.dk/TBISWS/Main";
                    break;
                default:
                    s = "";
                    break;
            }
            return (s);
        }



        private MainClient GetMainClient(Miljoe miljoe)
        {
            MainClient client = new MainClient();
            System.ServiceModel.EndpointAddress addr = new System.ServiceModel.EndpointAddress(GetEndpointAddress());
            client.Endpoint.Address = addr;
            return (client);
        }

        public ValidatePartyResponse GetValidatePartyResponse()
        {
            MainClient client = GetMainClient(miljoe);
            ValidatePartyResponse response = client.validateParty(partyid, password);
            return (response);
        }
    }

}
