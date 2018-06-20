using System;
using System.Windows.Forms;
using EHP_Client;

namespace EHP_Client
{
    public enum Funktion { ejendomshandler, aktoerer, garantier, haendelser}
    public enum Miljoe { Test, Staging, Produktion }

    public partial class FormHent : Form
    {

        private ServiceReferenceTBIS.ValidatePartyResponse validatePartyResponse;
        private Funktion funktion;
            
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
                case 0: // Login side
                    {
                        if (!backgroundWorkerRKNET.IsBusy) { backgroundWorkerRKNET.RunWorkerAsync(); }
                        break;
                    }
                case 1:
                    {
                        funktion = userControlVaelg1.Function;
                        switch (funktion)
                        {
                            case Funktion.aktoerer:
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
                                    SetStatus("Not implemented yet..");
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


        private void BackgroundWorkerRKNET_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            RKNet_Utils rknet_Utils = new RKNet_Utils(userControlLogon1.Miljoe, userControlLogon1.PartID, "", userControlLogon1.Password);
            validatePartyResponse = rknet_Utils.GetValidatePartyResponse();
            backgroundWorkerRKNET.ReportProgress(0, validatePartyResponse.backEndStatusCode.ToString() + " " + validatePartyResponse.backEndStatusText);
        }

        private void backgroundWorkerRKNET_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            SetStatus(e.UserState.ToString());
        }

        private void backgroundWorkerRKNET_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (validatePartyResponse.backEndStatusCode == 0) { wizardTabcontrol1.SelectedIndex += 1; }
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
                resetDataGridwied();
                for (int i = 0; i < response.HaendelseHistorikSamling.Length; i++)
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["Tidsstempel"].Value = response.HaendelseHistorikSamling[i].Haendelse.Tidsstempel;
                    dataGridView1.Rows[rowIndex].Cells["Hændelsekode"].Value = response.HaendelseHistorikSamling[i].Haendelse.HaendelseKode;
                    dataGridView1.Rows[rowIndex].Cells["Hændelsebeskrivelse"].Value = response.HaendelseHistorikSamling[i].Haendelse.HaendelseBeskrivelse;
                    dataGridView1.Rows[rowIndex].Cells["ProcesID"].Value = ((ServiceReferenceEjendomshandel.HaendelseKontekstType)response.HaendelseHistorikSamling[i].Item).ProcesID;
                }
            }
            catch (Exception f) { backgroundWorkerHaendelser.ReportProgress(0, f.Message); }
        }

        private void resetDataGridwied()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            switch (funktion)
            {
                case Funktion.aktoerer:
                    {
                        dataGridView1.Columns.Add("Aktør", "Aktør");
                        dataGridView1.Columns.Add("Navn", "Navn");
                        dataGridView1.Columns.Add("Roller", "Roller");
                        break;
                    }
                case Funktion.ejendomshandler:
                    {
                        dataGridView1.Columns.Add("ProcesID", "ProcesID");
                        dataGridView1.Columns.Add("Adresse", "Adresse");
                        dataGridView1.Columns.Add("Rolle", "Rolle");
                        dataGridView1.Columns.Add("Tilstand", "Tilstand");
                        dataGridView1.Columns.Add("Rolle", "Rolle");
                        break;
                    }
                case Funktion.haendelser:
                    {
                        dataGridView1.Columns.Add("Tidsstempel", "Tidsstempel");
                        dataGridView1.Columns.Add("Hændelsekode", "Hændelsekode");
                        dataGridView1.Columns.Add("Hændelsebeskrivelse", "Hændelsebeskrivelse");
                        break;
                    }
                default: break;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            SetStatus(e.UserState.ToString());
        }

        private void backgroundWorkerEjendomshandler_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
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
                resetDataGridwied();
                for (int i = 0; i < response.AktoerDataSamling.Length; i++)
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["Aktør"].Value = response.AktoerDataSamling[i].AktoerInformation.AktoerID;
                    dataGridView1.Rows[rowIndex].Cells["Navn"].Value = response.AktoerDataSamling[i].AktoerInformation.AktoerNavn;
                    dataGridView1.Rows[rowIndex].Cells["Roller"].Value = aktoerregisterUtils.GetPrettyRoller(response.AktoerDataSamling[i].ProcesDeltagelseSamling);
                }
            }
            catch (Exception f) { backgroundWorkerAktoerer.ReportProgress(0, f.Message); }
        }


        private void backgroundWorkerAktoerer_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            wizardTabcontrol1.SelectedIndex += 1;
            EnableButtons(true);
        }

        private void backgroundWorkerEjendomshandler_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            SagshaandteringUtils sagshaandteringUtils = new SagshaandteringUtils(userControlLogon1.Miljoe, userControlLogon1.PartID, userControlLogon1.ActAs, userControlLogon1.Password);
            EjendomshandelUtils ejendomshandelUtils = new EjendomshandelUtils(userControlLogon1.Miljoe, userControlLogon1.PartID, userControlLogon1.ActAs, userControlLogon1.Password);
            backgroundWorkerEjendomshandler.ReportProgress(0, "Henter aktive processer");
            try
            {
                ServiceReferenceEjendomshandel.SagsAktivitetListeHentResponseType response = ejendomshandelUtils.GetSagsAktivitetListeHentResponseType();
                backgroundWorkerEjendomshandler.ReportProgress(0, response.ProcesSamling.Length.ToString() + "aktive processer hentet");
                resetDataGridwied();
                for (int i = 0; i < response.ProcesSamling.Length; i++)
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["ProcesID"].Value = response.ProcesSamling[i].eFPIprocesID;
                    try { dataGridView1.Rows[rowIndex].Cells["Adresse"].Value = ejendomshandelUtils.GetPrettyAddress(response.ProcesSamling[i].Adresse); }
                    catch(Exception g) { dataGridView1.Rows[rowIndex].Cells["Adresse"].Value = "Ingen adresse"; }
                    dataGridView1.Rows[rowIndex].Cells["Rolle"].Value = response.ProcesSamling[i].Rolle;
                    dataGridView1.Rows[rowIndex].Cells["Tilstand"].Value = response.ProcesSamling[i].Tilstand;

                }
            }
            catch (Exception f) { backgroundWorkerEjendomshandler.ReportProgress(0, f.Message); }

        }

        private void backgroundWorkerHaendelser_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            wizardTabcontrol1.SelectedIndex += 1;
            EnableButtons(true);
        }
    }
}
