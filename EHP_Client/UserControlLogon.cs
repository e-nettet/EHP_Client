using System;
using System.Windows.Forms;

namespace EHP_Client
{
    public partial class UserControlLogon : UserControl
    {
        private Miljoe miljoe;
        private string partID;
        private string actAs;
        private string password;

        public UserControlLogon()
        {
            InitializeComponent();
            comboBoxMiljoe.DataSource = Enum.GetValues(typeof(Miljoe));
        }

        public Miljoe Miljoe { get => miljoe; set => miljoe = value; }
        public string PartID { get => partID; set => partID = value; }
        public string ActAs { get => actAs; set => actAs = value; }
        public string Password { get => password; set => password = value; }

        private void ComboBoxMiljoe_SelectedIndexChanged(object sender, EventArgs e)
        {
            Miljoe = (Miljoe)comboBoxMiljoe.SelectedIndex;
        }

        private void TextBoxPartID_TextChanged(object sender, EventArgs e)
        {
            PartID = textBoxPartID.Text;
        }

        private void TextBoxPassword_TextChanged(object sender, EventArgs e)
        {
            Password = textBoxPassword.Text;
        }

        private void TextBoxActAs_TextChanged(object sender, EventArgs e)
        {
            ActAs = textBoxActAs.Text;
        }
    }
}
