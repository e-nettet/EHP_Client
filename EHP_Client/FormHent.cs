using System;
using System.Data;
using System.Windows.Forms;

namespace EHP_Client
{
    public enum Funktion { ejendomshandler, aktoerer, garantier, haendelser}
    public enum Miljoe { Test, Staging, Produktion }

    public partial class FormHent : Form
    {
        private ServiceReferenceTBIS.ValidatePartyResponse validatePartyResponse;
        private Funktion funktion;
        private DataTable dataTable;

        public FormHent()
        {
            InitializeComponent();
            EnableButtons(true);
        }

        private void SetStatus(string status)
        {
            toolStripStatusLabel1.Text = status;
        }

        private void ButtonNext_Click(object sender, EventArgs e)
        {
            EnableButtons(false);
            switch (wizardTabcontrol1.SelectedIndex)
            {
                case 0:// Login side
                    {
                        if (!backgroundWorkerRKNET.IsBusy) { backgroundWorkerRKNET.RunWorkerAsync(); }
                        break;
                    }
                case 1:
                    {
                        funktion = userControlVaelg1.Function;
                        switch (funktion)
                        {
                            case Funktion.aktoerer: // Aktører
                                {
                                    if (!backgroundWorkerAktoerer.IsBusy) { backgroundWorkerAktoerer.RunWorkerAsync(); }
                                    break;
                                }
                            case Funktion.haendelser: // Hændelser
                                {
                                    if (!backgroundWorkerHaendelser.IsBusy) { backgroundWorkerHaendelser.RunWorkerAsync(); }
                                    break;
                                }
                            case Funktion.garantier:
                                {
                                    if (!backgroundWorkerGaranti.IsBusy) { backgroundWorkerGaranti.RunWorkerAsync(); }
                                    //SetStatus("Not implemented yet..");
                                    break;
                                }
                            case Funktion.ejendomshandler: // Ejendomshandelsprocesser
                                {
                                    if (!backgroundWorkerEjendomshandler.IsBusy) { backgroundWorkerEjendomshandler.RunWorkerAsync(); }
                                    break;
                                }
                        }

                        break;
                    }
                default:break;
            }
        }

        private void EnableButtons(bool enable)
        {
            buttonNext.Enabled = (enable && wizardTabcontrol1.SelectedIndex < wizardTabcontrol1.TabCount-1);
            buttonBack.Enabled = (enable && wizardTabcontrol1.SelectedIndex >0);
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            if (wizardTabcontrol1.SelectedIndex > 0)
            {
                wizardTabcontrol1.SelectedIndex -= 1;
            }
            EnableButtons(true);

        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            SetStatus(e.UserState.ToString());
        }

        private void BackgroundWorkerRKNET_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            RKNet_Utils rknet_Utils = new RKNet_Utils(userControlLogon1.Miljoe, userControlLogon1.PartID, "", userControlLogon1.Password);
            validatePartyResponse = rknet_Utils.GetValidatePartyResponse();
            backgroundWorkerRKNET.ReportProgress(0, validatePartyResponse.backEndStatusCode.ToString() + " " + validatePartyResponse.backEndStatusText);
        }

