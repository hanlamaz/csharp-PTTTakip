using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace PttTakip
{
    public partial class Form1 : Form
    {
        public static string kargonumarası;
        public static string kargoaciklamasi;
        public static string eklenecekkargonumarasi;
        public static string eklenecekkargoaciklamasi;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Func2();
            linkLabel1.Text = "Blog Adresim";
            linkLabel1.Links.Add(0, linkLabel1.Text.Length, "https://hanlamaz.wordpress.com");
            linkLabel2.Text = "acomputerengineer";
            linkLabel2.Links.Add(0, linkLabel2.Text.Length, "https://forum.donanimhaber.com/showProfile.asp?memID=1599949");
        }



        public void button1_Click(object sender, EventArgs e)
        {

            Func();

        }

        public void Func()// Kargo numarasını ve açıklamasını ekler
        {
            int s = 0;
            string kargonumaram = "";
            string kargoaciklamam = "";
            if (/*textBox1.Text == "" || textBox2.Text == "" ||*/ eklenecekkargoaciklamasi != "" && eklenecekkargonumarasi != "" && eklenecekkargoaciklamasi != " " && eklenecekkargonumarasi != " " && eklenecekkargoaciklamasi != null && eklenecekkargonumarasi != null)
            {
                kargonumaram = eklenecekkargonumarasi;
                kargoaciklamam = eklenecekkargoaciklamasi;
            }
            
            else if (/*eklenecekkargonumarasi == "" || eklenecekkargoaciklamasi == "" ||*/ textBox2.Text != "" && textBox1.Text != "" && textBox2.Text != " " && textBox1.Text != " " && textBox2.Text != null && textBox1.Text != null && textBox1.Text.Trim().Length !=0 && textBox2.Text.Trim().Length != 0)
            {
                kargonumaram = textBox1.Text;
                kargoaciklamam = textBox2.Text;
            }
            else
            {
                MessageBox.Show("Hata . Boş Kargo Numarası veya Kargo Açıklaması Eklenemez");
                s = 1;
            }
            if (aciklamakontrol(kargonumaram, kargoaciklamam) && kargonumkontrol(kargonumaram, kargoaciklamam) && s != 1) // Eğer kargo numarası ve açıklaması boş , null veya aynısı dosyada bulunmuyor ise dosyaya ekle.
            {
                string dosya_yolu = @"kargobilgileri.txt";
                //İşlem yapacağımız dosyanın yolunu belirtiyoruz.
                FileStream fs = new FileStream(dosya_yolu, FileMode.Append, FileAccess.Write);
                //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
                //2.parametre dosya varsa açılacağını yoksa oluşturulacağını belirtir,
                //3.parametre dosyaya erişimin veri yazmak için olacağını gösterir.
                StreamWriter sw = new StreamWriter(fs);
                //Yazma işlemi için bir StreamWriter nesnesi oluşturduk.
                try
                {
                    sw.WriteLine(kargonumaram); // Dosyanın sonuna
                    sw.WriteLine(kargoaciklamam);// kargo numarası ve açıklamasını ekledik
                    string[] dizi = { kargonumaram, kargoaciklamam };// Listview e eklemek için bir dizi oluşturduk
                    listView1.Items.Add(new ListViewItem(dizi));     // O diziyi listview e ekledik
                    textBox1.Clear();
                    textBox2.Clear();
                }
                catch (Exception)
                {

                    throw;
                }


                //Dosyaya ekleyeceğimiz iki satırlık yazıyı WriteLine() metodu ile yazacağız.
                sw.Flush();
                //Veriyi tampon bölgeden dosyaya aktardık.
                sw.Close();
                fs.Close();

                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //

            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e) // Seçilen kargo numarasını ve açıklamasını kaldırır
        {
            string[] lines = File.ReadAllLines("kargobilgileri.txt"); // Dosyayı okuyup içindeki tüm yazıyı satır şeklinde lines dizisine atadık
            List<string> list = new List<string>(); // Yeni bir list oluşturup
            list = lines.ToList<string>();          // lines dizisine atadım ( handle lamak daha kolay )
            if (list.Count < 2 || listView1.SelectedIndices.Count == 0 || listView1.SelectedItems.Count == 0) // Eğer herhangibir eleman yoksa yada tıklanmamışsa hata ver
            {
                MessageBox.Show("Hata . Silinmek için bir kargo seçilmemiş");
            }
            else
            {
                int index = listView1.SelectedIndices[0];
                string p = listView1.SelectedItems[0].Text; // Kargo numarasını p stringine atadım
                string s = listView1.SelectedItems[0].SubItems[1].Text; // Kargo açıklamasını s stringine atadım
                if (list.Count == 2)// Eğer sadece tek bir kargo varsa onu sil
                {
                    list.Clear();
                }
                else// Değil ise
                {
                    list.Remove(p);// listden p değişkenini sil
                    list.Remove(s);// listden s değişkenini sil
                }

                string[] newLines = list.ToArray<string>();// Değiştirilen list i newLines dizisine aktardım
                File.WriteAllLines("kargobilgileri.txt", newLines);// newLines dizinin içeriğini dosyaya yazdır
                listView1.Items[index].Remove(); // Listviewde o indexteki elemanı kaldır
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("KARGO BİLGİLERİNİ GÖRÜNTÜLEMEK İÇİN KARGOYA TIKLAYIN VE KARGO DETAYI BUTONUNA BASIN");
            MessageBox.Show("KARGO SİLMEK İÇİN KARGOYA TIKLAYIN VE KARGO SİL BUTONUNA BASIN");
        }

        private bool aciklamakontrol(string kargonum, string kargoacik)// Verilen kargo açıklaması dizide varmı diye kontrol eder . Varsa false , yoksa true döner
        {
            string[] lines = File.ReadAllLines("kargobilgileri.txt");
            List<string> list = new List<string>();
            list = lines.ToList<string>();
            if (list.Contains(kargoacik))
            {
                MessageBox.Show("Hata . Aynı Açıklama Tekrar Eklenemez");
                return false;
            }
            if (kargoacik == null || kargoacik == " ")
            {
                MessageBox.Show("Hata . Kargo Açıklaması Boş Olamaz");
                return false;
            }
            return true;
        }

        private bool kargonumkontrol(string kargonum, string kargoacik)// Verilen kargo numarası dizide varmı diye kontrol eder . Varsa false , yoksa true döner
        {
            string[] lines = File.ReadAllLines("kargobilgileri.txt");
            List<string> list = new List<string>();
            list = lines.ToList<string>();
            if (list.Contains(kargonum))
            {
                MessageBox.Show("Hata . Aynı Kargo Numarası Tekrar Eklenemez");
                return false;
            }
            if (kargonum == null || kargonum == " ")
            {
                MessageBox.Show("Hata . Kargo Numarası Boş Olamaz");
                return false;
            }
            return true;
        }



        private void button5_Click(object sender, EventArgs e)
        {
            Form3 topluekle = new Form3();
            topluekle.ShowDialog();
            Func2();
        }

        public void Func2()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Kargo Numarası", 250);
            listView1.Columns.Add("Kargo Açıklaması", 350);
            string dosya_yolu = @"kargobilgileri.txt";
            //Okuma işlem yapacağımız dosyanın yolunu belirtiyoruz.
            FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.Read);
            //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
            //2.parametre dosyanın açılacağını,
            //3.parametre dosyaya erişimin veri okumak için olacağını gösterir.
            StreamReader sw = new StreamReader(fs);

            //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
            string yazi = sw.ReadLine();
            while (yazi != null || yazi == " ")
            {
                List<string> list = new List<string>();
                list.Add(yazi);
                yazi = sw.ReadLine();
                list.Add(yazi);
                string[] dizi = list.ToArray<string>();
                listView1.Items.Add(new ListViewItem(dizi));
                yazi = sw.ReadLine();
            }
            //Satır satır okuma işlemini gerçekleştirdik ve ekrana yazdırdık
            //Son satır okunduktan sonra okuma işlemini bitirdik
            sw.Close();
            fs.Close();
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Func2();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.Filter = "Metin Belgesi(.txt) |*.txt";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string dosya_yolu = openFileDialog1.FileName;
                //Okuma işlem yapacağımız dosyanın yolunu belirtiyoruz.
                FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
                //2.parametre dosyanın açılacağını,
                //3.parametre dosyaya erişimin veri okumak için olacağını gösterir.
                StreamReader sw = new StreamReader(fs);
                StreamWriter sr = new StreamWriter(fs);
                //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
                string yazi = sw.ReadLine();
                int counter = 0;
                char[] ayrac = { ',' };
                while (yazi != null || yazi == " ")
                {
                    textBox1.Clear();
                    textBox2.Clear();
                    string[] a = yazi.Split(ayrac);
                    counter++;
                    try
                    {
                        eklenecekkargoaciklamasi = a[1];
                        eklenecekkargonumarasi = a[0];
                    }
                    catch (Exception)
                    {

                        MessageBox.Show(counter + "inci satırda hata var . Lütfen kontrol edip tekrar yükleyiniz . Not : Hatalılar eklenmez , hatasızlar eklenir");
                    }

                    Func();
                    yazi = sw.ReadLine();

                }

                eklenecekkargoaciklamasi = "";
                eklenecekkargonumarasi = "";
                //Satır satır okuma işlemini gerçekleştirdik ve ekrana yazdırdık
                //Son satır okunduktan sonra okuma işlemini bitirdik
                sw.Close();
                fs.Close();
            }
        }
                        
        private async void button8_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                kargonumarası = listView1.SelectedItems[0].Text.ToString();
                //kargoaciklamasi = listView1.SelectedItems[1].Text.ToString();
            }

            if (kargonumarası == null || kargonumarası == " " || kargonumarası == "")
            {
                MessageBox.Show("Boş Kargo Numarası Takip Edilemez :)");
            }
            else
            {
                if (CheckForInternetConnection() == false)
                {
                    MessageBox.Show("İnternet bağlantısı yok . Lütfen internete bağlanıp tekrar deneyin");
                }
                else if (listView1.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Hata . Hiçbir kargo takip edilmek için seçilmemiş");
                }
                else
                {
                    string kargokodu = listView1.SelectedItems[0].Text.ToString();
                    Form4 f4 = new Form4();
                    f4.Text = kargokodu + " kargosunun bilgileri";
                    f4.Show();
                    if (kargokodu.StartsWith("LP") || kargokodu.Length == 16)
                    {
                        Task cainao = f4.AliexpressTakipEt(kargokodu);
                        await cainao;
                        if (f4.inforeceivedtrigger == 0)
                        {
                            MessageBox.Show("Aliexpress kodunuz yanlış ya da henüz veri girilmemiş . Lütfen bekleyin");
                        }
                    }
                    else
                    {
                        Task track24 = f4.Track24(kargokodu);
                        await track24;
                        if (f4.inforeceivedtrigger == 0)
                        {
                            Task cainao = f4.AliexpressTakipEt(kargokodu);
                            await cainao;
                            if (f4.inforeceivedtrigger == 0)
                            {
                                Task aftership = f4.AfterShipTakipEt(kargokodu);
                                await aftership;
                                if (f4.inforeceivedtrigger == 0)
                                {
                                    Task globaltrack = f4.GlobalTrack(kargokodu);
                                    await globaltrack;
                                    if (f4.inforeceivedtrigger == 0)
                                    {
                                        MessageBox.Show("KARGONUZ BULUNAMADI !");
                                        f4.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.Start("https://hanlamaz.wordpress.com/");
        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}




