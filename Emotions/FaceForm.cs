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
    public partial class FaceForm : Form
    {
        public FaceForm()
        {
            InitializeComponent();
        }

        private void FaceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.GC.Collect();
        }
    }

    partial class Form1
    {
        private void faceProp(object sender, EventArgs e)
        {
            FaceForm prop = new FaceForm();
            prop.Owner = this;
            Panel face = (Panel)sender;
            prop.Text += " " + face.TabIndex.ToString();
            prop.panel1.BackgroundImage = (Image)(face.BackgroundImage.Clone());
            prop.Show();
        }
    }

}