        private void backgroundWorkerRKNET_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (validatePartyResponse.backEndStatusCode == 0) { wizardTabcontrol1.SelectedIndex += 1; }
            EnableButtons(true);
        }

        private void backgroundWorkerEjendomshandler_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EjendomshandelUtils ejendomshandelUtils = new EjendomshandelUtils(userControlLogon1.Miljoe, userControlLogon1.PartID, userControlLogon1.ActAs, userControlLogon1.Password);
            backgroundWorkerEjendomshandler.ReportProgress(0, "Henter aktive processer");
            try
            {
                ServiceReferenceEjendomshandel.SagsAktivitetListeHentResponseType response = ejendomshandelUtils.GetSagsAktivitetListeHentResponseType();
                backgroundWorkerEjendomshandler.ReportProgress(0, response.ProcesSamling.Length.ToString() + "aktive processer hentet");

                dataTable = new DataTable();
                dataTable.Columns.Add("ProcesID", typeof(string));
                dataTable.Columns.Add("Adresse", typeof(string));
                dataTable.Columns.Add("Rolle", typeof(string));
                dataTable.Columns.Add("Tilstand", typeof(string));

                for (int i = 0; i < response.ProcesSamling.Length; i++)
                {
                    string adresse = "";
                    try { adresse = ejendomshandelUtils.GetPrettyAddress(response.ProcesSamling[i].Adresse); }
                    catch (Exception g) { adresse = "Ingen adresse"; }
                    dataTable.Rows.Add(response.ProcesSamling[i].eFPIprocesID, adresse, response.ProcesSamling[i].Rolle, response.ProcesSamling[i].Tilstand);
                }
            }
            catch (Exception f) { backgroundWorkerEjendomshandler.ReportProgress(0, f.Message); }
        }

        private void backgroundWorkerEjendomshandler_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataTable;
            wizardTabcontrol1.SelectedIndex += 1;
            EnableButtons(true);
        }

        private void backgroundWorkerAktoerer_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            AktoerregisterUtils aktoerregisterUtils = new AktoerregisterUtils(userControlLogon1.Miljoe, userControlLogon1.PartID, userControlLogon1.ActAs, userControlLogon1.Password);
            backgroundWorkerAktoerer.ReportProgress(0, "Henter aktørinformationer");
            try {
                ServiceReferenceAktoerregister.HentAlleAktoerInformationerResponseType response = aktoerregisterUtils.GetHentAlleAktoerInformationer();
                backgroundWorkerAktoerer.ReportProgress(0, response.AktoerDataSamling.Length.ToString() + " aktørinformationer hentet");

                dataTable = new DataTable();
                dataTable.Columns.Add("Aktør", typeof(string));
                dataTable.Columns.Add("Navn", typeof(string));
                dataTable.Columns.Add("Roller", typeof(string));

                for (int i = 0; i < response.AktoerDataSamling.Length; i++)
                {
                    dataTable.Rows.Add(
                        response.AktoerDataSamling[i].AktoerInformation.AktoerID, 
                        response.AktoerDataSamling[i].AktoerInformation.AktoerNavn, 
                        aktoerregisterUtils.GetPrettyRoller(response.AktoerDataSamling[i].ProcesDeltagelseSamling));
                }
            }
            catch (Exception f) { backgroundWorkerAktoerer.ReportProgress(0, f.Message); }
        }
        
        private void backgroundWorkerAktoerer_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataTable;
            wizardTabcontrol1.SelectedIndex += 1;
            EnableButtons(true);
        }

        private void backgroundWorkerHaendelser_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EjendomshandelUtils ejendomshandelUtils = new EjendomshandelUtils(userControlLogon1.Miljoe, userControlLogon1.PartID, userControlLogon1.ActAs, userControlLogon1.Password);
            backgroundWorkerHaendelser.ReportProgress(0, "Henter hændelseshistorik");
            try
            {
                ServiceReferenceEjendomshandel.HaendelsesHistorikHentResponseType response = ejendomshandelUtils.GetHaendelsesHistorikHentResponseType();
                backgroundWorkerHaendelser.ReportProgress(0, response.HaendelseHistorikSamling.Length.ToString() + " hændelser hentet");

                dataTable = new DataTable();
                dataTable.Columns.Add("Tidsstempel", typeof(DateTime));
                dataTable.Columns.Add("Hændelsekode", typeof(string));
                dataTable.Columns.Add("Hændelsebeskrivelse", typeof(string));
                dataTable.Columns.Add("ProcesID", typeof(string));

                for (int i = 0; i < response.HaendelseHistorikSamling.Length; i++)
                {
                    dataTable.Rows.Add(
                        response.HaendelseHistorikSamling[i].Haendelse.Tidsstempel,
                        response.HaendelseHistorikSamling[i].Haendelse.HaendelseKode,
                        response.HaendelseHistorikSamling[i].Haendelse.HaendelseBeskrivelse,
                        ((ServiceReferenceEjendomshandel.HaendelseKontekstType)response.HaendelseHistorikSamling[i].Item).ProcesID);
                }
            }
            catch (Exception f) { backgroundWorkerHaendelser.ReportProgress(0, f.Message); }
        }

        private void backgroundWorkerHaendelser_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataTable;
            wizardTabcontrol1.SelectedIndex += 1;
            EnableButtons(true);
        }

        private void backgroundWorkerGaranti_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            SagshaandteringUtils sagshaandteringUtils = new SagshaandteringUtils(userControlLogon1.Miljoe, userControlLogon1.PartID, userControlLogon1.ActAs, userControlLogon1.Password);
            GarantiUtils garantiUtils = new GarantiUtils(userControlLogon1.Miljoe, userControlLogon1.PartID, userControlLogon1.ActAs, userControlLogon1.Password);
            backgroundWorkerGaranti.ReportProgress(0, "Henter aktive processer");
            try
            {
                ServiceReferenceGaranti.GarantierListeHentResponseType response = garantiUtils.GetGarantierListeHentResponseType();
                backgroundWorkerGaranti.ReportProgress(0, response.GarantiSamling.Length.ToString() + "aktive processer hentet");

                dataTable = new DataTable();
                dataTable.Columns.Add("ProcesID", typeof(string));
                dataTable.Columns.Add("Rolle", typeof(string));
                dataTable.Columns.Add("Køber", typeof(string));
                dataTable.Columns.Add("Sælger", typeof(string));

                for (int i = 0; i < response.GarantiSamling.Length; i++)
                {
                    dataTable.Rows.Add(
                        response.GarantiSamling[i].ProcesID, 
                        response.GarantiSamling[i].RolleID, 
                        response.GarantiSamling[i].Koebere, 
                        response.GarantiSamling[i].Saelgere
                        );
                }
            }
            catch (Exception f) { backgroundWorkerGaranti.ReportProgress(0, f.Message); }
        }

        private void backgroundWorkerGaranti_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataTable;
            wizardTabcontrol1.SelectedIndex += 1;
            EnableButtons(true);
        }

    }
}
