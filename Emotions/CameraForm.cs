using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emotions
{
    public partial class CameraForm : Form
    {
        public CameraForm()
        {
            InitializeComponent();
        }

        private void CameraForm_Load(object sender, EventArgs e)
        {
           Application.Idle += capture;            
        }

        private void CameraForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            cap.Stop();
            cap.Dispose();
            cap = null;
            Application.Idle -= capture;
        }

    }
}
