using System;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using EHP_Client.ServiceReferenceEjendomshandel;
using EHP_Client.ServiceReferenceGaranti;
using EHP_Client.ServiceReferenceSagshaandtering;
using EHP_Client.ServiceReferenceAktoerregister;


namespace EHP_Client
{
    public static class ClientFactory
    {
        
        private static string Endpoint_Ejendomshandel = "Ejendomshandel";
        private static string Endpoint_Garanti = "Garanti";
        private static string Endpoint_Sagshaandtering = "Sagshaandtering";
        private static string Endpoint_Aktoerregister = "Aktoerregister";

        public static string ToActoerID(string s)
        {
            if (s.Length == 16)
            {
                if (s.Substring(13, 3).Equals(":14"))
                    s = "ean:" + s;
            }
            return (s);
        }

        public static EjendomshandeleFPIClient GetEjendomshandeleFPIClient(string partyid, string password, string endpointAddress) 
        {
            EjendomshandeleFPIClient client = new EjendomshandeleFPIClient(Endpoint_Ejendomshandel);
            client.ClientCredentials.UserName.UserName = partyid;
            client.ClientCredentials.UserName.Password = password;
            SetEndpoint(client.Endpoint, ToActoerID(partyid), endpointAddress);
            return (client);
        }

        public static GarantieFPIClient GetGarantieFPIClient(string partyid, string password, string endpointAddress)
        {
            GarantieFPIClient client = new GarantieFPIClient(Endpoint_Garanti);
            client.ClientCredentials.UserName.UserName = partyid;
            client.ClientCredentials.UserName.Password = password;
            SetEndpoint(client.Endpoint, ToActoerID(partyid), endpointAddress);
            return (client);
        }

        public static SagshaandteringeFPIClient GetSagshaandteringeFPIClient(string partyid, string password, string endpointAddress)
        {
            SagshaandteringeFPIClient client = new SagshaandteringeFPIClient(Endpoint_Sagshaandtering);
            client.ClientCredentials.UserName.UserName = partyid;
            client.ClientCredentials.UserName.Password = password;
            SetEndpoint(client.Endpoint, ToActoerID(partyid), endpointAddress);
            return (client);
        }

        public static AktoerregistereFPIClient GetAktoerregistereFPIClient(string partyid, string password, string endpointaddress)
        {
            AktoerregistereFPIClient client = new AktoerregistereFPIClient(Endpoint_Aktoerregister);
            client.ClientCredentials.UserName.UserName = partyid;
            client.ClientCredentials.UserName.Password = password;
            SetEndpoint(client.Endpoint, ToActoerID(partyid), endpointaddress);
            return (client);
        }

        public static void SetEndpoint(ServiceEndpoint serviceEndpoint, string aktoerID, string endpointAddress)
        {
            serviceEndpoint.Behaviors.Add(new eFPIBehavior(aktoerID));
        }
    }

    class eFPIBehavior : IEndpointBehavior
    {
        private string actorId;

        public eFPIBehavior(String actorId) { this.actorId = actorId; }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new eFPIInspector(actorId));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class eFPIInspector : IClientMessageInspector
    {
        private string actorId;

        public eFPIInspector(String actorId) { this.actorId = actorId; }
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {

            if (actorId.Substring(0, 4).Equals("ean:")) {  request.Headers.From = new EndpointAddress(new Uri(actorId)); }
            else  {  request.Headers.From = new EndpointAddress(new Uri("http://efpi.dk/aktoer/" + actorId)); }
            request.Headers.To =new Uri("http://efpi.dk/aktoer/21270776");
            return null;
        }
    }
}

