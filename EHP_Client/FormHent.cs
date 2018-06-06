using System;
using System.Windows.Forms;
using EHP_Client;

namespace EHP_Client
{
    public enum Funktion { ejendomshandel, aktoerregister, sagshaandtering }
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
                case 0: // Login sige
                    {
                        if (!backgroundWorkerRKNET.IsBusy) { backgroundWorkerRKNET.RunWorkerAsync(); }
                        break;
                    }
                case 1:
                    {
                        funktion = userControlVaelg1.Function;
                        if (!backgroundWorkerEjendomshandel.IsBusy) { backgroundWorkerEjendomshandel.RunWorkerAsync(); }
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

        private void backgroundWorkerEjendomshandel_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EjendomshandelUtils ejendomshandelUtils = new EjendomshandelUtils(userControlLogon1.Miljoe, userControlLogon1.PartID, userControlLogon1.ActAs, userControlLogon1.Password);
            ServiceReferenceEjendomshandel.HaendelsesHistorikHentResponseType response = ejendomshandelUtils.GetHaendelsesHistorikHentResponseType();
            resetDataGridwied();
            for (int i = 0; i < response.HaendelseHistorikSamling.Length; i++)
            {
                int rowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowIndex].Cells["Tidsstempel"].Value= response.HaendelseHistorikSamling[i].Haendelse.Tidsstempel;
                dataGridView1.Rows[rowIndex].Cells["Hændelsekode"].Value = response.HaendelseHistorikSamling[i].Haendelse.HaendelseKode;
                dataGridView1.Rows[rowIndex].Cells["Hændelsebeskrivelse"].Value = response.HaendelseHistorikSamling[i].Haendelse.HaendelseBeskrivelse;
                dataGridView1.Rows[rowIndex].Cells["ProcesID"].Value = ((ServiceReferenceEjendomshandel.HaendelseKontekstType) response.HaendelseHistorikSamling[i].Item).ProcesID;

            }
        }

        private void resetDataGridwied()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            switch (funktion)
            {
                case Funktion.aktoerregister:break; // TBD
                case Funktion.ejendomshandel:
                    {
                        dataGridView1.Columns.Add("Tidsstempel", "Tidsstempel");
                        dataGridView1.Columns.Add("Hændelsekode", "Hændelsekode");
                        dataGridView1.Columns.Add("Hændelsebeskrivelse", "Hændelsebeskrivelse");
                        dataGridView1.Columns.Add("ProcesID", "ProcesID");
                        break;
                    }
                default:break;
            }
        }

        private void backgroundWorkerEjendomshandel_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            SetStatus(e.UserState.ToString());
        }

        private void backgroundWorkerEjendomshandel_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            wizardTabcontrol1.SelectedIndex += 1;
            EnableButtons(true);
        }
    }
}
