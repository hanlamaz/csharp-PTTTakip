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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public string secilenkargo = "";
        public int A = 1;
        public System.Windows.Forms.RadioButton Radioekle(string kargoadi)
        {
            System.Windows.Forms.RadioButton radio = new RadioButton();
            this.Controls.Add(radio);
            radio.Top = A * 28;
            radio.Left = 15;
            radio.Text = kargoadi;
            A = A + 1;
            radio.Click += new EventHandler(radio_Click);
            return radio;
        }
        public System.Windows.Forms.Button Buttonekle()
        {
            System.Windows.Forms.Button radio = new Button();
            this.Controls.Add(radio);
            radio.Top = A * 28;
            radio.Left = 15;
            radio.Text = "Tamam";
            radio.Click += new EventHandler(button_Click);
            A = A + 1;
            return radio;
        }
        private void button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void radio_Click(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                if (item is RadioButton)
                {
                    RadioButton rb = item as RadioButton;
                    if (rb.Checked==true)
                    {
                        secilenkargo = rb.Text;
                    }
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
    }
}
