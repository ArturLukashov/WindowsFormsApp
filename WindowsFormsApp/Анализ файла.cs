using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Анализ_файла : Form
    {
        public Анализ_файла()
        {
            InitializeComponent();
        }

        private void Анализ_файла_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = label1.Text + ".";

            if (label1.Text.Length >= 22)
            {
                label1.Text = label1.Text.Trim(new char[] { '.' });
            }
        }
    }
}
