using System;
using System.Windows.Forms;
using EHP_Client;

namespace EHP_Client
{
    public partial class Form1 : Form
    {
        enum Testomraade { aktoerregister, ejendomshandel, sagshaandtering}

        private Testomraade testomraade;

        public Form1()
        {
            InitializeComponent();
            comboBoxTestomraade.DataSource = Enum.GetValues(typeof(Testomraade));
        }

        private void SetStatus(string status)
        {
            toolStripStatusLabel1.Text = status;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            testomraade= (Testomraade)comboBoxTestomraade.SelectedIndex;
            int i;
            switch (testomraade)
            {
                case Testomraade.aktoerregister:
                    {
                        try {
                            i = TestAktoerregister();
                            SetStatus("Succes ved test af aktørregister med " + i.ToString() + " fundne aktører.");
                        }
                        catch (Exception f) { SetStatus("Fejl ved test af aktørregister: " + f.Message); }
                        break;
                    }
                case Testomraade.ejendomshandel:
                    {
                        try
                        {
                            i = TestEjendomshandel();
                            SetStatus("Succes ved test af ejendomshandler med " + i.ToString() + " fundne ejendomshandler.");
                        }
                        catch (Exception f) { SetStatus("Fejl ved test af ejendomshandel: " + f.Message); }
                        break;
                    }
                case Testomraade.sagshaandtering:
                    {
                        try
                        {
                            i = TestSagshaandtering();
                            SetStatus("Succes ved hent af aktive processer med " + i.ToString() + " fundne processer.");
                        }
                        catch (Exception f) { SetStatus("Fejl ved test af sagshaandtering: " + f.Message); }
                        break;
                    }

                default: break;
            }
        }

        private int TestAktoerregister()
        {
            ServiceReferenceAktoerregister.AktoerregistereFPIClient client = ClientFactory.GetAktoerregistereFPIClient(textBoxPartyid.Text, textBoxPassword.Text);
            ServiceReferenceAktoerregister.HentAlleAktoerInformationerType request = new ServiceReferenceAktoerregister.HentAlleAktoerInformationerType();
            ServiceReferenceAktoerregister.HentAlleAktoerInformationerResponseType response =  client.HentAlleAktoerInformationer(request);
            int i = response.AktoerDataSamling.Length;
            return i;
        }

        private int TestEjendomshandel()
        {

            ServiceReferenceEjendomshandel.EjendomshandeleFPIClient client = ClientFactory.GetEjendomshandeleFPIClient(textBoxPartyid.Text, textBoxPassword.Text);
            ServiceReferenceEjendomshandel.ProcesSoegRequestHeaderType header = new ServiceReferenceEjendomshandel.ProcesSoegRequestHeaderType();
            header.DeltagerID = ClientFactory.ToActoerID(textBoxPartyid.Text);
            ServiceReferenceEjendomshandel.ProcesSoegRequestType request = new ServiceReferenceEjendomshandel.ProcesSoegRequestType();
            request.Soegestreng = "En eller anden søgestreng";
            ServiceReferenceEjendomshandel.ProcesSoegResponseType response = client.ProcesSoeg(header, request);
            int i = response.ProcesSoegFundSamling.Length;
            return (i);
        }

        private int TestSagshaandtering()
        {
            ServiceReferenceSagshaandtering.SagshaandteringeFPIClient client = ClientFactory.GetSagshaandteringeFPIClient(textBoxPartyid.Text, textBoxPassword.Text);
            ServiceReferenceSagshaandtering.ListAktiveProcesserType request = new ServiceReferenceSagshaandtering.ListAktiveProcesserType();
            request.AktoerID = ClientFactory.ToActoerID(textBoxPartyid.Text);
            ServiceReferenceSagshaandtering.ListAktiveProcesserResponseType response = client.ListAktiveProcesser(request);
            int i = response.eFPIprocesIDSamling.Length;
            return (i);
        }
    }
}
