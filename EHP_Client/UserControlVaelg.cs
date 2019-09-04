using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EHP_Client
{
    public partial class UserControlVaelg : UserControl
    {
        private Funktion function;

        public Funktion Function { get => function; set => function = value; }

        public UserControlVaelg()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            comboBox1.DataSource = Enum.GetValues(typeof(Funktion));
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Function = (Funktion)comboBox1.SelectedIndex;
        }
    }
}
