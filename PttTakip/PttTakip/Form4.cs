using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Globalization;
using HtmlAgilityPack;
using System.Xml;
using System.Threading;

namespace PttTakip
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }
        /*
         YAPILACAKLAR
         FONKAIYONLAR ASYNC TASK OLACAK
         */
        public int inforeceivedtrigger = 0;
        public int aftershipfinished = 0;
        public int track24finished = 0;
        public int trackchinapostglobalfinished = 0;
        int trackchinapostglobalcounter = 0;
        int trackchinapostchinapostcounter = 0;
        int trackUpTrackcounter = 0;
        int track24counter = 0;
        bool stopexecute = false;
        bool webBrowser4loaded = false;
        bool webBrowser2loaded = false;

        private void Form4_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("tr-TR");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            listView1.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Tarih", 150);
            listView1.Columns.Add("Saat", 150);
            listView1.Columns.Add("Hareket/İşlem", 150);
            listView1.Columns.Add("Yer", 150);
            listView1.Columns.Add("Kargo Firması", 150);
            listView1.Columns[0].Width = -2;
            /*listView1.Columns[1].Width = -2;
            listView1.Columns[2].Width = -2;
            listView1.Columns[3].Width = -2;
            listView1.Columns[4].Width = -2;*/



        }

        public async Task AfterShipTakipEt(string kargokodu)
        {
            List<string> durum = new List<string>(); //
            List<string> kargo = new List<string>(); //
            List<string> yer = new List<string>();   //  -> Genel kargo durumları list i
            List<string> tarih = new List<string>(); //
            List<string> saat = new List<string>();  //
            System.Uri url = new Uri("https://track.aftership.com/" + kargokodu); // Örnek olarak -> https://track.aftership.com/1265562
            WebClient client = new WebClient(); // -> Webclient başlatır
            client.Encoding = Encoding.UTF8; // -> UTF-8 ile okunması dil kaynaklı sorunları çözer
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063"); // -> HTMLAGILITYPACK in bot tarzı hareketini algılanmasını engellemek için header ini değiştirdim
            string html = client.DownloadString(url); // -> Verilen url adresini indirir ve html'e atar
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument(); // -> doc1 adlı HTMLDocument oluşturur
            doc1.LoadHtml(html); // -> İndirilen html stringini doc1'e parametre olarak atar ve yükler ( indirilen string in ele alınabilmesini sağlar )
            string aranacakclass = "checkpoint__content"; // -> Html saysafındaki aranacak class.
            var aranacakclassSonuc =
               doc1.DocumentNode.SelectNodes(string.Format("//*[contains(@class,'{0}')]", aranacakclass)); // -> Aranılacak class taki bulunan tüm düğümleri aranacakclassSonuc'a atar . 
            if (aranacakclassSonuc != null) // -> Sonuç boş değilse
            {
                inforeceivedtrigger = 1;
                foreach (var item in aranacakclassSonuc) // -> Foreach döngüsü
                {
                    durum.Add(item.FirstChild.FirstChild.InnerText); //
                    kargo.Add(item.FirstChild.LastChild.InnerText);  //  -> item de debug işlemi yaparak durum , kargo şirketi ve yer bilgilerinin yerleri alınıp gerekli List'lere eklenir .
                    yer.Add(item.LastChild.InnerText);               //

                }
                aftershipfinished = 1;
            }
            else
            {
                List<string> kargolistesi = new List<string>();
                List<string> kargoadresi = new List<string>();
                string XPath = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[3]/div[1]/p[1]";
                var a = doc1.DocumentNode.SelectSingleNode(XPath);
                XPath = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]/div[1]/ul[1]/li[2]/a[1]";
                var b = doc1.DocumentNode.SelectSingleNode(XPath);
                if (a != null && a.InnerText == "Pending")
                {
                    MessageBox.Show("Kargonuz takip edilmek üzere ilk kez eklenmiş . Sonuçları görebilmek için daha sonra tekrar deneyin .AFTERSHIP STATUS(PENDING)");
                    inforeceivedtrigger = 0;
                    aftershipfinished = 1;
                    //this.Close();
                }
                else if (b != null)
                {
                    inforeceivedtrigger = 1;
                    kargolistesi.Add(b.InnerText);
                    kargoadresi.Add(b.Attributes[0].Value);
                    for (int i = 3; i < 100; i++)
                    {
                        XPath = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]/div[1]/ul[1]/li[" + i + "]/a[1]";
                        b = doc1.DocumentNode.SelectSingleNode(XPath);
                        if (b != null)
                        {
                            kargolistesi.Add(b.InnerText);
                            kargoadresi.Add(b.Attributes[0].Value);
                        }
                        else
                        {
                            break;
                        }
                    }
                    string[] dizi = kargolistesi.ToArray();
                    Form2 frm2 = new Form2();
                    for (int i = 0; i < dizi.Length; i++)
                    {
                        frm2.Radioekle(dizi[i]);
                        if (i == dizi.Length - 1)
                        {
                            frm2.Buttonekle();
                        }
                    }
                    frm2.Text = "Lütfen bir kargo firması seçiniz";
                    frm2.ShowDialog();
                    while (frm2.secilenkargo == "")
                    {
                        frm2.ShowDialog();
                    }
                    for (int i = 0; i < dizi.Length; i++)
                    {
                        if (dizi[i] == frm2.secilenkargo)
                        {

                            AfterShipTakipEt(kargoadresi[i].Remove(0, 1));
                        }
                    }
                    aftershipfinished = 1;
                }
                else
                {
                    MessageBox.Show("Kargo hatası . Muhtemelen kargo numarası yanlış girilmiş yada çok fazla istek yapılmış . Daha sonra tekrar deneyin . AFTERSHIP ERROR(NULLORHUMANCONTROL)");
                    inforeceivedtrigger = 0;
                    aftershipfinished = 1;
                    //this.Close();
                }


            }
            aranacakclass = "checkpoint__time";
            aranacakclassSonuc =
               doc1.DocumentNode.SelectNodes(string.Format("//*[contains(@class,'{0}')]", aranacakclass));
            if (aranacakclassSonuc != null)
            {
                foreach (var item in aranacakclassSonuc)
                {
                    tarih.Add(item.FirstChild.InnerText); //  -> itemde debug işlemi yapılarak tarih ve saat bigilerini gerekli List'lere ekler
                    saat.Add(item.LastChild.InnerText);   //

                }
            }
            int hatatrigger = 0;
            if (durum.Count == 0 || durum == null)
            {
                hatatrigger = 1;
            }
            if (hatatrigger != 1)
            {
                for (int i = durum.Count - 1; 0 <= i; i--) // -> Gelen sonuçlara bakıldığında kargo sonuçları ters zamanlı geliyordu bu yüzden tersten alındı
                {
                    string[] dizi = { TarihCevir(tarih[i]), SaatCevir(saat[i]), durum[i], yer[i], kargo[i] }; // Kargo hareketinin tüm birimleri eklenerek bir dizi oluşturulur
                    listView1.Items.Add(new ListViewItem(dizi)); // O dizi listview'e eklenir
                }


                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
            }
            aftershipfinished = 1;
        }

        public async Task AliexpressTakipEt(string kargokodu)
        {
            listView1.Items.Clear();
            System.Uri url = new Uri("https://global.cainiao.com/detail.htm?mailNoList=" + kargokodu); // Örnek olarak -> https://track.aftership.com/1265562
            WebClient client = new WebClient(); // -> Webclient başlatır
            client.Encoding = Encoding.UTF8; // -> UTF-8 ile okunması dil kaynaklı sorunları çözer
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063"); // -> HTMLAGILITYPACK in bot tarzı hareketini engellemek için
            string html = client.DownloadString(url); // -> Verilen url adresini indirir ve html'e atar
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument(); // -> doc1 adlı HTMLDocument oluşturur
            doc1.LoadHtml(html); // -> İndirilen html stringini doc1'e parametre olarak atar ve yükler ( indirilen string in ele alınabilmesini sağlar )
            List<string> durum = new List<string>(); //
            List<string> kargo = new List<string>(); //
            List<string> yer = new List<string>();   //  -> Genel kargo durumları list i
            List<string> tarih = new List<string>(); //
            List<string> saat = new List<string>();  //
                                                     /*string aranacakclass = "waybill-path"; // -> Html saysafındaki aranacak class.
                                                     var aranacakclassSonuc =
                                                        doc1.DocumentNode.SelectNodes(string.Format("//*[contains(@class,'{0}')]", aranacakclass)); // -> Aranılacak class taki bulunan tüm düğümleri aranacakclassSonuc'a atar . 
                                                     foreach (var item in aranacakclassSonuc) // -> Foreach döngüsü
                                                     {
                                                         MessageBox.Show(item.ToString());

                                                     }*/
            var b = doc1.GetElementbyId("waybill_list_val_box");
            if (b == null)
            {
                MessageBox.Show("Hata . Böyle bir kargo numarası bulunamadı . ERROR CAINAO(NULL)");
                inforeceivedtrigger = 0;
            }
            else
            {

                string s = b.InnerText;

                if (CainaoCozumleyicisi(s) == null)
                {
                    inforeceivedtrigger = 0;
                }
                else
                {
                    inforeceivedtrigger = 1;
                    string[] cozumleyicidizi = CainaoCozumleyicisi(s).ToArray();
                    if (cozumleyicidizi[cozumleyicidizi.Length - 1].ToString().EndsWith("MY") == true)
                    {
                        for (int i = cozumleyicidizi.Length - 2; i >= 1; i--)
                        {
                            if (i % 2 != 0)
                            {
                                string[] dizi = { TarihCevir(cozumleyicidizi[i]), SaatCevir(cozumleyicidizi[i]), cozumleyicidizi[i - 1], "", "" };
                                listView1.Items.Add(new ListViewItem(dizi));

                            }
                            else
                            {

                            }
                        }
                        string malezyakargokodu = cozumleyicidizi[cozumleyicidizi.Length - 1];
                        Task aftership = AfterShipTakipEt(malezyakargokodu);
                        await aftership;
                        if (inforeceivedtrigger == 0)
                        {
                            Task track24 = Track24(malezyakargokodu);
                            await track24;
                            if (inforeceivedtrigger == 0)
                            {
                                Task cainao = AliexpressTakipEt(malezyakargokodu);
                                await cainao;
                                if (inforeceivedtrigger == 0)
                                {
                                    Task globaltrack = GlobalTrack(malezyakargokodu);
                                    await globaltrack;
                                    if (inforeceivedtrigger == 0)
                                    {
                                        MessageBox.Show("MY(MALEZYA) KARGONUZ BULUNAMADI !");
                                        this.Close();
                                    }
                                }
                            }
                        }
                        inforeceivedtrigger = 1;
                    }
                    else
                    {
                        for (int i = cozumleyicidizi.Length - 1; i >= 1; i = i - 2)
                        {
                            if (true)
                            {
                                string[] dizi = { TarihCevir(cozumleyicidizi[i]), SaatCevir(cozumleyicidizi[i]), cozumleyicidizi[i - 1], "", "" };
                                listView1.Items.Add(new ListViewItem(dizi));

                            }
                            else
                            {

                            }
                        }
                    }

                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
                }

            }
        }

        public void TrackingMore(string kargokodu)
        {
            System.Uri url = new Uri("https://track.trackingmore.com/choose-en-" + kargokodu + ".html?"); // Örnek olarak -> https://track.aftership.com/1265562
            WebClient client = new WebClient(); // -> Webclient başlatır
            client.Encoding = Encoding.UTF8; // -> UTF-8 ile okunması dil kaynaklı sorunları çözer
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063"); // -> HTMLAGILITYPACK in bot tarzı hareketini engellemek için
            string html = client.DownloadString(url); // -> Verilen url adresini indirir ve html'e atar
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument(); // -> doc1 adlı HTMLDocument oluşturur
            doc1.LoadHtml(html); // -> İndirilen html stringini doc1'e parametre olarak atar ve yükler ( indirilen string in ele alınabilmesini sağlar )
            List<string> durum = new List<string>(); //
            List<string> kargo = new List<string>(); //
            List<string> yer = new List<string>();   //  -> Genel kargo durumları list i
            List<string> tarih = new List<string>(); //
            List<string> saat = new List<string>();  //
            string aranacakclass = "destination";
            var aranacakclassSonuc =
   doc1.DocumentNode.SelectNodes(string.Format("//*[contains(@data-role,'{0}')]", aranacakclass)); // -> Aranılacak class taki bulunan tüm düğümleri aranacakclassSonuc'a atar . 
            if (aranacakclassSonuc != null)
            {
                foreach (var item in aranacakclassSonuc[0].ChildNodes)
                {
                    if (item.ChildNodes.Count != 0)
                    {
                        if (item.ChildNodes[0].Name == "time" && item.ChildNodes[1].Name == "span")
                        {
                            string a = "";
                            a = item.ChildNodes[1].InnerText.Replace('Ý', 'İ');
                            a = a.Replace('Þ', 'Ş');
                            tarih.Add(TarihCevir(item.ChildNodes[0].InnerText));
                            saat.Add(SaatCevir(item.ChildNodes[0].InnerText));
                            durum.Add(a);
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("Nothing to do");
            }

            for (int i = durum.Count - 1; 0 <= i; i--) // -> Gelen sonuçlara bakıldığında kargo sonuçları ters zamanlı geliyordu bu yüzden tersten alındı
            {
                string[] dizi = { TarihCevir(tarih[i]), SaatCevir(saat[i]), durum[i], "", "" }; // Kargo hareketinin tüm birimleri eklenerek bir dizi oluşturulur
                listView1.Items.Add(new ListViewItem(dizi)); // O dizi listview'e eklenir
            }


            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
        }

        public void ChinaPostTrack(string kargokodu)
        {
            webBrowser1.Navigate("http://track-chinapost.com/startairmail.php?code=" + kargokodu);
            webBrowser1.ScriptErrorsSuppressed = true;

        }

        public async Task GlobalTrack(string kargokodu)
        {

            webBrowser2.Navigate("http://track-chinapost.com/track_global.php");
            webBrowser2.ScriptErrorsSuppressed = true;
            while (webBrowser2loaded == false)
            {
                await Task.Delay(2000);
            }
            Task t = trackchinapostglobal(kargokodu);
            await t;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            trackchinapostchinapost();
            trackchinapostchinapostcounter++;
            if (trackchinapostchinapostcounter == 3)
            {
                if (listviewcontrol(listView1) == true)
                {
                    inforeceivedtrigger = 1;
                }
                else
                {
                    inforeceivedtrigger = 0;
                }
            }
            /*HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            var diller = webBrowser1.Document.GetElementsByTagName("li");
            foreach (HtmlElement item in diller)
            {
                if (item.GetAttribute("data-lng") == "en")
                {
                    item.InvokeMember("click");
                }
            }
            webBrowser1.Document.GetElementById("inputTrackCode").InnerText = Form1.kargonumarası;
            //login in to account(fire a login button promagatelly)
            webBrowser1.Document.GetElementById("trackingButton").InvokeMember("click");
            webBrowser1.Document.GetElementById("trackingButton").InvokeMember("click");
            await Task.Delay(3000);
            string text = webBrowser1.DocumentText;
            doc1.LoadHtml(text);
            string aranacakclass = "date"; // -> Html saysafındaki aranacak class.
            var aranacakclassSonuc =
               doc1.DocumentNode.SelectNodes(string.Format("//span[contains(@class,'{0}')]", aranacakclass)); // -> Aranılacak class taki bulunan tüm düğümleri aranacakclassSonuc'a atar .     
            var nodes = doc1.DocumentNode.ChildNodes;
            string a = "//*[@id="+ "trackingInfoEvents" + "]/div[1]/div[3]/div[1]";
            var result = doc1.DocumentNode.SelectSingleNode(a);*/

            /*
             * PTT İÇİN
            await Task.Delay(5000);
            webBrowser1.Document.Window.ResizeTo(500, 500);
            webBrowser1.Document.GetElementById("barkod").InnerText = Form1.kargonumarası;
            var diller = webBrowser1.Document.GetElementsByTagName("img");
            System.Drawing.Rectangle yer = new Rectangle(0,0,0,0);
            foreach (HtmlElement item in diller)
            {
                yer = GetAbsoluteRectangle(item);
            }
            Bitmap bitmap = new Bitmap(yer.Width, yer.Height);
            webBrowser1.DrawToBitmap(bitmap, new Rectangle(0,0, yer.Width, yer.Height));
            pictureBox1.Image = bitmap;

            */

            /*var input1 = webBrowser1.Document.GetElementsByTagName("input");
            foreach (HtmlElement item in input1)
            {
                if (item.GetAttribute("alt") == "trackitonline search")
                {
                    item.InvokeMember("click");
                }
            }*/
        }

        private void webBrowser2_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            trackchinapostglobalcounter++;
            if (trackchinapostglobalcounter == 1)
            {
                webBrowser2loaded = true;
                /*Task t = trackchinapostglobal();
                await t;
                if (listviewcontrol(listView1) == true)
                {
                    inforeceivedtrigger = 1;
                }
                else
                {
                    inforeceivedtrigger = 0;
                }*/
            }
        }

        private async Task trackchinapostglobal(string kargokodu)
        {
            List<string> durum = new List<string>(); //
            List<string> tarih = new List<string>(); //
            List<string> saat = new List<string>();  //
            List<string> yer = new List<string>();  //
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            var textbox = webBrowser2.Document.GetElementsByTagName("input");
            foreach (HtmlElement item in textbox)
            {
                if (item.GetAttribute("name") == "code")
                {
                    item.InnerText = kargokodu;
                }

            }
            var button = webBrowser2.Document.GetElementsByTagName("input");
            foreach (HtmlElement item in button)
            {
                if (item.GetAttribute("value") == "Start Tracking")
                {
                    item.InvokeMember("click");
                }

            }
            await Task.Delay(3000);
            string text = webBrowser2.DocumentText;
            doc1.LoadHtml(text);
            var sonuclar = doc1.DocumentNode.SelectNodes("//td");
            if (sonuclar != null)
            {
                for (int i = 7; i < sonuclar.Count() - 1; i = i + 4)
                {
                    tarih.Add(sonuclar[i].InnerText);
                    saat.Add(sonuclar[i].InnerText);
                    durum.Add(sonuclar[i + 1].InnerText + "   " + sonuclar[i + 3].InnerText);
                    yer.Add(sonuclar[i + 2].InnerText);
                }
                for (int i = 0; i < durum.Count(); i++) // -> Gelen sonuçlara bakıldığında kargo sonuçları ters zamanlı geliyordu bu yüzden tersten alındı
                {
                    string[] dizi = { TarihCevir(tarih[i]), SaatCevir(saat[i]), durum[i].Trim(), yer[i].Trim(), "" }; // Kargo hareketinin tüm birimleri eklenerek bir dizi oluşturulur
                    listView1.Items.Add(new ListViewItem(dizi)); // O dizi listview'e eklenir
                }
                trackchinapostglobalfinished = 1;
                inforeceivedtrigger = 1;
            }
            else
            {
                trackchinapostglobalfinished = 1;
                inforeceivedtrigger = 0;
                return;
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
        }

        private void trackchinapostchinapost()
        {
            List<string> durum = new List<string>(); //
            List<string> tarih = new List<string>(); //
            List<string> saat = new List<string>();  //
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            var diller = webBrowser1.Document.GetElementsByTagName("input");
            foreach (HtmlElement item in diller)
            {
                if (item.GetAttribute("value") == "Click to Track")
                {
                    item.InvokeMember("click");
                }
            }
            string text = webBrowser1.DocumentText;
            doc1.LoadHtml(text);
            var sonuclar = doc1.DocumentNode.SelectNodes("//tr");
            if (sonuclar != null)
            {
                for (int i = 2; i < sonuclar.Count() - 1; i++)
                {
                    tarih.Add(sonuclar[i].ChildNodes[1].InnerText);
                    saat.Add(sonuclar[i].ChildNodes[1].InnerText);
                    durum.Add(sonuclar[i].ChildNodes[3].InnerText);
                }
                for (int i = 0; i < durum.Count(); i++) // -> Gelen sonuçlara bakıldığında kargo sonuçları ters zamanlı geliyordu bu yüzden tersten alındı
                {
                    string[] dizi = { TarihCevir(tarih[i]), SaatCevir(saat[i]), durum[i].Trim(), "", "China Post" }; // Kargo hareketinin tüm birimleri eklenerek bir dizi oluşturulur
                    listView1.Items.Add(new ListViewItem(dizi)); // O dizi listview'e eklenir
                }
            }
            else
            {
                MessageBox.Show("Sunucu meşgul yada kargo numaranız yanlış . Kargo numaranızdan eminseniz birkaç dakika sonra tekrar deneyin . ERROR TRACK-CHINAPOST(BUSYORWRONGCODE)");
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
        }

        private List<string> CainaoCozumleyicisi(string text)
        {

            string a = text.Replace("&quot", "");
            a = a.Replace(";:;", "");
            a = a.Replace(",:", "");
            a = a.Replace(";,;", "\n");
            a = a.Replace(";},{;desc", " ");
            string[] dizi = a.Split('\n');
            List<string> liste = dizi.ToList();
            if (15 <= liste.Count())
            {
                liste.RemoveRange(0, 10);
                liste.RemoveRange(liste.Count - 3, 3);
                for (int i = 0; i < liste.Count; i++)
                {
                    liste[i] = liste[i].Replace("detailList;:[{;desc", "");
                    liste[i] = liste[i].Replace("timeZone", "");
                    liste[i] = liste[i].Replace("timezone", "");
                    liste[i] = liste[i].Replace("time", "");
                    liste[i] = liste[i].Replace("status", "");
                    liste[i] = liste[i].Replace("status", "");
                    liste[i] = liste[i].Replace("cpNamePOSTTR;}],;section1;:{;countryNameChina", "");
                    liste[i] = liste[i].Replace("countryNameTurkey", "");



                    if (liste[i].StartsWith("status") || liste[i].StartsWith("SIGNIN") || liste[i].StartsWith("SIGNIN_EXC") || liste[i].StartsWith("ARRIVED_AT_DEST_COUNTRY") || liste[i].StartsWith("DEPART_FROM_ORIGINAL_COUNTRY") || liste[i].StartsWith("PICKEDUP") || liste[i].StartsWith("WAIT4PICKUP"))
                    {
                        liste.RemoveAt(i);
                    }
                    if (liste[i].StartsWith("time"))
                    {
                        liste[i] = liste[i].Remove(0, 4);
                    }
                    if (liste[i].StartsWith(" [-]"))
                    {
                        liste[i] = liste[i].Remove(0, 4);
                    }

                }
                for (int s = 0; s < 2; s++)
                {
                    for (int i = 0; i < liste.Count; i++)
                    {
                        if (liste[i].StartsWith("cpCode") || liste[i].StartsWith("cpNamePOSTTR;}],;") || liste[i].StartsWith("detailList") || liste[i].StartsWith("countryName") || liste[i].StartsWith("SHIPPING"))
                        {
                            liste.RemoveAt(i);
                        }
                        liste[i] = liste[i].Replace("detailList;:[]},;section2;:{;companyNamePTT", "");
                        if (liste[i] == null || liste[i] == "")
                        {
                            liste.RemoveAt(i);
                        }
                        if (s == 0)
                        {
                            if (i == liste.Count - 1)
                            {
                                liste[liste.Count - 1] = liste[liste.Count - 1].Remove(0, 11);
                            }
                        }

                        liste[i] = liste[i].Trim();
                    }


                }
                return liste;
            }
            else
            {
                return null;
            }


        }

        private bool listviewcontrol(ListView list)
        {
            if (list.Items.Count == 0)
            {

                return false;
            }
            else
            {
                return true;
            }
        }

        private string SaatCevir(string saat) // Alınan stringi 24 saatlik birime çevirerek geri döndürür / Örnek SaatCevir(7:00 AM) -> 07:00
        {
            DateTime dt = DateTime.Parse(saat);
            return dt.ToString("HH:mm");
        }

        private string TarihCevir(string tarih) // Alınan tarihi dd/mm/yyyy formatına çevirerek döndürür
        {

            DateTime dt = DateTime.Parse(tarih);
            return String.Format("{0:dd/MM/yyyy}", dt);
        }

        public void UpTrack(string kargokodu)
        {
            webBrowser3.Navigate("https://myparcels.net/");
            webBrowser3.ScriptErrorsSuppressed = true;

        }

        private void trackuptrack()
        {
            List<string> durum = new List<string>(); //
            List<string> yer = new List<string>();  //
                                                    //await Task.Delay(5000);
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            //webBrowser3.Document.GetElementById("one-time-track-input").InnerText = Form1.kargonumarası;
            var button = webBrowser3.Document.GetElementsByTagName("input");
            foreach (HtmlElement item in button)
            {
                if (item.GetAttribute("id") == "one-time-track-input")
                {
                    item.InnerText = Form1.kargonumarası;
                }
                if (item.GetAttribute("value") == "Verify")
                {
                    item.InvokeMember("click");
                }
            }
            //await Task.Delay(5000);
            var detailsbutton = webBrowser3.Document.GetElementsByTagName("a");
            foreach (HtmlElement item in detailsbutton)
            {
                if (item.GetAttribute("rel") == "redrose")
                {
                    item.InvokeMember("click");
                }
            }
            string text = webBrowser3.DocumentText;
            var items = webBrowser3.Document.GetElementsByTagName("div");
            List<string> stringlist = new List<string>();
            foreach (HtmlElement item in items)
            {
                if (item.InnerText != null)
                {
                    stringlist.Add(item.InnerText);
                }
            }
            var blabal = stringlist;
            string[] sonucdizi;
            if (stringlist[27].Length < stringlist[26].Length)
            {
                sonucdizi = stringlist[26].Split('\n');
            }
            else
            {
                sonucdizi = stringlist[27].Split('\n');
            }
            List<string> tarihsaatlist = new List<string>();

            for (int i = 0; i < sonucdizi.Length; i++)
            {
                int s = 0;
                foreach (char item in sonucdizi[i])
                {
                    if (Char.IsNumber(item))
                    {
                        string a = sonucdizi[i].Remove(0, s);
                        a = a.Replace('\r', ' ');
                        tarihsaatlist.Add(a);
                        durum.Add(sonucdizi[i].Remove(s, sonucdizi[i].Length - s));
                        break;
                    }
                    else
                    {
                        s++;
                    }
                }
            }

            tarihsaatlist.RemoveAt(tarihsaatlist.Count - 1);
            durum.RemoveAt(durum.Count - 1);

            for (int i = 0; i < durum.Count(); i++) // -> Gelen sonuçlara bakıldığında kargo sonuçları ters zamanlı geliyordu bu yüzden tersten alındı
            {

                string[] dizi = { TarihCevir(tarihsaatlist[i]), SaatCevir(tarihsaatlist[i]), durum[i].Trim(), "", "" }; // Kargo hareketinin tüm birimleri eklenerek bir dizi oluşturulur
                if (listView1.Items.Count < durum.Count())
                {
                    listView1.Items.Add(new ListViewItem(dizi)); // O dizi listview'e eklenir
                }

                //catch (Exception)
                //{
                //    MessageBox.Show("Kargo sonucu dönmedi . Lütfen daha sonra tekrar deneyin . ERROR MYPARCELS(ADSRETURNED)");
                //    stopexecute = true;
                //}

            }


            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
        }

        public async Task Track24(string kargokodu)
        {
            webBrowser4.Navigate("https://track24.net/");
            webBrowser4.ScriptErrorsSuppressed = true;
            while (webBrowser4loaded == false)
            {
                await Task.Delay(2000);
            }
            Task t = tracktrack24(kargokodu);
            await t;
        }

        private async Task tracktrack24(string kargokodu)
        {
            List<string> kargo = new List<string>(); //
            List<string> durum = new List<string>(); //
            List<string> yer = new List<string>();  //
            List<string> saatvezaman = new List<string>();  //
            webBrowser4.Document.GetElementById("inputTrackCode").InnerText = kargokodu;
            var button = webBrowser4.Document.GetElementsByTagName("li");
            foreach (HtmlElement item in button)
            {
                if (item.GetAttribute("data-lng") == "en")
                {
                    item.InvokeMember("click");
                }
            }
            var buton = webBrowser4.Document.GetElementsByTagName("button");
            foreach (HtmlElement item in buton)
            {
                if (item.GetAttribute("type") == "submit")
                {
                    item.InvokeMember("click");
                }
            }
            List<HtmlElement> elements = new List<HtmlElement>();
            await Task.Delay(5000);
            var sonuc = webBrowser4.Document.GetElementById("trackingEvents");
            /*var sonuc = webBrowser4.Document.GetElementsByTagName("div");
            foreach (HtmlElement item in sonuc)
            {
                if (item.GetAttribute("id") == "trackingInfoEvents")
                {
                    elements.Add(item);
                }
            }*/
            foreach (HtmlElement item in sonuc.Children)
            {
                if (item.Children.Count>=3)
                {
                    saatvezaman.Add(item.Children[0].InnerText);
                    durum.Add(item.Children[2].InnerText);
                    kargo.Add(item.Children[3].InnerText);
                }

            }
            if (track24counter != 0)
            {
                track24finished = 1;
                inforeceivedtrigger = 1;
                return;
            }
            else
            {
                track24counter = 1;
                for (int i = 0; i < durum.Count; i++)
                {
                    string[] dizi = { TarihCevir(track24zaman(track24cleaner(saatvezaman[i]))[0]), SaatCevir(track24zaman(track24cleaner(saatvezaman[i]))[1]), track24cleaner(durum[i].Trim()), "", track24cleaner(kargo[i]) }; // Kargo hareketinin tüm birimleri eklenerek bir dizi oluşturulur
                    listView1.Items.Add(new ListViewItem(dizi)); // O dizi listview'e eklenir
                }
                if (listView1.Items.Count == 1 || listView1.Items.Count == 0)
                {
                    MessageBox.Show("Uyarı . Kargonuz bulundu ancak kargo için beklemek gerek . Lütfen birkaç dakika sonra tekrar deneyin . ERROR TRACK24(ADDED)");
                    inforeceivedtrigger = 0;
                    track24finished = 1;
                }
                else
                {
                    track24finished = 1;
                    inforeceivedtrigger = 1;
                }
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);       //  -> Listview'i otomatik olarak genişliğini ayarlar .
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); //
        }



        private string track24cleaner(string text)
        {
            return text.Replace("\r", "").Replace("\n", " ");
        }
        private string[] track24zaman(string text)
        {
            string[] dizi = text.Split(' ');
            return dizi;
        }



        private void webBrowser3_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (trackUpTrackcounter <= 5)
            {


                if (stopexecute == false)
                {
                    if (listviewcontrol(listView1) == true)
                    {
                        stopexecute = true;
                        inforeceivedtrigger = 1;
                        return;
                    }
                    else
                    {
                        inforeceivedtrigger = 0;
                        trackuptrack();
                        trackUpTrackcounter++;
                    }



                }
                else
                {
                    if (listviewcontrol(listView1) == true)
                    {
                        inforeceivedtrigger = 1;
                        return;
                    }
                    else
                    {
                        inforeceivedtrigger = 0;
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Bu kargo sistemde bulunamadı . Bu uyarıyı birkaç kere alabilirsiniz. ERROR MYPARCEL(NOINFO)");
                return;

            }

        }

        private void webBrowser4_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser4loaded = true;
        }

    }
}


