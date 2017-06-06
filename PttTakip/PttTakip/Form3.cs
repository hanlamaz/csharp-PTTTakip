using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PttTakip
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int s = 0;
            Form1 kargoekle = new Form1();
            int p = textBox1.Lines.Length;
            char[] ayrac = { ',' };
            try
            {
                for (int i = 0; i < p; i++)
                {
                    string[] kargo = textBox1.Lines[i].Split(ayrac);
                    Form1.eklenecekkargonumarasi = kargo[0];
                    Form1.eklenecekkargoaciklamasi = kargo[1];
                    kargoekle.Func();

                }


            }
            catch (Exception)
            {

                MessageBox.Show("HATA . Lütfen kargoları kurallara uygun bir şekilde tekrar ekleyin");
                s = 1;
            }
            Form1.eklenecekkargoaciklamasi = "";
            Form1.eklenecekkargonumarasi = "";
            if (s != 1)
            {
                MessageBox.Show("Kargolar başarı ile eklendi");
                this.Close();
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }

}
