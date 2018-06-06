using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HentRestgaeld
{
    public partial class WizardTabcontrol : TabControl
    {
        public WizardTabcontrol()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void WndProc(ref Message m)
        {

            // Hide tabs by trapping the TCM_ADJUSTRECT message

            if (m.Msg == 0x1328 && !DesignMode) m.Result = (IntPtr)1;

            else base.WndProc(ref m);

        }
    }
}
