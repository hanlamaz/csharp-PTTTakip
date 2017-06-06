using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PttTakip
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }
        private bool webBrowser1completed = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Count()==0)
            {
                MessageBox.Show("Yemez");
            }
            else
            {
               
            }
        }
    }
}
